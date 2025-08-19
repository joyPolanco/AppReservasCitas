import React, { useState, useEffect } from "react";
import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import { useUser } from "../UserContext";
import Swal from "sweetalert2";
import esLocale from "@fullcalendar/core/locales/es";
import "../estilos/calendar.css";

const AgendaUsuario = () => {
    const [events, setEvents] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const { usuario } = useUser();
    const token = sessionStorage.getItem("token");

    // Función para traer eventos
    const fetchEvents = async () => {
        try {
            const res = await fetch(
                "https://localhost:7210/api/Planificacion/getSlots",
                { headers: { Authorization: `Bearer ${token}` } }
            );

            if (!res.ok) throw new Error("No se pudieron cargar los slots");

            const data = await res.json();
            const formatted = data.map((slot) => ({
                id: slot.publicId,
                title: `${slot.horaInicio} - ${slot.horaFin}`,
                start: `${slot.fecha}T${slot.horaInicio}`,
                end: `${slot.fecha}T${slot.horaFin}`,
                backgroundColor: slot.disponible ? "#4caf50" : "#e0e0e0",
                borderColor: slot.disponible ? "#2e7d32" : "#999",
                textColor: slot.disponible ? "#fff" : "#555",
                extendedProps: { available: slot.disponible },
            }));
            setEvents(formatted);
            setIsLoading(false);
        } catch (err) {
            console.error(err);
            Swal.fire("Error", "No se pudieron cargar los slots", "error");
            setIsLoading(false);
        }
    };

    // Refresco cada 20 segundos
    useEffect(() => {
        fetchEvents();
        const interval = setInterval(fetchEvents, 20000);
        return () => clearInterval(interval);
    }, [token]);

    // Manejo de click en slot
    const handleEventClick = async (info) => {
        if (!info.event.extendedProps.available) {
            Swal.fire(
                "Turno ocupado",
                "Este horario ya fue reservado",
                "error"
            );
            return;
        }

        Swal.fire({
            title: "Confirmar reserva",
            text: `¿Deseas reservar el turno del ${info.event.start.toLocaleString()} al ${info.event.end.toLocaleTimeString()}?`,
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, reservar",
            cancelButtonText: "Cancelar",
        }).then(async (result) => {
            if (!result.isConfirmed) return;

            try {
                const response = await fetch(
                    "https://localhost:7210/api/Reservas/crear",
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`,
                        },
                        body: JSON.stringify({
                            CorreoUsuario: usuario.correo,
                            IdSlot: info.event.id,
                        }),
                    }
                );

                const data = await response.json();

                if (!response.ok) throw new Error(data.error || "Error al reservar");

                Swal.fire("Reservado", data.mensaje, "success");
                fetchEvents(); // actualizar slots
            } catch (err) {
                Swal.fire("Error", err.message, "error");
            }
        });
    };

    if (isLoading) return <p>Cargando agenda...</p>;

    return (
        <div className="calendar-wrapper" style={{ width: "100%", height: "86vh", padding: "20px"  ,backgroundColor:"white"}}>
            <h2 style={{ textAlign: "center", marginBottom: "15px", color: "rebeccapurple" }}>
                Selecciona un slot disponible (verde) para reservar tu turno
            </h2>
            <FullCalendar
                plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
                initialView="timeGridWeek"
                locale={esLocale}
                headerToolbar={{
                    left: "prev,next today",
                    center: "title",
                    right: "dayGridMonth,timeGridWeek,timeGridDay",
                }}
                events={events}
                eventClick={handleEventClick}
                editable={false}
                selectable={true}
                height="95%"
                eventClassNames={(eventInfo) => [
                    "custom-event",
                    eventInfo.event.extendedProps.available ? "available" : "reserved",
                ]}
                eventContent={(eventInfo) => (
                    <div
                        style={{
                            cursor: "pointer",
                            padding: "5px",
                            borderRadius: "5px",
                            fontWeight: 500,
                            textAlign: "center",
                        }}
                    >
                        {eventInfo.event.title}
                    </div>
                )}
            />
        </div>
    );
};

export default AgendaUsuario;

import { useCalendarApp, ScheduleXCalendar } from '@schedule-x/react'
import {
    createViewDay,
    createViewWeek,
    createViewMonthGrid,
    createViewMonthAgenda,
} from '@schedule-x/calendar'
import { createEventsServicePlugin } from '@schedule-x/events-service'
import '@schedule-x/theme-default/dist/index.css'
import { useState, useEffect } from 'react'
import "../estilos/admin.css"
import { useUser } from '../UserContext';

function CalendarUser() {
    const eventsService = useState(() => createEventsServicePlugin())[0]
    const [isLoading, setIsLoading] = useState(true)
    const { usuario } = useUser(); // Obtenemos el usuario del contexto
    const correo = usuario?.correo || ""; // Por si usuario no tiene correo

    const calendar = useCalendarApp({
        views: [
            createViewDay(),
            createViewWeek(),
            createViewMonthGrid(),
            createViewMonthAgenda()
        ],
        events: [], // inicialmente vacío
        plugins: [eventsService],
        options: {
            eventOverlap: false,
            slotMinHeight: 80,
            slotMinTime: "08:00",
            slotMaxTime: "12:00",
            gridHeight: 2400,
            dayBoundaries: { start: '06:00', end: '18:00' }
        },
        onEventClick: (event) => {
            alert(`Evento\nFecha: ${event.extendedProps.fecha}\nEstado: ${event.extendedProps.estado}`)
        }
    })

    const normalizeTime = (time) => {
        if (!time) return "00:00";
        if (/^\d{2}:\d{2}:\d{2}$/.test(time)) return time.slice(0, 5);
        if (/^\d{2}:\d{2}$/.test(time)) return time;
        return time;
    }

    useEffect(() => {
        async function fetchEvents() {
            if (!correo) {
                setIsLoading(false);
                return;
            }

            try {
                const token = sessionStorage.getItem("token");
                const res = await fetch(`https://localhost:7210/api/Reservas/reservas-usuario?correo=${encodeURIComponent(correo)}`, {
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": "Bearer " + token
                    }
                });

                const data = await res.json();

                if (!Array.isArray(data)) {
                    console.error('API no devolvió un array:', data);
                    setIsLoading(false);
                    return;
                }

                const formatted = data.map((reserva, index) => {
                    const startTime = normalizeTime(reserva.horaInicio);
                    const endTime = normalizeTime(reserva.horaFin);

                    return {
                        id: index.toString(),
                        title: `Estado: ${reserva.estado}`,
                        start: `${reserva.fecha} ${startTime}`,
                        end: `${reserva.fecha} ${endTime}`,
                        extendedProps: {
                            fecha: reserva.fecha,
                            estado: reserva.estado
                        }
                    };
                });

                eventsService.set(formatted);
                setIsLoading(false);

            } catch (err) {
                console.error('Error fetching events:', err);
                setIsLoading(false);
            }
        }

        fetchEvents();
    }, [eventsService, correo]);

    if (isLoading) return <div>Cargando calendario...</div>

    return (<div className="calendar-user">
        <ScheduleXCalendar calendarApp={calendar} />
     </div> 
    );
   
}

export default CalendarUser

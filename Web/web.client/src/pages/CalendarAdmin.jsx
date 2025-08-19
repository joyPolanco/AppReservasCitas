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

function CalendarAdmin() {
    const eventsService = useState(() => createEventsServicePlugin())[0]
    const [isLoading, setIsLoading] = useState(true)

    const calendar = useCalendarApp({
        views: [
            createViewDay(),
            createViewWeek(),
            createViewMonthGrid(),
            createViewMonthAgenda()
        ],
        events: [],
        plugins: [eventsService],
        options: {
            eventOverlap: false,
            slotMinHeight: 90,
            slotMinTime: "06:00",
            slotMaxTime: "23:00",
            gridHeight: 2000, // Altura inicial, puede cambiarse
            dayBoundaries: { start: '06:00', end: '23:00' }
        },
        onEventClick: (event) => {
            alert(`Turno: ${event.title}\nFecha: ${event.extendedProps.fecha}\nHora inicio: ${event.extendedProps.horaInicio}\nHora fin: ${event.extendedProps.horaFin}`)
        }
    })

    const convertTo24h = (name, time) => {
        if (!time) return "00:00"
        let [hours, minutes] = time.split(':').map(Number)
        if (name.toLowerCase() === 'vespertino' && hours < 12) hours += 12
        return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`
    }

    useEffect(() => {
        async function fetchEvents() {
            try {
                const token = sessionStorage.getItem("token")
                const res = await fetch('https://localhost:7210/api/Planificacion/getTurnosActivos', {
                    headers: { "Content-Type": "application/json", "Authorization": "Bearer " + token }
                })

                const data = await res.json()
                const formatted = data.flatMap((dia, indexDia) =>
                    dia.turnos.map((turno, indexTurno) => {
                        const startTime = convertTo24h(turno.nombre, turno.horaInicio)
                        const endTime = convertTo24h(turno.nombre, turno.horaFin)
                        return {
                            id: `${indexDia}-${indexTurno}`,
                            title: turno.nombre,
                            start: `${dia.fecha} ${startTime}`,
                            end: `${dia.fecha} ${endTime}`,
                            extendedProps: { fecha: dia.fecha, horaInicio: startTime, horaFin: endTime }
                        }
                    })
                )

                eventsService.set(formatted)
                setIsLoading(false)
            } catch (err) {
                console.error('Error fetching events:', err)
                setIsLoading(false)
            }
        }

        fetchEvents()
    }, [eventsService])

    if (isLoading) return <div>Cargando calendario...</div>

    return (
        <div style={{ height: '100vh', overflowY: 'auto' }}>
            <ScheduleXCalendar calendarApp={calendar} />
        </div>
    )
}

export default CalendarAdmin

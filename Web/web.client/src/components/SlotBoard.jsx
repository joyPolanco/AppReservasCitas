import React, { useState } from 'react';
import '../estilos/reservationCalendar.css';

const ReservationCalendar = ({ disabledSlots = [], onSlotSelect }) => {
    // Configuración básica
    const config = {
        availableHours: { start: 8, end: 18 }, // 8AM - 6PM
        weekStartsOn: 1 // Lunes (0 es Domingo)
    };

    // Estado
    const [currentDate, setCurrentDate] = useState(new Date());
    const [selectedSlot, setSelectedSlot] = useState(null);

    // Datos de prueba de ejemplo con diferentes duraciones
    const testDisabledSlots = [
        {
            start: new Date(new Date().setHours(10, 0, 0, 0)),
            end: new Date(new Date().setHours(12, 30, 0, 0)), // Duración: 2.5 horas
            title: "Reunión larga"
        },
        {
            start: new Date(new Date().setHours(14, 0, 0, 0)),
            end: new Date(new Date().setHours(14, 45, 0, 0)), // Duración: 45 minutos
            title: "Sesión corta"
        },
        {
            start: new Date(new Date().setDate(new Date().getDate() + 1)),
            end: new Date(new Date().setDate(new Date().getDate() + 1)),
            title: "Evento de día completo"
        }
    ];

    const finalDisabledSlots = disabledSlots.length > 0 ? disabledSlots : testDisabledSlots;

    // Generar días de la semana
    const getWeekDays = () => {
        const startDate = new Date(currentDate);
        const day = startDate.getDay();
        const diff = startDate.getDate() - day + (day === 0 ? -6 : config.weekStartsOn);
        startDate.setDate(diff);

        return Array.from({ length: 7 }).map((_, i) => {
            const date = new Date(startDate);
            date.setDate(date.getDate() + i);
            return date;
        });
    };

    // Calcular altura en px basada en la duración (4px por minuto)
    const calculateSlotHeight = (start, end) => {
        const durationMinutes = (end - start) / (1000 * 60);
        return durationMinutes * 0.8; // Ajuste para mejor visualización
    };

    // Generar slots disponibles para un día específico
    const generateAvailableSlots = (date) => {
        const slots = [];
        let currentTime = new Date(date);
        currentTime.setHours(config.availableHours.start, 0, 0, 0);

        const endTime = new Date(date);
        endTime.setHours(config.availableHours.end, 0, 0, 0);

        // Encontrar todos los slots deshabilitados para este día
        const dayDisabledSlots = finalDisabledSlots.filter(slot => {
            return new Date(slot.start).toDateString() === date.toDateString();
        });

        // Si no hay slots deshabilitados, todo el día está disponible
        if (dayDisabledSlots.length === 0) {
            return [{
                start: new Date(currentTime),
                end: new Date(endTime),
                available: true,
                height: calculateSlotHeight(currentTime, endTime)
            }];
        }

        // Ordenar slots deshabilitados por hora de inicio
        dayDisabledSlots.sort((a, b) => a.start - b.start);

        // Generar slots disponibles entre los deshabilitados
        dayDisabledSlots.forEach((disabledSlot, i) => {
            const disabledStart = new Date(disabledSlot.start);
            const disabledEnd = new Date(disabledSlot.end);

            // Slot disponible antes del primer slot deshabilitado
            if (i === 0 && currentTime < disabledStart) {
                slots.push({
                    start: new Date(currentTime),
                    end: new Date(disabledStart),
                    available: true,
                    height: calculateSlotHeight(currentTime, disabledStart)
                });
            }

            // Slot deshabilitado
            slots.push({
                ...disabledSlot,
                available: false,
                height: calculateSlotHeight(disabledStart, disabledEnd)
            });

            currentTime = new Date(disabledEnd);

            // Slot disponible después del último slot deshabilitado
            if (i === dayDisabledSlots.length - 1 && currentTime < endTime) {
                slots.push({
                    start: new Date(currentTime),
                    end: new Date(endTime),
                    available: true,
                    height: calculateSlotHeight(currentTime, endTime)
                });
            }
        });

        return slots;
    };

    // Navegación entre semanas
    const changeWeek = (weeks) => {
        const newDate = new Date(currentDate);
        newDate.setDate(newDate.getDate() + weeks * 7);
        setCurrentDate(newDate);
    };

    // Manejar selección de slot
    const handleSlotClick = (slot) => {
        if (!slot.available) return;

        setSelectedSlot(slot);
        if (onSlotSelect) {
            onSlotSelect({
                start: slot.start,
                end: slot.end,
                date: slot.start
            });
        }
    };

    // Render
    const weekDays = getWeekDays();

    return (
        <div className="reservation-calendar">
            <div className="calendar-header">
                <button onClick={() => changeWeek(-1)}>&lt; Semana anterior</button>
                <h2>
                    {weekDays[0].toLocaleDateString()} - {weekDays[6].toLocaleDateString()}
                </h2>
                <button onClick={() => changeWeek(1)}>Siguiente semana &gt;</button>
            </div>

            <div className="calendar-grid">
                {/* Columna de horas */}
                <div className="time-column">
                    {Array.from({ length: config.availableHours.end - config.availableHours.start }).map((_, i) => (
                        <div key={i} className="time-label">
                            {config.availableHours.start + i}:00
                        </div>
                    ))}
                </div>

                {/* Columnas de días */}
                {weekDays.map((day, dayIndex) => {
                    const slots = generateAvailableSlots(day);

                    return (
                        <div key={dayIndex} className="day-column">
                            <div className="day-header">
                                {day.toLocaleDateString([], { weekday: 'short', day: 'numeric' })}
                            </div>

                            <div className="day-slots">
                                {slots.map((slot, slotIndex) => {
                                    const isSelected = selectedSlot &&
                                        slot.start.getTime() === selectedSlot.start.getTime() &&
                                        slot.end.getTime() === selectedSlot.end.getTime();

                                    return (
                                        <div
                                            key={slotIndex}
                                            className={`time-slot ${slot.available ? 'available' : 'disabled'} ${isSelected ? 'selected' : ''}`}
                                            style={{ height: `${slot.height}px` }}
                                            onClick={() => handleSlotClick(slot)}
                                        >
                                            <div className="slot-content">
                                                {slot.start.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} -
                                                {slot.end.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                                {!slot.available && slot.title && (
                                                    <div className="slot-title">{slot.title}</div>
                                                )}
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        </div>
                    );
                })}
            </div>

            {selectedSlot && (
                <div className="selection-info">
                    Seleccionado: {selectedSlot.start.toLocaleDateString()} {selectedSlot.start.toLocaleTimeString()} - {selectedSlot.end.toLocaleTimeString()}
                </div>
            )}
        </div>
    );
};

export default ReservationCalendar;
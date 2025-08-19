import React, { useState, useEffect } from 'react';
import TimePicker from 'react-time-picker';
import 'react-time-picker/dist/TimePicker.css';

export const Pagina2 = ({ turnos, setTurnos, estaciones, setEstaciones }) => {
    const [habilitado, setHabilitado] = useState({});
    const [turnosDisponibles, setTurnosDisponibles] = useState([]);

    useEffect(() => {
        const cargarTurnos = async () => {
            try {
                const response = await fetch('https://localhost:7210/api/Configuracion/turnos');
                if (!response.ok) throw new Error('Error al cargar los turnos');
                const data = await response.json();
                console.log(data)
                const turnosAjustados = data.map(t => ({
                    Nombre: t.nombre,
                    HoraInicio: t.horaInicio,
                    HoraFin: t.horaFin,
                }));

                const inicial = {};
                turnosAjustados.forEach(t => { inicial[t.Nombre] = false; });

                setTurnosDisponibles(turnosAjustados);
                setHabilitado(inicial);
            } catch (error) {
                console.error("Error cargando los turnos:", error);
            }
        };
        cargarTurnos();
    }, []);

    useEffect(() => {
        const seleccionados = turnosDisponibles
            .filter(t => habilitado[t.Nombre])
            .map(t => ({
                Nombre: t.Nombre,
                HoraInicio: t.HoraInicio,
                HoraFin: t.HoraFin
            }));

        setTurnos(seleccionados);
    }, [habilitado]);

    const toggleHabilitado = (nombre) => {
        setHabilitado(prev => ({ ...prev, [nombre]: !prev[nombre] }));
    };

    return (
        <div className="pag3">
            <h4>Configurar los días habilitados</h4>
            <label>Seleccione los turnos a habilitar</label>

            {turnosDisponibles.map(t => (
                <div key={t.Nombre} className="turno-row">
                    <div className="grupo">
                        <input
                            type="checkbox"
                            id={t.Nombre}
                            checked={habilitado[t.Nombre] || false}
                            onChange={() => toggleHabilitado(t.Nombre)}
                        />
                        <label htmlFor={t.Nombre}>{t.Nombre}</label>
                    </div>
                    <div className="time-range">
                        <TimePicker value={t.HoraInicio} disableClock format="HH:mm" disabled />
                        <span>-</span>
                        <TimePicker value={t.HoraFin} disableClock format="HH:mm" disabled />
                    </div>
                </div>
            ))}

            <div className="estaciones-duracion">
                <div className="input-group">
                    <label>Cantidad de estaciones</label>
                    <input
                        type="number"
                        value={estaciones}
                        onChange={(e) => setEstaciones(Number(e.target.value))}
                        min={1}
                    />
                </div>
            </div>
        </div>
    );
};
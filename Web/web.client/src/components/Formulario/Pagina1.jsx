import { Calendar } from "react-multi-date-picker";
import DatePanel from "react-multi-date-picker/plugins/date_panel";
import "../../estilos/pagina.css";

export const Pagina1 = ({ fechas, setFechas }) => {
    const handleChange = (dates) => {
        // Convertir cada fecha seleccionada a string YYYY-MM-DD
        const normalizadas = dates.map(d => d.format("YYYY-MM-DD"));
        setFechas(normalizadas);
    };

    const calendarStyles = {
        border: "1px solid #ccc",
        borderRadius: "8px",
        padding: "10px",
        width: "100%",
        height:"55vh"
,        backgroundColor: "#f9f9f9"
    };

    const datePanelStyles = {
        width: "50%", // corregido de "with"
        backgroundColor: "#fff",
        border: "1px solid #ddd",
        borderRadius: "8px",
        padding: "10px",
        fontSize: "14px",
        height:"50vh",
        color: "#333"
    };

    return (
        <div className="pagina1-container">
            <div className="pagina1-header">
                <h2>Planificar sesiones de atención</h2>
                <p className="pagina1-subtitle">Seleccione 1 o más días para habilitarlos</p>
            </div>

            <div className="calendar-wrapper">
                <Calendar
                    multiple
                    numberOfMonths={2}
                    value={fechas}
                    onChange={handleChange}
                    style={calendarStyles}
                    plugins={[
                        <DatePanel
                            key="panel"
                            position="right"
                            header="Fechas seleccionadas"
                            style={datePanelStyles}
                        />
                    ]}
                    minDate={new Date()}
                    weekDays={["Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb"]}
                />
            </div>
        </div>
    );
};

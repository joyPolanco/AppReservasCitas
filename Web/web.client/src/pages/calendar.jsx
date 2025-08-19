
import { useUser } from "../UserContext";
import CalendarAdmin from "../pages/CalendarAdmin";
import CalendarUser from "../pages/CalendarUser";

export default function Calendar() {
    const { usuario } = useUser();
    return (
        <div style={{ width: "100%", height:"90%" }}>
        { usuario.rol === 'admin' ?<CalendarAdmin /> : <CalendarUser / > }
        </div>
    );
}
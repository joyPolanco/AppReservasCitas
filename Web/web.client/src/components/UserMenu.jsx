import { Link } from 'react-router-dom';
import '../estilos/userLayout.css';
import { useNavigate } from 'react-router-dom';
import { useUser } from '../UserContext';

export const UserMenu = () => {
    const navigate = useNavigate();
    const { usuario } = useUser();

    const manejarLogout = () => {
        sessionStorage.removeItem("token");
        navigate('/login');
    };

    return (
        <nav className="topbar">
            {/* Logo y rol */}
            <div className="topbar-left">
                <div className="logo-cont"></div>
                <span className="rol-text">{usuario.correo}</span>
            </div>

            {/* Menú de navegación */}
            <ul className="topbar-menu">
                <li>
                    <Link to="/agenda" className="topbar-link">
                       Calendario  de disponibilidad
                    </Link>
                </li>
                <li>
                    <Link to="/calendario" className="topbar-link">
                        Mis reservas
                    </Link>
                </li>
            </ul>

            {/* Botón de logout */}
            <div className="topbar-right">
                <button className="btn-salir" onClick={manejarLogout}>
                    Cerrar sesión
                </button>
            </div>
        </nav>
    );
};

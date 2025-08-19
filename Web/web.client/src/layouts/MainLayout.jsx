import { Navbar } from '../components/Navbar';
import { Outlet } from 'react-router-dom';
import '../estilos/main.css';
import { useUser } from '../UserContext';

export const MainLayout = () => {
    const { usuario } = useUser();

    if (!usuario) return null; // No renderiza si no hay usuario

    return (
        <div className="app-container">
            <Navbar className="barra"  />
            <main className="main-content">
                <Outlet /> {/* Renderiza rutas hijas */}
            </main>
        </div>
    );
};

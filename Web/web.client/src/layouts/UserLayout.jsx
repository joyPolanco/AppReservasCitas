import { Navbar } from '../components/Navbar';
import { Outlet } from 'react-router-dom';
import { useUser } from '../UserContext';
import { UserMenu } from '../components/UserMenu';
import '../estilos/userLayout.css';


export const UserLayout = () => {
    const { usuario } = useUser();

    if (!usuario) return null; // No renderiza si no hay usuario

    return (
        <div className="user-app-container">
            <UserMenu  />
            <main className="user-main-content">
                <Outlet />
            </main>
        </div>

    );
};

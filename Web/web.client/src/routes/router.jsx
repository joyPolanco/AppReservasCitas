import { Routes, Route } from 'react-router-dom';
import { MainLayout } from '../layouts/MainLayout';
import { ProtectedRoute } from '../ProtectedRoute';

import Login from '../pages/Login';
import Home from '../pages/Home';
import Agendar from '../pages/Agendar';
import Calendar from '../pages/calendar';
import Actividades from '../pages/Actividades';
import Configuracion from '../pages/Configuracion';
import Registrar from '../pages/Registrar';

import RegistrarAdmin from '../pages/RegistrarAdmin';
import { useUser } from '../UserContext';
import { UserLayout } from '../layouts/UserLayout';
import CancelarCita from '../pages/CancelarCita';
import UsuariosView from '../pages/UsuariosView';
export function MyRoutes() {
    const { usuario } = useUser();

    // Selecciona layout según rol
    const Layout = usuario?.rol === 'admin' ? MainLayout : UserLayout;

    return (
        <Routes>
            <Route element={<Layout />}>
                <Route element={<ProtectedRoute allowedRoles={['admin']} />}>
                    <Route path="/actividades" element={<Actividades />} />
                    <Route path="/configuracion" element={<Configuracion />} />
                    <Route path="/usuarios" element={<UsuariosView />} />
                    <Route path="/registrar-admin" element={<RegistrarAdmin/>} />


                </Route>

                <Route element={<ProtectedRoute allowedRoles={['admin', 'usuario']} />}>
                    <Route path="/agenda" element={<Agendar />} />
                    <Route path="/calendario" element={<Calendar />} />
                </Route>
            </Route>

            <Route path="/login" element={<Login />} />
            <Route path="/" element={<Home />} />
            <Route path="/registrarse" element={<Registrar />} />
            <Route path="/cancelar" element={< CancelarCita />} />

        </Routes>
    );
}
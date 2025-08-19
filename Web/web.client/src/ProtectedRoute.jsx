import { Navigate, Outlet } from 'react-router-dom';
import { useUser } from './UserContext';

export function ProtectedRoute({ allowedRoles }) {
    const { usuario, loading } = useUser();

    if (loading) return null; // Espera a verificar autenticación

  if (!usuario) return <Navigate to="/login" replace />;

    if (allowedRoles && !allowedRoles.includes(usuario.rol)) {
        return <Navigate to="/" replace />;
    }

    return <Outlet />;
}

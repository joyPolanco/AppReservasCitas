import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';

const RoleRoute = ({ usuario, allowedRoles }) => {
    if (!usuario) {
        // Si no hay usuario, redirige a login
        return <Navigate to="/login" replace />;
    }

    if (!allowedRoles.includes(usuario.rol)) {
        // Si el usuario no tiene rol permitido, redirige o muestra error
        return <Navigate to="/" replace />;
    }

    // Usuario con rol permitido: renderiza las rutas hijas
    return <Outlet />;
};

export default RoleRoute;

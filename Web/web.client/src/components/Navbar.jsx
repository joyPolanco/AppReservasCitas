import React from 'react';
import { useUser } from '../UserContext';
import { AdminMenu } from './AdminMenu';
import { UserMenu } from './UserMenu';

export const Navbar = () => {
    const { usuario } = useUser();

    if (!usuario) return <div>No hay usuario</div>;

    return (
        <div className="navbar">
            {usuario.rol === 'admin' ? <AdminMenu  /> : <UserMenu  />}
        </div>
    );
};

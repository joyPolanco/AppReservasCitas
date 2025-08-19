import React, { createContext, useState, useEffect, useContext } from 'react';
import * as jwt_decode from 'jwt-decode';

const UserContext = createContext();

export function UserProvider({ children }) {
    // Cargar usuario desde sessionStorage al inicializar
    const [usuario, setUsuario] = useState(() => {
        const storedUser = sessionStorage.getItem("usuario");
        return storedUser ? JSON.parse(storedUser) : null;
    });
    const [loading, setLoading] = useState(true);

    const redirigirLogin = () => {
        setUsuario(null);
        setLoading(false);
        sessionStorage.removeItem('token');
        sessionStorage.removeItem('usuario');

        const publicPaths = ['/login', '/registrarse','/cancelar'];
        if (!publicPaths.includes(window.location.pathname)) {
            window.location.href = '/login';
        }
    };

    const verificarAutenticacion = async () => {
        const token = sessionStorage.getItem('token');

        if (!token) {
            redirigirLogin();
            return;
        }

        try {
            const decoded = jwt_decode.default(token);
            const now = Date.now() / 1000;

            if (decoded.exp < now) {
                redirigirLogin();
                return;
            }

            const res = await fetch('https://localhost:7210/api/Auth/me', {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + token
                }
            });
            if (res.status === 401) {
                redirigirLogin();
                return;
            }

            if (!res.ok) throw new Error('No autenticado');

            const userData = await res.json();
            console.log(userData)
            setUsuario(userData);
            sessionStorage.setItem("usuario", JSON.stringify(userData));
        } catch (error) {
            console.log(error);
            redirigirLogin();
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        verificarAutenticacion();
    }, []);

    return (
        <UserContext.Provider value={{ usuario, setUsuario, loading }}>
            {children}
        </UserContext.Provider>
    );
}

export function useUser() {
    const context = useContext(UserContext);
    if (!context) {
        throw new Error('useUser must be used within a UserProvider');
    }
    return context;
}

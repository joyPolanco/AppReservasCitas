import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../estilos/login.css';
import { useUser } from '../UserContext';

export default function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const { setUsuario } = useUser();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);

        try {
            const res = await fetch('https://localhost:7210/api/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ Email: email, Contrasena: password })
            });

            const data = await res.json();

            if (!res.ok) {
                if (res.status >= 500) {
                    throw new Error('El servidor tuvo un problema, inténtalo más tarde.');
                } else if (res.status === 400 || res.status === 401) {
                    throw new Error(data.error || data.message || 'Correo o contraseña incorrectos.');
                } else {
                    throw new Error('Ocurrió un error inesperado.');
                }
            }

            if (data.token) {
                sessionStorage.setItem('token', data.token);
            }

            if (data.user) {
                sessionStorage.setItem('usuario', JSON.stringify(data.user));
                setUsuario(data.user);
            }
            navigate('/agenda');
        } catch (err) {
            setError(err.message || 'Ocurrió un error al iniciar sesión');
        } finally {
            setIsLoading(false);
        }
    };


    const handleChangeEmail = (e) => {
        setEmail(e.target.value);
        if (error) setError('');
    };

    const handleChangePassword = (e) => {
        setPassword(e.target.value);
        if (error) setError('');
    };

    return (
        <div className="login-container">
            <form className="login-form" onSubmit={handleSubmit}>
                <h1 className="login-title">Iniciar Sesión</h1>

                {error && <p className="error-text">{error}</p>}

                <div className="form-group">
                    <label htmlFor="email">Correo electrónico</label>
                    <input
                        id="email"
                        type="email"
                        placeholder="tu@email.com"
                        value={email}
                        onChange={handleChangeEmail}
                        required
                        autoFocus
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="password">Contraseña</label>
                    <input
                        id="password"
                        type="password"
                        placeholder="Introduce tu contraseña"
                        value={password}
                        onChange={handleChangePassword}
                        required
                    />
                    <a href="/recuperar-contrasena" className="forgot-link">¿Olvidaste tu contraseña?</a>
                </div>

                <button
                    type="submit"
                    className={`btn-login ${isLoading ? 'loading' : ''}`}
                    disabled={isLoading}
                >
                    {isLoading ? '' : 'Iniciar sesión'}
                </button>

                <p className="signup-text">
                    ¿No tienes una cuenta? <a href="/registrarse">Regístrate</a>
                </p>
            </form>
        </div>
    );
}

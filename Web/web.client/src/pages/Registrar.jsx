import { useState, useEffect } from "react";
import "../estilos/Registrar.css";
import { useNavigate } from 'react-router-dom';

function Registrar() {
    const [formData, setFormData] = useState({
        nombre: "",
        correo: "",
        contrasena: ""
    });
    const [contrasenaConfirmar, setContrasenaConfirmar] = useState("");
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    // Limpiar mensajes automáticamente después de 5 segundos
    useEffect(() => {
        if (error || success) {
            const timer = setTimeout(() => {
                setError("");
                setSuccess("");
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, [error, success]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setError("");
        setSuccess("");

        if (name === "confirmPassword") {
            setContrasenaConfirmar(value);
        } else {
            setFormData((prev) => ({ ...prev, [name]: value }));
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (formData.contrasena !== contrasenaConfirmar) {
            setError("Las contraseñas no coinciden.");
            return;
        }

        setLoading(true); // Activar estado de carga

        try {
            const response = await fetch("https://localhost:7210/api/Usuarios/registrar", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formData),
            });
            const data = await response.json();

            if (response.ok) {
                setSuccess(data.message || "Usuario registrado exitosamente.");
                setFormData({ nombre: "", correo: "", contrasena: "" });
                setContrasenaConfirmar("");
                setTimeout(() => {
                    navigate('/login');
                }, 5000);

            } else {
                setError(data.error || "Error al registrar el usuario.");
            }
        } catch (err) {
            setError("Ocurrió un error al procesar la solicitud.");
        } finally {
            setLoading(false); // Desactivar estado de carga
        }
    };

    return (
        <div className="container">
            <div className="form-container">
                <h2 className="title">Registrarse</h2>
                <form onSubmit={handleSubmit} className="form">
                    <div className="form-group">
                        <label>Nombre</label>
                        <input
                            type="text"
                            name="nombre"
                            value={formData.nombre}
                            onChange={handleChange}
                            required
                            className="input"
                        />
                    </div>
                    <div className="form-group">
                        <label>Correo</label>
                        <input
                            type="email"
                            name="correo"
                            value={formData.correo}
                            onChange={handleChange}
                            required
                            className="input"
                        />
                    </div>
                    <div className="form-group">
                        <label>Contraseña</label>
                        <input
                            type="password"
                            name="contrasena"
                            value={formData.contrasena}
                            onChange={handleChange}
                            required
                            className="input"
                        />
                    </div>
                    <div className="form-group">
                        <label>Confirmar Contraseña</label>
                        <input
                            type="password"
                            name="confirmPassword"
                            value={contrasenaConfirmar}
                            onChange={handleChange}
                            required
                            className="input"
                        />
                    </div>

                    {error && <p className="error">{error}</p>}
                    {success && <p className="success">{success}</p>}

                    <button type="submit" className="submit-btn" disabled={loading}>
                        {loading ? "Registrando..." : "Registrar"}
                    </button>

                    <p className="redirect-text">
                        ¿Ya tienes una cuenta? <a href="/login">Inicia sesión</a>
                    </p>
                </form>
            </div>
            <div className="img-container"></div>
        </div>
    );
}

export default Registrar;

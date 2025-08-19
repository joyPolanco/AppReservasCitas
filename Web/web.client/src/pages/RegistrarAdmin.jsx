import { useState, useEffect } from "react";
import { useNavigate } from 'react-router-dom';

function RegistrarAdmin() {
    const [formData, setFormData] = useState({
        nombre: "",
        correo: "",
        contrasena: "AdminV6HJU7G"  // contraseña fija
    });
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const token = localStorage.getItem("token");

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
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await fetch("https://localhost:7210/api/Usuarios/registrar-admin", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(formData),
            });

            const data = await response.json();

            if (response.ok) {
                setSuccess(data.message || "Administrador registrado exitosamente.");
                setFormData({ nombre: "", correo: "", contrasena: "12345678" });
                setTimeout(() => navigate('/usuarios-admin'), 4000);
            } else {
                setError(data.error || "Error al registrar el administrador.");
            }
        } catch (err) {
            setError("Ocurrió un error al procesar la solicitud.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={styles.container}>
            <div style={styles.formContainer}>
                <h2 style={styles.title}>Registrar Administrador</h2>
                <form onSubmit={handleSubmit} style={styles.form}>
                    <div style={styles.formGroup}>
                        <label style={styles.label}>Nombre</label>
                        <input
                            type="text"
                            name="nombre"
                            value={formData.nombre}
                            onChange={handleChange}
                            required
                            style={styles.input}
                        />
                    </div>

                    <div style={styles.formGroup}>
                        <label style={styles.label}>Correo</label>
                        <input
                            type="email"
                            name="correo"
                            value={formData.correo}
                            onChange={handleChange}
                            required
                            style={styles.input}
                        />
                    </div>

                    <div style={styles.formGroup}>
                        <label style={styles.label}>Contraseña</label>
                        <input
                            type="text"
                            name="contrasena"
                            value={formData.contrasena}
                            readOnly
                            style={styles.input}
                        />
                    </div>

                    {error && <p style={styles.error}>{error}</p>}
                    {success && <p style={styles.success}>{success}</p>}

                    <button
                        type="submit"
                        disabled={loading}
                        style={loading ? styles.buttonDisabled : styles.button}
                    >
                        {loading ? "Registrando..." : "Registrar Administrador"}
                    </button>

                    <p style={styles.backLink}>
                        <a href="/usuarios" style={styles.link}>← Volver a lista de administradores</a>
                    </p>
                </form>
            </div>
        </div>
    );
}

const styles = {
    container: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        backgroundColor: '#f8fafc',
        width:'100%' ,
        fontFamily: '"Segoe UI", Tahoma, Geneva, Verdana, sans-serif',
        padding: '50px'
    },
    formContainer: {
        width: '100%',
        maxWidth: '100%',
        padding: '40px',
        borderRadius: '12px',
        backgroundColor: '#ffffff',
        boxShadow: '0 12px 30px rgba(0, 0, 0, 0.1)',
        border: '1px solid #e2e8f0'
    },
    title: {
        textAlign: 'center',
        fontSize: '2.2rem',
        fontWeight: '700',
        color: '#4f46e5',
        marginBottom: '30px'
    },
    form: {
        display: 'flex',
        flexDirection: 'column'
    },
    formGroup: {
        marginBottom: '20px'
    },
    label: {
        display: 'block',
        marginBottom: '8px',
        fontWeight: '600',
        color: '#1e293b',
        fontSize: '0.95rem'
    },
    input: {
        width: '100%',
        padding: '14px 16px',
        borderRadius: '8px',
        border: '1px solid #e2e8f0',
        outline: 'none',
        fontSize: '1rem',
        backgroundColor: '#f8fafc',
        color: '#334155',
        transition: 'all 0.3s ease'
    },
    inputFocus: {
        borderColor: '#4f46e5',
        boxShadow: '0 0 0 3px rgba(79, 70, 229, 0.2)'
    },
    error: {
        color: '#dc2626',
        marginBottom: '10px',
        fontWeight: '500',
        fontSize: '0.9rem',
        textAlign: 'center'
    },
    success: {
        color: '#16a34a',
        marginBottom: '10px',
        fontWeight: '500',
        fontSize: '0.9rem',
        textAlign: 'center'
    },
    button: {
        width: '100%',
        padding: '14px',
        marginTop: '10px',
        backgroundColor: '#4f46e5',
        color: '#fff',
        fontWeight: '600',
        border: 'none',
        borderRadius: '8px',
        cursor: 'pointer',
        fontSize: '1rem',
        transition: 'all 0.3s ease'
    },
    buttonDisabled: {
        width: '100%',
        padding: '14px',
        marginTop: '10px',
        backgroundColor: '#a5b4fc',
        color: '#fff',
        fontWeight: '600',
        border: 'none',
        borderRadius: '8px',
        cursor: 'not-allowed',
        fontSize: '1rem'
    },
    backLink: {
        textAlign: 'center',
        marginTop: '20px',
        fontSize: '0.95rem',
        color: '#64748b'
    },
    link: {
        color: '#4f46e5',
        fontWeight: '600',
        textDecoration: 'none',
        display: 'inline-flex',
        alignItems: 'center',
        gap: '5px'
    }
};

export default RegistrarAdmin;
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

function UsuariosView() {
    const [usuarios, setUsuarios] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    // Función que maneja el click del botón
    const handleRegistrarClick = () => {
        navigate("/registrar-admin");
    };

    // Llamada al endpoint real
    useEffect(() => {
        const fetchUsuarios = async () => {
            try {
                const endpoint = "https://localhost:7210/api/usuarios/usuarios-a";

               
                const token = sessionStorage.getItem("token");

                const response = await fetch(endpoint, {
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}` 
                    }
                });
               
                if (response.status === 404) {
                    setUsuarios([]);
                } else if (!response.ok) {
                    throw new Error("Error al obtener los usuarios");
                } else {
                    const data = await response.json();
                    setUsuarios(data);
                }
            } catch (err) {
                console.error(err);
                setError("No se pudieron cargar los usuarios");
            } finally {
                setLoading(false);
            }
        };

        fetchUsuarios();
    }, []);

    return (
        <div className="usuarios-container">
            <div className="cabecera">
                <h1>Usuarios administradores</h1>
                <div className="btn-cont">
                    <button className="btn-registrar" onClick={handleRegistrarClick}>
                        + Registrar usuario administrador

                    </button>
                </div>
            </div>

            <div className="tabla-usuarios">
                {loading ? (
                    <p style={{ textAlign: "center" }}>Cargando usuarios...</p>
                ) : error ? (
                    <p style={{ textAlign: "center", color: "red" }}>{error}</p>
                ) : (
                    <table>
                        <thead>
                            <tr>
                                <th>Nombre</th>
                                <th>Correo</th>
                            </tr>
                        </thead>
                        <tbody>
                            {usuarios.length > 0 ? (
                                usuarios.map((u) => (
                                    <tr key={u.id}>
                                        <td>{u.nombre}</td>
                                        <td>{u.correo}</td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan="3" style={{ textAlign: "center", padding: "30px", color: "#6b7280" }}>
                                        No hay usuarios registrados
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                )}
            </div>

            {/* Estilos */}
            <style>{`
        .usuarios-container {
          padding: 40px;
          font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
          background-color: #f9fafb;
          min-height: 100vh;
          width:100%;
        }

        .cabecera {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 30px;
        }

        h1 {
          margin: 0;
          font-size: 1.8rem;
          color: #111827;
        }

        .btn-cont {
          display: flex;
          justify-content: flex-end;
          width: auto;
        }

        .btn-registrar {
          background-color: #2563eb;
          color: white;
          border: none;
          padding: 12px 20px;
          border-radius: 8px;
          cursor: pointer;
          font-size: 0.95rem;
          font-weight: 500;
          box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
          transition: all 0.2s;
        }

        .btn-registrar:hover {
          background-color: #1d4ed8;
          transform: translateY(-2px);
          box-shadow: 0 6px 10px rgba(0, 0, 0, 0.15);
        }

        .tabla-usuarios table {
          width: 100%;
          border-collapse: collapse;
          background: white;
          box-shadow: 0 4px 12px rgba(0,0,0,0.08);
          border-radius: 10px;
          overflow: hidden;
        }

        .tabla-usuarios th, .tabla-usuarios td {
          padding: 16px 20px;
          text-align: left;
          border-bottom: 1px solid #e5e7eb;
          font-size: 0.95rem;
        }

        .tabla-usuarios th {
          background-color: #f3f4f6;
          font-weight: 600;
          color: #374151;
        }

        .tabla-usuarios tr:last-child td {
          border-bottom: none;
        }

        .tabla-usuarios tr:hover {
          background-color: #f9fafb;
        }
      `}</style>
        </div>
    );
}

export default UsuariosView;

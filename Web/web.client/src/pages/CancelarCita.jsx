import { useState } from "react";

export default function VerificarReserva() {
    const [loading, setLoading] = useState(false);
    const [mensaje, setMensaje] = useState("");

    const verificarReserva = async (accion) => {
        setLoading(true);
        setMensaje("");

        try {
            const params = new URLSearchParams(window.location.search);
            const token = params.get("token");

            if (!token) {
                setMensaje("⚠️ No se encontró el token en la URL.");
                return;
            }

            const endpoint =
                accion === "cancelar"
                    ? `https://localhost:7210/api/Reservas/cancelar-cita?token=${token}`
                    : `https://localhost:7210/api/Reservas/confirmar-cita?token=${token}`;

            const response = await fetch(endpoint, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
            });

            const data = await response.json();
            setMensaje(data.message);

        } catch (error) {
            setMensaje("❌ Error de conexión. Intenta nuevamente.");
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container">
            <h2>Gestión de Reserva</h2>
            <h1>Selecciona la acción que deseas realizar</h1>

            <div className="buttons">
                <button
                    className="confirm"
                    onClick={() => verificarReserva("confirmar")}
                    disabled={loading}
                >
                    {loading ? "Cargando..." : "✅ Confirmar reserva"}
                </button>
                <button
                    className="cancel"
                    onClick={() => verificarReserva("cancelar")}
                    disabled={loading}
                >
                    {loading ? "Cargando..." : "❌ Cancelar reserva"}
                </button>
            </div>

            {mensaje && <div className="mensaje">{mensaje}</div>}

            <style>{`
                .container {
                    display: flex;
                    flex-direction: column;
                    justify-content: center;
                    align-items: center;
                    min-height: 100vh;
                    background: #4b6cb7;
                    padding: 40px 20px;
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    color: #fff;
                    text-align: center;
                }
                h2 {
                    font-size: 48px;
                    margin-bottom: 10px;
                                        color:white;

                    font-weight: 700;
                    text-shadow: 1px 1px 6px rgba(0,0,0,0.3);
                }
                h1 {
                    font-size: 28px;
                    margin-bottom: 30px;
                    font-weight: 400;
                    text-shadow: 1px 1px 4px rgba(0,0,0,0.2);
                }
                .buttons {
                    display: flex;
                    gap: 20px;
                    flex-wrap: wrap;
                }
                button {
                    padding: 14px 28px;
                    font-size: 18px;
                    font-weight: 600;
                    border-radius: 8px;
                    border: none;
                    cursor: pointer;
                    transition: all 0.3s ease;
                    box-shadow: 0 4px 10px rgba(0,0,0,0.2);
                }
                button.confirm {
                    background: #28a745;
                    color: white;
                }
                button.confirm:hover:enabled {
                    background: #218838;
                    transform: translateY(-2px);
                }
                button.cancel {
                    background: #dc3545;
                    color: white;
                }
                button.cancel:hover:enabled {
                    background: #c82333;
                    transform: translateY(-2px);
                }
                button:disabled {
                    opacity: 0.6;
                    cursor: not-allowed;
                }
                .mensaje {
                    margin-top: 30px;
                    padding: 20px 25px;
                    background: rgba(255,255,255,0.9);
                    border-radius: 10px;
                    color: #333;
                    font-weight: 500;
                    max-width: 350px;
                    box-shadow: 0 6px 15px rgba(0,0,0,0.15);
                    transition: all 0.3s ease;
                }
            `}</style>
        </div>
    );
}

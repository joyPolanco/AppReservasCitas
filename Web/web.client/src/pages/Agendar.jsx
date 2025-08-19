import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Scrollbar, A11y } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/pagination';
import 'swiper/css/scrollbar';
import '../estilos/agenda.css';
import { Pagina1 } from '../components/Formulario/Pagina1';
import { Pagina2 } from '../components/Formulario/Pagina2';

import { useRef, useState } from "react";
import swal from 'sweetalert';
import { useUser } from '../UserContext';
import AgendaUsuario from '../pages/AgendaUsuario';

export default function Agendar() {
    const swiperRef = useRef(null);
    const { usuario } = useUser();

    if (usuario.rol !== "admin") {
        return <AgendaUsuario />;
    }

    const [formData, setFormData] = useState({
        Fechas: [],
        Turnos: [],
        Estaciones: 0,
        CorreoUsuario: usuario.correo
    });

    const irAdelante = () => {
        if (swiperRef.current?.swiper) {
            const swiper = swiperRef.current.swiper;
            if (swiper.activeIndex === 0 && formData.Fechas.length === 0) {
                swal({
                    title: "Datos incompletos",
                    text: "Seleccione al menos 1 día para continuar",
                    icon: "warning",
                });
                return;
            }
            swiper.slideNext();
        }
    };

    const irAtras = () => {
        swiperRef.current?.swiper.slidePrev();
    };

    const guardar = async () => {
        // Convertir correctamente las fechas a string ISO
        const converted = formData.Fechas.map(d => {
            const dateObj = d.toDate ? d.toDate() : d;
            return new Date(dateObj).toISOString().split("T")[0];
        });

        try {
            const data = {
                Fechas: converted,
                Turnos: [...formData.Turnos],
                Estaciones: formData.Estaciones,
                CorreoUsuario: formData.CorreoUsuario,
            };
            console.log("📤 Enviando:", data);

            const token = sessionStorage.getItem("token");

            const response = await fetch("https://localhost:7210/api/Planificacion/crear", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
                body: JSON.stringify(data),
            });

            let result = null;
            const text = await response.text();
            if (text) {
                result = JSON.parse(text);
            }
            console.log("📥 Respuesta:", result);

            if (response.ok) {
                swal("Éxito", result?.mensaje || "La planificación se creó correctamente.", "success");
            } else {
                swal("Error", result?.mensaje || "Ocurrió un error desconocido.", "error");
            }
        } catch (error) {
            swal("Error", error.message, "error");
        }
    };

    return (
        <div className="formslider">
            <Swiper
                ref={swiperRef}
                className="carrusel"
                modules={[Navigation, Scrollbar, A11y]}
                spaceBetween={30}
                slidesPerView={1}
                pagination={{ clickable: false }}
                scrollbar={{ draggable: false }}
            >
                <SwiperSlide>
                    <Pagina1
                        fechas={formData.Fechas}
                        setFechas={(fechas) =>
                            setFormData(prev => ({ ...prev, Fechas: [...fechas] }))
                        }
                    />
                    <div className="button-group">
                        <button type="button" onClick={irAtras} className="btn btn-back">
                            <i className="fas fa-arrow-left"></i> Atrás
                        </button>
                        <button type="button" onClick={irAdelante} className="btn btn-next">
                            Continuar <i className="fas fa-arrow-right"></i>
                        </button>
                    </div>
                </SwiperSlide>

                <SwiperSlide>
                    <Pagina2
                        turnos={formData.Turnos}
                        setTurnos={(turnos) =>
                            setFormData(prev => ({ ...prev, Turnos: [...turnos] }))
                        }
                        estaciones={formData.Estaciones}
                        setEstaciones={(estaciones) =>
                            setFormData(prev => ({ ...prev, Estaciones: estaciones }))
                        }
                    />
                    <div className="button-group">
                        <button type="button" onClick={irAtras} className="btn btn-back">
                            <i className="fas fa-arrow-left"></i> Atrás
                        </button>
                        <button type="button" onClick={guardar} className="btn btn-save">
                            <i className="fas fa-save"></i> Guardar
                        </button>
                    </div>
                </SwiperSlide>
            </Swiper>
        </div>
    );
}

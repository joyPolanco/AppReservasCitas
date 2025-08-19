import '../estilos/configuracion.css';
import { useState, useRef, useEffect } from 'react';
import { Swiper, SwiperSlide } from 'swiper/react';
import { Navigation, Scrollbar, A11y } from 'swiper/modules';
import 'swiper/css';
import 'swiper/css/navigation';
import 'swiper/css/scrollbar';
import Swal from 'sweetalert2';

export default function Configuracion() {
  const swiperRef = useRef(null);
  const configuracionGenerica = {
    cupoEstaciones: 1,
    duracionCita: 30,
    horarios: {
      matutino: { inicio: "05:00", fin: "11:59" },
      vespertino: { inicio: "12:00", fin: "17:59" },
      nocturno: { inicio: "18:00", fin: "23:59" }
    }
  };

  const [checked, setChecked] = useState(false);
  const [cupoEstaciones, setCupoEstaciones] = useState(configuracionGenerica.cupoEstaciones);
  const [duracionCita, setDuracionCita] = useState(configuracionGenerica.duracionCita);
  const [horarios, setHorarios] = useState(configuracionGenerica.horarios);
  const [turnosDisponibles, setTurnosDisponibles] = useState([]);
  const [habilitado, setHabilitado] = useState({});

  const rangos = {
    matutino: { min: 5, max: 11 },   // 05:00 - 11:59
    vespertino: { min: 12, max: 17 }, // 12:00 - 17:59
    nocturno: { min: 18, max: 23 }    // 18:00 - 23:59
  };

  // Función para formatear hora en 24H (HH:mm)
  const formatearHora = (valor) => {
    if (!valor) return "00:00";
    const [h, m] = valor.split(":");
    return `${h.padStart(2, "0")}:${m.padStart(2, "0")}`;
  };

  // Validar si una hora está en el rango correcto para su turno
  const validarRangoTurno = (hora, turno) => {
    const [horas] = hora.split(":").map(Number);
    return horas >= rangos[turno].min && horas <= rangos[turno].max;
  };

  useEffect(() => {
    const cargarConfiguracion = async () => {
      try {
        const response = await fetch('https://localhost:7210/api/Configuracion');
        if (!response.ok) throw new Error('Error al cargar la configuración');
        
        const data = await response.json();
        
        // Mapear turnos recibidos asegurando formato 24H
        const turnosAjustados = data.turnos?.map(t => ({
          Nombre: t.nombre,
          HoraInicio: formatearHora(t.horaInicio),
          HoraFin: formatearHora(t.horaFin),
        })) || [];

        const inicial = {};
        turnosAjustados.forEach(t => {
          inicial[t.Nombre] = false;
        });

        setTurnosDisponibles(turnosAjustados);
        setCupoEstaciones(data.cupoEstaciones);
        setChecked(data.cupoEstaciones > 1);
        setDuracionCita(data.duracionCitas);

        if (turnosAjustados.length > 0) {
          const horariosDb = { ...configuracionGenerica.horarios };
          turnosAjustados.forEach(t => {
            const key = t.Nombre.toLowerCase();
            if (horariosDb[key]) {
              horariosDb[key] = {
                inicio: formatearHora(t.HoraInicio),
                fin: formatearHora(t.HoraFin)
              };
            }
          });
          setHorarios(horariosDb);
        } else {
          setHorarios(configuracionGenerica.horarios);
        }

        setHabilitado(inicial);
      } catch (error) {
        console.error("Error cargando la configuración:", error);
        setCupoEstaciones(configuracionGenerica.cupoEstaciones);
        setChecked(false);
        setDuracionCita(configuracionGenerica.duracionCita);
        setHorarios(configuracionGenerica.horarios);
      }
    };
    cargarConfiguracion();
  }, []);

  const manejarCheck = (e) => {
    setChecked(e.target.checked);
    if (!e.target.checked) setCupoEstaciones(1);
  };

  const manejarRefrescar = () => {
    setCupoEstaciones(configuracionGenerica.cupoEstaciones);
    setChecked(false);
    setDuracionCita(configuracionGenerica.duracionCita);
    setHorarios(configuracionGenerica.horarios);
  };

  const actualizarHorario = (turno, campo, valor) => {
    const valorFormateado = formatearHora(valor);
    
    // Validar formato 24H
    const [hora, minutos] = valorFormateado.split(":").map(Number);
    if (hora < 0 || hora > 23 || minutos < 0 || minutos > 59) {
      Swal.fire({
        icon: 'error',
        title: 'Hora inválida',
        text: 'La hora debe estar entre 00:00 y 23:59.',
        timer: 2500,
        showConfirmButton: false
      });
      return;
    }

    // Validar rango del turno
    if (!validarRangoTurno(valorFormateado, turno)) {
      Swal.fire({
        icon: 'error',
        title: 'Horario fuera de rango',
        text: `El turno ${turno} debe estar entre ${rangos[turno].min}:00 y ${rangos[turno].max}:59.`,
        timer: 2500,
        showConfirmButton: false
      });
      return;
    }

    const nuevoHorario = { ...horarios[turno], [campo]: valorFormateado };
    const inicio = new Date(`1970-01-01T${nuevoHorario.inicio}`);
    const fin = new Date(`1970-01-01T${nuevoHorario.fin}`);
    const diff = (fin - inicio) / 60000; // diferencia en minutos

    if (diff < duracionCita) {
      Swal.fire({
        icon: 'error',
        title: 'Horario inválido',
        text: `El turno ${turno} debe durar al menos ${duracionCita} minutos.`,
        timer: 2500,
        showConfirmButton: false
      });
      return;
    }

    if (inicio >= fin) {
      Swal.fire({
        icon: 'error',
        title: 'Rango inválido',
        text: `La hora de inicio no puede ser mayor o igual a la hora de fin en el turno ${turno}.`,
        timer: 2500,
        showConfirmButton: false
      });
      return;
    }

    setHorarios({ ...horarios, [turno]: nuevoHorario });
  };

  const irAtras = () => swiperRef.current?.swiper.slidePrev();
  const irAdelante = () => swiperRef.current?.swiper.slideNext();

  const manejarGuardar = async (e) => {
    e.preventDefault();
    
    // Preparar datos asegurando formato 24H
    const turnos = Object.entries(horarios).map(([nombre, { inicio, fin }]) => ({
      Nombre: nombre,
      HoraInicio: formatearHora(inicio),
      HoraFin: formatearHora(fin)
    }));

    const configData = {
      cupoEstaciones: checked ? Number(cupoEstaciones) : 1,
      turnos: turnos,
      duracionCitas: duracionCita,
    };

    try {
      const response = await fetch("https://localhost:7210/api/Configuracion", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(configData)
      });

      if (!response.ok) throw new Error("Error en el servidor");
      
      await response.json();
      Swal.fire({
        icon: 'success',
        title: 'Configuración guardada',
        text: 'Se guardó correctamente.',
        timer: 2000,
        showConfirmButton: false
      });
      manejarRefrescar();
    } catch (error) {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: 'No se pudo guardar la configuración.',
        timer: 2500,
        showConfirmButton: false
      });
      console.error("Error al guardar configuración:", error);
    }
  };

  return (
    <div className="configuracion">
      <div className="configuracion-form">
        <h2>Configuración de Agenda</h2>
        <form className="form-config" onSubmit={manejarGuardar}>
          <Swiper 
            ref={swiperRef} 
            modules={[Navigation, Scrollbar, A11y]} 
            spaceBetween={30} 
            slidesPerView={1} 
            scrollbar={{ draggable: true }}
          >
            {/* Primera slide - Configuración básica */}
            <SwiperSlide>
              <div className="form-section">
                <h3>Estaciones y Citas</h3>
                <div className="section-conf">
                  <input 
                    type="checkbox" 
                    checked={checked} 
                    onChange={manejarCheck} 
                  />
                  <label>Activar cupo personalizado por estación</label>
                </div>
                <div className="section">
                  <label>Cupo por estación</label>
                  <input 
                    type="number" 
                    value={cupoEstaciones} 
                    onChange={e => setCupoEstaciones(Number(e.target.value))} 
                    min={1} 
                    disabled={!checked} 
                    style={{ 
                      backgroundColor: checked ? "#fafbfc" : "rgba(0,0,0,0.1)", 
                      color: checked ? "#0c69f5" : "#898c8f" 
                    }} 
                  />
                </div>
                <div className="section">
                  <label>Duración de la cita (minutos)</label>
                  <input 
                    type="number" 
                    value={duracionCita} 
                    onChange={e => setDuracionCita(Number(e.target.value))} 
                    min={1} 
                  />
                </div>
                <br /><br />
                <div className="form-buttons">
                  <button type="button" onClick={manejarRefrescar} className="btn-refresh">
                    Refrescar
                  </button>
                  <button type="button" onClick={irAdelante} className="btn-refresh">
                    Continuar
                  </button>
                </div>
              </div>
            </SwiperSlide>

            {/* Segunda slide - Configuración de turnos */}
            <SwiperSlide>
              <h3>Turnos</h3>
              {["matutino", "vespertino", "nocturno"].map(turno => (
                <div key={turno} className="turno-card">
                  <h3>{turno.charAt(0).toUpperCase() + turno.slice(1)}</h3>
                  <div className="time-inputs">
                    <input 
                      type="time" 
                      value={horarios[turno].inicio} 
                      onChange={e => actualizarHorario(turno, "inicio", e.target.value)} 
                      step="300" // Incrementos de 5 minutos
                    />
                    <span>-</span>
                    <input 
                      type="time" 
                      value={horarios[turno].fin} 
                      onChange={e => actualizarHorario(turno, "fin", e.target.value)} 
                      step="300" // Incrementos de 5 minutos
                    />
                  </div>
                  <p className="rango-turno">
                  </p>
                </div>
              ))}
              <div className="form-buttons">
                <button type="button" onClick={irAtras} className="btn-refresh">
                  Atrás
                </button>
                <button type="submit" className="btn-refresh">
                  Guardar
                </button>
              </div>
            </SwiperSlide>
          </Swiper>
        </form>
      </div>
    </div>
  );
}
import '../estilos/navbar.css';
import { Link } from 'react-router-dom';
import { Sidebar, Menu, MenuItem } from 'react-pro-sidebar';
import { useUser } from '../UserContext';
import { useNavigate } from 'react-router-dom';
 

export const AdminMenu = () => {
    const navigate = useNavigate();
    const { usuario } = useUser();
    const manejarLogout = () => {
        sessionStorage.removeItem("token");
        navigate('/login');

    };
    return (
        <Sidebar className="barra-estilo"
            backgroundColor="transparent"
            width="80%"
            rootStyles={{
                height: "100%",
                width: "250px",   
                background: "linear-gradient(180deg, #344cb7, #3468d0)",
                boxShadow: "3px 0 20px rgba(0,0,0,0.6)",
                borderRight: "1px solid rgba(255,255,255,0.2)",
                color: "snow",
                fontFamily: "'Segoe UI', sans-serif"

            }}
        >
            {/* Encabezado */}
            <div style={{
                padding: "24px 16px",
                textAlign: "center",
                borderBottom: "1px solid rgba(255,255,255,0.3)",
            }}>
                <h2 style={{
                    fontSize: "1.5rem",
                    fontWeight: "700",
                    color: "white",
                    marginBottom: "8px",
                    textShadow: "0 1px 3px rgba(0,0,0,0.1)"
                }}>
                    <div className="logo-cont"></div>
                   

                </h2>
                <div style={{
                    backgroundColor: "rgba(255, 255, 255, 0.15)",
                    padding: "6px 12px",
                    borderRadius: "8px",
                    fontSize: "1rem",
                    color: "white",
                    backdropFilter: "blur(4px)"
                }}>
                    {usuario.correo }

                </div>
            </div>

            {/* Menú */}
            <Menu
                menuItemStyles={{
                    button: {
                        padding: "20px 20px",
                        width:"90%",
                        borderRadius: "12px",
                        margin: "8px 12px",
                        fontSize: "1.05rem",
                        fontWeight: "500",
                        height: "32%",
                        backgroundColor: "rgba(255,255,255,0.08)",
                        color: "white",
                        backdropFilter: "blur(6px)",
                        transition: "all 0.3s ease",
                        "&:hover": {
                            transform: "translateX(5px) scale(1.02)",
                            boxShadow: "0 4px 20px rgba(0,0,0,0.15)",
                            backgroundColor: "white",
                            color: "#374151",
                        }
                    },
                }}
            >
                <MenuItem className="menuItem" component={<Link to="/agenda" />}>
                    Agenda
                </MenuItem>

                <MenuItem className="menuItem" component={<Link to="/calendario" />}>
                  Turnos habilitados                     
                </MenuItem>

                <MenuItem className="menuItem" component={<Link to="/usuarios" />}>
                   Usuarios admin
                </MenuItem>

                
                <MenuItem className="menuItem"  component={<Link to="/configuracion" />}>
                    Configuración
                </MenuItem>

            
                <div className="btn-logout">
                    <button className="btn-salir" onClick={manejarLogout }>
                        Cerrar sesión
                    </button>
                     

                    </div>
             
            </Menu>
          
        </Sidebar>
    );
};

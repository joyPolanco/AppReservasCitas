using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Infraestructura.Persistencia.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController: ControllerBase
    {
        private readonly IUsuarioService _servicio;
        public UsuariosController(IUsuarioService servicio)
        {
            _servicio = servicio;
        }

        [HttpGet("verify")]
        public async Task<IActionResult> GetUser([FromQuery] string q)
        {
            try
            {
                var user = await _servicio.GetUsuarioByEmail(q);
                var userLoginData = new UsuarioLoginDTO() { Correo = user.Email, Rol = user.Rol };
                return Ok(userLoginData); // Devuelve el objeto completo

            }
            catch (ArgumentException)
            {
                return NotFound(new { message = "User not found" });

            }
            catch (Exception ex)
            {
                LoggerContext.GetLogger().SetFileLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.UtcNow,
                    Tipo = "Error",
                    Mensaje = ex.Message
                });

                return StatusCode(StatusCodes.Status500InternalServerError,
                            new { error = "Ocurrió un error inesperado.", detalle = ex.Message });
            }
        }
    

       
            [HttpPost("registrar")]
            public async Task<IActionResult> Registrar([FromBody] UsuarioCreateDTO usuario)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    await _servicio.Add(usuario);
                    return Ok(new { message = "Usuario registrado exitosamente" });
                }
                catch (ArgumentException ex) // Ejemplo de error de validación
                {
                    return BadRequest(new { error = ex.Message });
                }
                catch (InvalidOperationException ex) 
                {
                    return Conflict(new { error = ex.Message });
                }
                catch (Exception ex) 
                {
                LoggerContext.GetLogger().SetFileLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.UtcNow,
                    Tipo = "Error",
                    Mensaje = ex.Message
                });
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new { error = "Ocurrió un error inesperado.", detalle = ex.Message });
                }
            }

            [HttpPost("registrar-admin")]
            public async Task<IActionResult> RegistrarAdmin([FromBody] UsuarioCreateDTO usuario)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    await _servicio.AddAdmin(usuario);
                    return Ok(new { message = "Usuario registrado exitosamente" });
                }
                catch (ArgumentException ex) // Ejemplo de error de validación
                {
                    return BadRequest(new { error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Conflict(new { error = ex.Message });
                }
                catch (Exception ex)
            {
                LoggerContext.GetLogger().SetFileLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                        FechaHora = DateTime.UtcNow,
                        Tipo = "Error",
                        Mensaje = ex.Message
                    });
                    return StatusCode(StatusCodes.Status500InternalServerError,
                            new { error = "Ocurrió un error inesperado.", detalle = ex.Message });
                }
            }

        [HttpGet("usuarios-a")]
        public async Task<ActionResult<List<AdminViewDTO>>> GetUsuarios()
        {
            var usuarios = await _servicio.GetUsuariosAdmin();

            if (usuarios == null || usuarios.Count == 0)
                return NotFound("No se encontraron usuarios administradores.");

            return Ok(usuarios);
        }
    }
}

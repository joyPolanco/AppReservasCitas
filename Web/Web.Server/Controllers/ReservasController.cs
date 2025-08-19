using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Loggers;
using Infraestructura.Persistencia.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;
        private readonly IEmailService _emailService;
        private readonly IRepositorio<Log> _repoLogs;

        public ReservasController(IReservaService reservaService, IEmailService emailService, IRepositorio<Log> repositorio)
        {
            _reservaService = reservaService;
            _emailService = emailService;
            _repoLogs = repositorio;
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<IActionResult> CrearReserva([FromBody] ReservaPostDTO data)
        {
            if (data == null)
                return BadRequest(new { error = "Datos de reserva no pueden estar vacíos." });

            try
            {
                // Crear la reserva
                var emailData = await _reservaService.Reservar(data);

                // Enviar correo en segundo plano
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.EnviarCorreoAsync(emailData.destinatario, emailData.Asunto, emailData.Mensaje);
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
                    }
                });

                // Registrar evento de creación
                LoggerContext.GetLogger().SetDatabaseLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.UtcNow,
                    Tipo = "Creacion",
                    Mensaje = "Reserva creada correctamente.",
                    UsuarioId=emailData.idUser
                });

                return Ok(new { mensaje = "Reserva creada exitosamente." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
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
                    new { error = "Ocurrió un error inesperado. Intente más tarde." });
            }
        }

        [HttpPost("cancelar-cita")]
        public async Task<IActionResult> CancelarReserva([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "No se pudo cancelar la cita" });

            try
            {
                var correo = await _reservaService.CancelarReserva(token);

              LoggerContext.GetLogger().SetDatabaseLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.UtcNow,
                    Tipo = "Modificación",
                    Mensaje = $"El usuario de email:{correo} canceló su reserva"
                });

                return Ok(new { message = "Tu reserva ha sido cancelada correctamente." });
            }
            catch (ArgumentException e)
            {
                return NotFound(new { message = e.Message });
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
                    new { error = "Ocurrió un error inesperado en el servidor" });
            }
        }

        [HttpGet("reservas-usuario")]
        public async Task<IActionResult> ObtenerReservasPorUsuario([FromQuery] string correo)
        {
            if (string.IsNullOrEmpty(correo))
                return BadRequest("Debe proporcionar un correo válido.");

            try
            {
                var reservas = await _reservaService.ObtenerReservasUsuario(correo);
                return Ok(reservas);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
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
                return StatusCode(500, new { message = "Ocurrió un error en el servidor" });
            }
        }

        [HttpPost("confirmar-cita")]
        public async Task<IActionResult> ConfirmarCita([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "No se pudo confirmar la cita" });

            try
            {
                var usuario = await _reservaService.ConfirmarReserva(token);


                LoggerContext.GetLogger().SetFileLogger();
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.UtcNow,
                    Tipo = "Modificación",
                    Mensaje = $"El usuario de email:{usuario.Email} confirmó su reserva",
                    UsuarioId = usuario.Id

                });
                

                return Ok(new { message = "Tu reserva ha sido confirmada correctamente." });
            }
            catch (ArgumentException e)
            {
                return NotFound(new { message = e.Message });
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
                    new { error = "Ocurrió un error inesperado en el servidor" });
            }
        }
    }
}

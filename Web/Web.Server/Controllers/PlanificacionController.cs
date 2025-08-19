using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Server.Controllers
{
    using Aplicacion.DTOs;
    using Aplicacion.DTOs.Planificacion;
    using Aplicacion.Interfaces;
    using Dominio.Entidades;
    using Dominio.Entidades.Models;
    using Infraestructura.Persistencia.Loggers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    namespace WebAPI.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class PlanificacionController : ControllerBase
        {
            private readonly IPlanificacionService _planificacionService;

            public PlanificacionController(IPlanificacionService planificacionService)
            {
                _planificacionService = planificacionService;
            }

            [Authorize]
            [HttpPost("crear")]
            public async Task<IActionResult> CrearPlanificacion([FromBody] DatosPlanificacion data)
            {
                var logger = LoggerContext.GetLogger; // Singleton

                if (data == null || data.Turnos == null || data.Turnos.Count == 0 || data.Estaciones <= 0)
                {
                 
                    return BadRequest(new { mensaje = "Datos de planificación incompletos o inválidos." });
                }

                try
                {
                    var usuario = await  _planificacionService.CrearPlanificacion(data);

                    LoggerContext.GetLogger().SetDatabaseLogger();
                    await LoggerContext.GetLogger().Registrar(new Log
                    {
                        FechaHora = DateTime.UtcNow,
                        Tipo = "Creación",
                        Mensaje = $"Planificación creada correctamente por {usuario.Email}",
                        UsuarioId = usuario.Id
                    });

                    return Ok(new { mensaje = "Planificación creada correctamente." });
                }
                catch (InvalidDataException e)
                {


                    return BadRequest(new { mensaje = e.Message });
                }
                catch (ArgumentException ex)
                {


                    return BadRequest(new { mensaje = ex.Message });
                }
                catch (Exception ex)
                {
                    LoggerContext.GetLogger().SetFileLogger(); //
                    await LoggerContext.GetLogger().Registrar(new Log
                    {
                        FechaHora = DateTime.UtcNow,
                        Tipo = "Error",
                        Mensaje = ex.Message
                    });

                    return StatusCode(500, new { mensaje = "Ocurrió un error al crear la planificación. ", detalle = ex.Message });
                }
            }
            [HttpGet("getSlots")]
            public async Task<ActionResult<List<SlotDTO>>> GetSlots()
            {
                var logger = LoggerContext.GetLogger;

                try
                {
                    var slots = await _planificacionService.ObtenerSlotsDisponibles();

                 
                    return Ok(slots);
                }
                catch (InvalidDataException ex)
                {
                    

                    return NotFound(ex.Message);
                }
                catch (Exception ex)
                {
                    LoggerContext.GetLogger().SetFileLogger(); //
                    await LoggerContext.GetLogger().Registrar(new Log
                    {
                        FechaHora = DateTime.UtcNow,
                        Tipo = "Error",
                        Mensaje = ex.Message
                    });

                    return StatusCode(500, new { mensaje = "Ocurrió un error interno en el servidor." });
                }
            }
            [HttpGet("getTurnosActivos")]
            public async Task<ActionResult<List<DiaHabilitadoDTO>>> getTurnosActivos()
            {
                var logger = LoggerContext.GetLogger;

                try
                {
                    var diasHabilitados = await _planificacionService.ObtenerActivos();
                    return Ok(diasHabilitados);
                }
                catch (InvalidDataException ex)
                {
                    return NotFound(new
                    {
                        mensaje = "No se encontraron días habilitados.",
                        detalles = ex.Message
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return NotFound(new
                    {
                        mensaje = "Operación inválida.",
                        detalles = ex.Message
                    });
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

                    return StatusCode(500, new
                    {
                        mensaje = "Ocurrió un error interno en el servidor.",
                        detalles = ex.Message
                    });
                }
            }



        }
    }
}



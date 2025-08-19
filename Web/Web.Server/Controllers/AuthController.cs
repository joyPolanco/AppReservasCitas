using Aplicacion.Casosdeuso;
using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Aplicacion.Interfaces;
using Aplicacion.Servicios;
using Infraestructura.Persistencia;
using Infraestructura.Persistencia.Loggers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Dominio.Entidades;
using Dominio.Entidades.Models;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly JwtConfiguration _jwtOptions;
        public AuthController(
            IAuthService authService,
            IUsuarioService usuarioService,
            IJwtTokenGenerator jwtTokenGenerator,
            JwtConfiguration jwtConfiguration)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _jwtOptions = jwtConfiguration;
          
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest usuario)
        {
            if (usuario == null || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Contrasena))
            {
                return BadRequest(new { message = "Correo y contraseña son requeridos." });
            }

            try
            {
                var userDto = await _usuarioService.GetUsuarioByEmail(usuario.Email);

                if (userDto == null)
                {
                    return BadRequest(new { message = "No existe un usuario con ese correo." });
                }

                var valido = _authService.ValidarDatos(usuario, userDto.ContrasenaHasheada);

                if (!valido)
                {
                    return Unauthorized(new { message = "Datos inválidos. Contraseña o correo incorrectos." });
                }

                var accessToken = _jwtTokenGenerator.GenerateAccessToken(userDto.Email, userDto.Rol);

                return Ok(new
                {
                    token = accessToken,
                    user = new UsuarioLoginDTO
                    {
                        Correo = userDto.Email,
                        Rol = userDto.Rol
                    }
                });
            }
            catch (ArgumentException ex)
            {
                LoggerContext.GetLogger().SetFileLogger();

                // Registrar error en FileLogger
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.Now,
                    Tipo = "ArgumentException",
                    Mensaje = ex.Message
                });

                return BadRequest(new { message = "Correo o contraseña incorrecta" });
            }
            catch (Exception ex)
            {
                await LoggerContext.GetLogger().Registrar(new Log
                {
                    FechaHora = DateTime.Now,
                    Tipo = "Exception",
                    Mensaje = ex.Message
                });

                return StatusCode(500, new { message = "Ocurrió un error inesperado. Intente nuevamente más tarde." });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var claims = User.Claims.ToList();

         
            return Ok(new
            {
                Email = email,
                Role = role
            });
        }
    }
}

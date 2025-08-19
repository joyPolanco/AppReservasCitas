using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Aplicacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entidades;
using System.Linq.Expressions;
using Dominio.Entidades.Models;

namespace Aplicacion.Servicios
{
    public class ConfiguracionService : IConfiguracionService
    {
        private readonly IRepositorio<AppConfiguracion> _repo;

        public ConfiguracionService(IRepositorio<AppConfiguracion> repo)
        {
            _repo = repo;
        }

        public async Task Actualizar(AppConfiguracionDTO dto)
        {
            // Validar datos generales
           

            if (dto.CupoEstaciones < 0)
                throw new ArgumentException("El cupo de estaciones no puede ser menor a 0");

            // Validar y convertir turnos
            var turnosValidados = dto.Turnos.Select(t =>
            {
                if (!TimeSpan.TryParse(t.HoraInicio, out var inicio))
                    throw new ArgumentException($"Horario de inicio del turno {t.Nombre} en formato incorrecto");

                if (!TimeSpan.TryParse(t.HoraFin, out var fin))
                    throw new ArgumentException($"Horario de fin del turno {t.Nombre} en formato incorrecto");

                if (inicio >= fin)
                    throw new ArgumentException($"El horario de {t.Nombre} debe tener un fin posterior al inicio");

                return new { t.Nombre, Inicio = inicio, Fin = fin };
            }).ToList();

            // Obtener configuración existente o crear nueva
            var configuracion = (await _repo.GetAll()).FirstOrDefault() ?? new AppConfiguracion();

            // Asignar valores generales
            configuracion.DuracionCitas = dto.DuracionCitas;
            configuracion.CupoEstaciones = dto.CupoEstaciones;

            // Asignar horarios de turnos
            foreach (var turno in turnosValidados)
            {
                switch (turno.Nombre.ToLower())
                {
                    case "matutino":
                        configuracion.HoraInicioMatutino = TimeOnly.FromTimeSpan(turno.Inicio);
                        configuracion.HoraFinMatutino = TimeOnly.FromTimeSpan(turno.Fin);
                        break;
                    case "vespertino":
                        configuracion.HoraInicioVespertino = TimeOnly.FromTimeSpan(turno.Inicio);
                        configuracion.HoraFinVespertino = TimeOnly.FromTimeSpan(turno.Fin);
                        break;
                    case "nocturno":
                        configuracion.HoraInicioNocturno = TimeOnly.FromTimeSpan(turno.Inicio);
                        configuracion.HoraFinNocturno = TimeOnly.FromTimeSpan(turno.Fin);
                        break;
                    default:
                        throw new ArgumentException($"Turno {turno.Nombre} no reconocido");
                }
            }


            try
            {
                // Guardar cambios
                if (configuracion.Id == 0)
                {
                    await _repo.Create(configuracion);
                }
                else
                {
                    await _repo.Update(configuracion);
                }

                await _repo.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<AppConfiguracionDTO> Get()
        {

            var configFromRepo = await _repo.Get(1);
            

                if (configFromRepo == null)
                {
                    configFromRepo = new AppConfiguracion
                    {
                        CupoEstaciones = 10,
                        DuracionCitas = 30,
                        HoraInicioMatutino = TimeOnly.Parse("08:00"),
                        HoraFinMatutino = TimeOnly.Parse("12:00"),
                        HoraInicioVespertino = TimeOnly.Parse("13:00"),
                        HoraFinVespertino = TimeOnly.Parse("17:00"),
                        HoraInicioNocturno = TimeOnly.Parse("18:00"),
                        HoraFinNocturno = TimeOnly.Parse("21:00")
                    };

                    // Guardar la configuración en la base de datos
                    await _repo.Create(configFromRepo);
                    await _repo.SaveChangesAsync();
                }

            
                var configuracionDto = new AppConfiguracionDTO()
                {
                    CupoEstaciones = configFromRepo.CupoEstaciones,
                    DuracionCitas = configFromRepo.DuracionCitas,
                    Turnos = new List<TurnoDTO>()
                    {
                        new TurnoDTO
                        {
                            Nombre = "matutino",
                            HoraInicio = configFromRepo.HoraInicioMatutino.ToString(@"HH\:mm"),
                            HoraFin = configFromRepo.HoraFinMatutino.ToString(@"HH\:mm")
                        },
                        new TurnoDTO
                        {
                            Nombre = "vespertino",
                            HoraInicio = configFromRepo.HoraInicioVespertino.ToString(@"hh\:mm"),
                            HoraFin = configFromRepo.HoraFinVespertino.ToString(@"HH\:mm")
                        },
                        new TurnoDTO
                        {
                            Nombre = "nocturno",
                            HoraInicio = configFromRepo.HoraInicioNocturno.ToString(@"HH\:mm"),
                            HoraFin = configFromRepo.HoraFinNocturno.ToString(@"HH\:mm")
                        }
                      }
                };


                return configuracionDto;

            }
        

        public async Task<List<TurnoDTO>> GetTurnoDTOs()
        {
            var configuracion = (await _repo.GetAll()).FirstOrDefault();
            if (configuracion == null)
                throw new Exception("No se encontró la configuración");

            return new List<TurnoDTO>
            {
                new TurnoDTO
                {
                    Nombre = "matutino",
                    HoraInicio = configuracion.HoraInicioMatutino.ToString(@"HH\:mm"),
                    HoraFin = configuracion.HoraFinMatutino.ToString(@"HH\:mm")
                },
                new TurnoDTO
                {
                    Nombre = "vespertino",
                    HoraInicio = configuracion.HoraInicioVespertino.ToString(@"HH\:mm"),
                    HoraFin = configuracion.HoraFinVespertino.ToString(@"HH\:mm")
                },
                new TurnoDTO
                {
                    Nombre = "nocturno",
                    HoraInicio = configuracion.HoraInicioNocturno.ToString(@"HH\:mm"),
                    HoraFin = configuracion.HoraFinNocturno.ToString(@"HH\:mm")
                }
            };
        }
    }
}

using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Threading.Tasks;


namespace Aplicacion.Servicios
{
    public class PlanificacionService : IPlanificacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositorio<AppConfiguracion> _repositorioConfiguracion;

        public PlanificacionService(
            IUnitOfWork unitOfWork,
            IRepositorio<AppConfiguracion> repoConfig
            )
        {
            _unitOfWork = unitOfWork;
            _repositorioConfiguracion = repoConfig;

        }

        public async Task <Usuario>CrearPlanificacion(DatosPlanificacion data)
        {
            if (data == null) throw new InvalidDataException(nameof(data));
            if (string.IsNullOrWhiteSpace(data.CorreoUsuario))
                throw new InvalidDataException("El correo del usuario es requerido.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var usuario = await _unitOfWork.UsuarioRepository.GetUsuarioByEmail(data.CorreoUsuario);
                if (usuario == null) throw new InvalidDataException("No se encontró un usuario con ese correo.");

                var config = await _repositorioConfiguracion.Get(1);
                if (config == null) throw new InvalidDataException("No se encontró configuración de la aplicación.");

                var planificacion = new Planificacion
                {
                    FechaRegistro = DateOnly.FromDateTime(DateTime.Now),
                    IdUsuario = usuario.Id,
                    DiaHabilitados = new List<DiaHabilitado>()
                };

                foreach (var fechaString in data.Fechas)
                {
                    if (!DateOnly.TryParse(fechaString, out var fecha))
                        throw new InvalidDataException($"La fecha '{fechaString}' no es válida.");

                    var dia = await _unitOfWork.DiaHabilitadoRepo.GetByDate(fecha);

                    if (dia == null)
                    {
                        dia = new DiaHabilitado
                        {
                            Fecha = fecha,
                            Estacions = new List<Estacion>(),
                            Slots = new List<Slot>(),
                            Turnos = new List<Turno>()
                        };

                        for (int i = 1; i <= data.Estaciones; i++)
                        {
                            dia.Estacions.Add(new Estacion
                            {
                                Nombre = $"Estación {i}",
                                Reservas = new List<Reserva>()
                            });
                        }

                        planificacion.DiaHabilitados.Add(dia);
                    }

                    foreach (var turnoDTO in data.Turnos)
                    {
                        var horaInicio = TimeOnly.Parse(turnoDTO.HoraInicio);
                        var horaFin = TimeOnly.Parse(turnoDTO.HoraFin);

                        bool solapa = dia.Turnos.Any(t => (horaInicio < t.HoraFin) && (horaFin > t.HoraInicio));
                        if (solapa)
                            throw new InvalidDataException($"El turno '{turnoDTO.Nombre}' se solapa con un turno existente en la fecha {fecha}. La planificación no se ha guardado.");
                    }

                    foreach (var turnoDTO in data.Turnos)
                    {
                        var horaInicio = TimeOnly.Parse(turnoDTO.HoraInicio);
                        var horaFin = TimeOnly.Parse(turnoDTO.HoraFin);

                        var turno = new Turno
                        {
                            HoraInicio = horaInicio,
                            HoraFin = horaFin,
                            Nombre = turnoDTO.Nombre
                        };
                        dia.Turnos.Add(turno);

                        int duracionCita = config.DuracionCitas;
                        int totalMinutos = (int)(horaFin - horaInicio).TotalMinutes;
                        int cantidadSlots = totalMinutos / duracionCita;

                        for (int s = 0; s < cantidadSlots; s++)
                        {
                            dia.Slots.Add(new Slot
                            {
                                HoraInicio = horaInicio.AddMinutes(s * duracionCita),
                                HoraFin = horaInicio.AddMinutes((s + 1) * duracionCita),
                                CapacidadMaxima = config.CupoEstaciones * data.Estaciones,
                                ReservasActuales = 0
                            });
                        }
                    }
                }

                await _unitOfWork.PlanificacionRepo.Create(planificacion);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();



                return usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<List<DiaHabilitadoDTO>> ObtenerActivos()
        {
            var diaHabilitados = await _unitOfWork.DiaHabilitadoRepo.GetAll();
            if (diaHabilitados == null || !diaHabilitados.Any())
                throw new InvalidOperationException("No se encontraron días disponibles.");

            var diaHabilitadoDtos = diaHabilitados.Select(d => new DiaHabilitadoDTO
            {
                Fecha = d.Fecha.ToString("yyyy-MM-dd"), // 
                Turnos = d.Turnos.Select(t => new TurnoDTO
                {
                    Nombre = t.Nombre,
                    HoraInicio = t.HoraInicio.ToString(@"HH\:mm"), 
                    HoraFin = t.HoraFin.ToString(@"HH\:mm")
                }).ToList()
            }).ToList();

            return diaHabilitadoDtos;
        }

        public async Task<List<SlotDTO>> ObtenerSlotsDisponibles()
        {
            var slots = await _unitOfWork.SlotRepo.ObtenerSlotsDisponiblesAfterDate(DateOnly.FromDateTime(DateTime.Now));
            if (slots == null || !slots.Any())
                throw new InvalidOperationException("No se encontraron slots disponibles para la fecha actual.");

            var slotsDtos = slots.Select(s => new SlotDTO
            {
                PublicId = s.PublicId.ToString(),
                Fecha = s.IdDiaHabilitadoNavigation.Fecha.ToString("yyyy-MM-dd"),
                HoraInicio = s.HoraInicio.ToString("HH:mm:ss"),
                HoraFin = s.HoraFin.ToString("HH:mm:ss"),
                Disponible=s.ReservasActuales<s.CapacidadMaxima
          
            }).ToList();

           

            return slotsDtos;
        }

        
         


    }
}

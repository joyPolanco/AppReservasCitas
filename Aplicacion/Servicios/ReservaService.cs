using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Aplicacion.Servicios
{
    public class ReservaService : IReservaService
    {
       private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        public ReservaService (IUnitOfWork unitofWork, IEmailService emailService)
        {
            _unitOfWork = unitofWork;
            _emailService = emailService;
        }

        public async Task<Usuario> CancelarReserva(string token)
        {
            var valid = Guid.TryParse(token, out var tokenGuid);
            if (!valid)
                throw new ArgumentException("El token de cancelación no es válido.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {

                var tokenDta = await _unitOfWork.TokenCancelacionRepo.GetByGuidAsync(tokenGuid);

                if (tokenDta == null)
                    throw new ArgumentException("El token de cancelación no es válido.");
                if (tokenDta.Usado) throw new ArgumentException("El token de cancelación ya fue utilizado.");

                var reserva = await _unitOfWork.ReservaRepo.ObtenerPorTokenCancelacionAsync(tokenGuid);
                if (reserva == null)
                    throw new ArgumentException("El token de cancelación no es válido.");

                reserva.Estado = "Cancelada";
                await _unitOfWork.ReservaRepo.Update(reserva);
                reserva.IdEstacion = null;
                var slot = await _unitOfWork.SlotRepo.Get(reserva.SlotId);
                slot.ReservasActuales -= 1; 


                tokenDta.Usado = true;
                await _unitOfWork.TokenCancelacionRepo.Update(tokenDta);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
                return tokenDta.Reserva.Usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<Usuario> ConfirmarReserva(string token)
        {
            var valid = Guid.TryParse(token, out var tokenGuid);
            if (!valid)
                throw new ArgumentException("El token no es válido.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {

                var tokenDta = await _unitOfWork.TokenCancelacionRepo.GetByGuidAsync(tokenGuid);

                if (tokenDta == null)
                    throw new ArgumentException("El token  no es válido.");
                if (tokenDta.Usado) throw new ArgumentException("El token de cancelación ya fue utilizado.");

                var reserva = await _unitOfWork.ReservaRepo.ObtenerPorTokenCancelacionAsync(tokenGuid);
                if (reserva == null)
                    throw new ArgumentException("El token no es válido.");

                reserva.Estado = "Confirmada";
                await _unitOfWork.ReservaRepo.Update(reserva);
            


                tokenDta.Usado = true;
                await _unitOfWork.TokenCancelacionRepo.Update(tokenDta);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
                return reserva.Usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task <EmailDataDTO> Reservar(ReservaPostDTO data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrWhiteSpace(data.CorreoUsuario))
                throw new ArgumentException("El correo del usuario es requerido.");

            var usuario = await _unitOfWork.UsuarioRepository.GetUsuarioByEmail(data.CorreoUsuario);
            if (usuario == null) throw new ArgumentException("No se encontró un usuario con ese correo.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var slot = await _unitOfWork.SlotRepo.GetSlotByPublicIdForUpdateAsync(data.IdSlot);
                if (slot == null) throw new InvalidDataException("Slot no encontrado.");
                if (slot.ReservasActuales >= slot.CapacidadMaxima)
                    throw new InvalidDataException("Ya no hay cupos disponibles");

                slot.ReservasActuales += 1;

                var usuarioReservaActiva = await _unitOfWork.ReservaRepo.UsuarioTieneReservaEnFecha(usuario.Id, slot.IdDiaHabilitadoNavigation.Fecha);
                if (usuarioReservaActiva!) throw new InvalidDataException("Ya tienes una reserva para este día.");

                var estacionLibre = await _unitOfWork.EstacionRepo.ObtenerEstacionDisponiblePorSlotAsync(slot.SlotId, slot.IdDiaHabilitado);
                if (estacionLibre == null)
                    throw new InvalidDataException("No hay estaciones disponibles para este slot.");

                // Crear reserva
                var reserva = new Reserva
                {
                    UsuarioId = usuario.Id,
                    SlotId = slot.SlotId,
                    FechaReserva = DateTime.Now,
                    FechaExpiracion = DateTime.Now.AddDays(1),
                    Estado = "Pendiente",
                    IdEstacion = estacionLibre.EstacionId
                };

                await _unitOfWork.ReservaRepo.Create(reserva);
                await _unitOfWork.SaveChangesAsync(); 

                // Crear token de cancelación directamente como Guid
                var tokenCancelacion = new TokenCancelacion
                {
                    ReservaId = reserva.ReservaId,
                    Token = Guid.NewGuid(),
                    FechaCreacion = DateTime.Now,
                    Usado = false
                };

                await _unitOfWork.TokenCancelacionRepo.Create(tokenCancelacion);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();


            
                string urlConfirmacion = $"https://localhost:50983/cancelar?token={tokenCancelacion.Token}";
                string mensaje = $"Tu reserva para el {slot.IdDiaHabilitadoNavigation.Fecha:dd/MM/yyyy} {slot.HoraInicio}- {slot.HoraFin} ha sido programada.\n" +
                                 $"Estación asignada: {reserva.IdEstacionNavigation.Nombre}\n\n" +
                                 $"Si deseas cancelarla o confirmarla, haz clic aquí: {urlConfirmacion}";

                return new EmailDataDTO()
                {
                    Asunto = "Mensaje de confirmacion",
                    Mensaje = mensaje,
                    destinatario = usuario.Email,
                    idUser = usuario.Id


                };
            }
            catch
            {
                try { await transaction.RollbackAsync(); } catch { }
                throw;
            }
        }

        public async Task CancelarReservasExpiradas()
        {


            var expiradas = await _unitOfWork.ReservaRepo.ObtenerReservasExpiradas();
                

            foreach (var r in expiradas)
            {
                r.Estado = "Cancelada";
                // devolver cupo al slot:
                var slot = await _unitOfWork.SlotRepo.Get(r.SlotId);
                if (slot != null)
                    slot.ReservasActuales -= 1 ;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ReservaDTO>> ObtenerReservasUsuario(string correo)
        {
            var usuario = await _unitOfWork.UsuarioRepository.GetUsuarioByEmail(correo);
            if (usuario == null)
                throw new ArgumentException("El usuario no existe");

            var reservas = await _unitOfWork.ReservaRepo.ObtenerReservasUsuario(usuario.Id);
            if (reservas == null || !reservas.Any())
                throw new ArgumentException("El usuario no tiene reservas registradas");

            // Mapear las entidades a DTOs
            var reservasDtos = reservas.Select(r => new ReservaDTO
            {
                Fecha = r.Slot.IdDiaHabilitadoNavigation.Fecha.ToString("yyyy-MM-dd"), 
                HoraInicio = r.Slot.HoraInicio.ToString(@"HH:mm"),
                HoraFin = r.Slot.HoraFin.ToString(@"HH:mm"),

                Estado = r.Estado,
            }).ToList();

            return reservasDtos;
        }
    }
}

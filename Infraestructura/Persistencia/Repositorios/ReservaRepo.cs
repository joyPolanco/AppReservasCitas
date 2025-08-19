using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Infraestructura.Persistencia.Repositorios
{
    public class ReservaRepo : IReservaRepositorio
    {
        private readonly DbSet<Reserva> _entidad;
        private readonly SistemaReservasContext _contexto;

        public ReservaRepo(SistemaReservasContext contexto)
        {
            _contexto = contexto;
            _entidad = contexto.Set<Reserva>();
        }

        public async Task Create(Reserva objeto)
        {
            await _entidad.AddAsync(objeto);
        }

        public async Task<Reserva> Get(int id)
        {
            return await _entidad.FindAsync(id);
        }

        public async Task<bool> UsuarioTieneReservaEnFecha(int usuarioId, DateOnly fecha)
        {
            return await _entidad
                .AnyAsync(r => r.UsuarioId == usuarioId
                               && r.IdEstacionNavigation.DiaHabilitado.Fecha == fecha
                               && (r.Estado == "Confirmada" || r.Estado == "Pendiente"));
        }

        public async Task<Reserva> ObtenerPorTokenCancelacionAsync(Guid token)
        {
            var reserva = await _contexto.TokenCancelacions
                .Where(t => t.Token == token)       // Filtra el token
                .Select(t => t.Reserva)          // Obtiene la reserva relacionada
                .FirstOrDefaultAsync();             // Toma la primera coincidencia o null

            return reserva;
        }

        public async Task<List<Reserva>> GetAll()
        {
            return await _entidad.ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _contexto.SaveChangesAsync();
        }

        public async Task Update(Reserva objeto)
        {
            _entidad.Update(objeto);
            await _contexto.SaveChangesAsync();
        }

        public async Task<List<Reserva>> ObtenerReservasExpiradas()
        {
            return await _entidad.Where(r => r.FechaExpiracion < DateTime.Now && r.Estado == "Pendiente").ToListAsync();
        }

        public Task<List<Reserva>> ObtenerReservasUsuario(int id)
        {
            return _entidad
                .Where(n => n.UsuarioId == id && n.Estado!="Cancelada")
               .Include(n => n.Slot)
                .ThenInclude(s => s.IdDiaHabilitadoNavigation)
            .Include(n => n.Usuario)


                .ToListAsync();
        }
    }
}

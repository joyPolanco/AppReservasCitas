using Aplicacion.DTOs;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Repositorios
{
    
    public class SlotRepositorio : ISlotRepositorio
    {


        private readonly DbSet<Slot> entidad;
        private readonly SistemaReservasContext _contexto;

        public SlotRepositorio(SistemaReservasContext contexto)
        {
            entidad =contexto.Set<Slot>();
            _contexto = contexto;
        }
        public async Task Create(Slot Objeto)
        {
            entidad.Add(Objeto);
            await _contexto.SaveChangesAsync();

        }

      

        public async Task<Slot?> Get(int id)
        {
            var slot = await entidad.FindAsync(id);
            return slot;
        }

        public async Task<List<Slot>> GetAll()
        {
            return await entidad.ToListAsync();
        }
        public async Task<List<Slot>> ObtenerSlotsDisponiblesAfterDate(DateOnly fechaSolicitud)
        {
            return await entidad
           .AsNoTracking()
           .Include(s => s.IdDiaHabilitadoNavigation) // incluye datos del día habilitado
           .Where(s => s.IdDiaHabilitadoNavigation.Fecha >= fechaSolicitud)
           .ToListAsync();
        }

        public async Task<Slot?> GetSlotByPublicIdForUpdateAsync(Guid publicId)
        {
            return await entidad
                .Where(s => s.PublicId == publicId).Include(s => s.IdDiaHabilitadoNavigation)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _contexto.SaveChangesAsync();

        }

        public async Task Update(Slot Objeto)
        {
            entidad.Update(Objeto);
            await _contexto.SaveChangesAsync();
        }

        public async Task<List<Slot>> GetActivos()
        {
            return await entidad
                .Include(s => s.IdDiaHabilitadoNavigation) 
                .Where(s => s.IdDiaHabilitadoNavigation.Fecha >= DateOnly.FromDateTime(DateTime.Now))
                .ToListAsync();
        }
    }
}

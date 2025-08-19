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
    public class EstacionRepo : IEstacionRepositorio
    {
        private readonly DbSet<Estacion> entidad;
        private readonly SistemaReservasContext _contexto;

        public EstacionRepo(SistemaReservasContext contexto)
        {
            entidad = contexto.Set<Estacion>();
            _contexto = contexto;
        }
        public Task Create(Estacion Objeto)
        {
            throw new NotImplementedException();
        }

     
        public async Task<Estacion> Get(int id)
        {
          return await  entidad.FindAsync(id);
        }

        public async Task<List<Estacion>> GetAll()
        {
            return await entidad.ToListAsync();

        }

        public async Task<Estacion?> ObtenerEstacionDisponiblePorSlotAsync(int slotId, int idDiaHabilitado)
        {
            return await entidad
                .Where(e => e.DiaHabilitadoId == idDiaHabilitado &&
                            !e.Reservas.Any(r => r.SlotId == slotId))
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
           return await _contexto.SaveChangesAsync();

        }

        public async Task Update(Estacion Objeto)
        {
               entidad.Update(Objeto);
        }
    }
}

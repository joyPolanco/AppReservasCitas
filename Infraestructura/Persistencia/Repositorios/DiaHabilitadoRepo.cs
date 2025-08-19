using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Persistencia.Repositorios
{
    public class DiaHabilitadoRepo : IDiaHabilitadoRepo
    {
        private readonly SistemaReservasContext _context;
        private readonly DbSet<DiaHabilitado> _dbSet;

        public DiaHabilitadoRepo(SistemaReservasContext context)
        {
            _context = context;
            _dbSet = _context.Set<DiaHabilitado>();
        }

        public async Task Create(DiaHabilitado objeto)
        {
            if (objeto == null)
                throw new ArgumentNullException(nameof(objeto));

            await _dbSet.AddAsync(objeto);
        }

        public async Task<DiaHabilitado> Get(int id)
        {
            return await _dbSet
                .Include(d => d.Turnos)
                .Include(d => d.Slots)
                .Include(d => d.Estacions)
                .FirstOrDefaultAsync(d => d.DiaHabilitadoId == id);
        }

        public async Task<List<DiaHabilitado>> GetAll()
        {
            return await _dbSet
                .Include(d => d.Turnos)
                .Include(d => d.Slots)
                .Include(d => d.Estacions)
                .ToListAsync();
        }

        public async Task<DiaHabilitado> GetByDate(DateOnly fecha)
        {
            return await _dbSet
                .Include(d => d.Turnos)
                .Include(d => d.Slots)
                .Include(d => d.Estacions)
                .FirstOrDefaultAsync(d => d.Fecha == fecha);
        }

        public async Task Update(DiaHabilitado objeto)
        {
            if (objeto == null)
                throw new ArgumentNullException(nameof(objeto));

            _dbSet.Update(objeto);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

       
    }
}

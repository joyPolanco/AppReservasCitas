using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Repositorios
{
    public class PlanificacionRepositorio : IPlanificacionRepositorio
    {
        private readonly SistemaReservasContext _context;

        public PlanificacionRepositorio(SistemaReservasContext context)
        {
            _context = context;
        }

        public async Task Create(Planificacion objeto)
        {
            await _context.Planificacions.AddAsync(objeto);
        }

       

        public async Task<bool> ExistePlanificacionEnFechasYTurnos(
            List<DateOnly> fechas,
            List<string> nombresTurnos)
        {
            return await _context.Planificacions
                .AnyAsync(p => p.DiaHabilitados
                    .Any(d => fechas.Contains(d.Fecha) ));
        }

        public async Task<Planificacion> Get(int id)
        {
            return await _context.Planificacions
        .Include(p => p.DiaHabilitados)
            .ThenInclude(d => d.Turnos)
        .Include(p => p.DiaHabilitados)
            .ThenInclude(d => d.Estacions)
        .FirstOrDefaultAsync(p => p.PlanificacionId == id);
        }

        public async Task<List<Planificacion>> GetAll()
        {
            return await _context.Planificacions
     .Include(p => p.DiaHabilitados)
         .ThenInclude(d => d.Turnos)
     .Include(p => p.DiaHabilitados)
         .ThenInclude(d => d.Estacions)
     .ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task Update(Planificacion objeto)
        {
            _context.Planificacions.Update(objeto);
            await Task.CompletedTask;
        }
    }
}

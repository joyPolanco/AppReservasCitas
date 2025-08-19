using Aplicacion.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Repositorios
{
    public class RepoGenerico<T> : IRepositorio<T> where T : class
    {
        private readonly DbSet<T> Entidad;
        private readonly SistemaReservasContext _contexto;
        public RepoGenerico(SistemaReservasContext contexto)
        {
            Entidad = contexto.Set<T>();
            _contexto = contexto;
        }
        public async Task Create(T Objeto)
        {
            await Entidad.AddAsync(Objeto);
        }

      


        public async Task<T> Get(int id)
        {
            return await Entidad.FindAsync(id);
        }

        public async Task<List<T>> GetAll()
        {
            return await Entidad.ToListAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _contexto.SaveChangesAsync();
        }

        public async Task Update(T Objeto)
        {
           Entidad.Update(Objeto);
        }
    }
}

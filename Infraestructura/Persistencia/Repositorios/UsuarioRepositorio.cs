using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace Infraestructura.Persistencia.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepository
    {
        private readonly DbSet<Usuario> entidad ;
        private readonly DbContext _contexto;
        public UsuarioRepositorio (SistemaReservasContext contexto)
        {
            entidad = contexto.Usuarios;
            _contexto = contexto;
        }
        public async Task Create(Usuario Objeto)
        {
            entidad.Add(Objeto);
            await _contexto.SaveChangesAsync();

        }

      

        public async Task<Usuario?> Get(int id)
        {
           var usuario= await entidad.FindAsync(id);
            return usuario;
        }


        public async Task<List<Usuario>> GetAll()
        {
            return await entidad.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioByEmail(string email)
           
        {
            var user = await entidad.Include(e=>e.IdRolNavigation).FirstOrDefaultAsync(e => e.Email.Equals(email));

            return user;
        }

        public async Task<int> SaveChangesAsync()
        {
          return await _contexto.SaveChangesAsync();
        }

        public async  Task Update(Usuario Objeto)
        {
             entidad.Update(Objeto);
             await _contexto.SaveChangesAsync();
        }
    }
}

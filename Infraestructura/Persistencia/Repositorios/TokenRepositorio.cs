using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Repositorios
{
    public class TokenRepositorio : ITokenRepositorio
    {
        private readonly SistemaReservasContext _context;

        public TokenRepositorio(SistemaReservasContext context)
        {
            _context = context;
        }

        public async Task Create(TokenCancelacion objeto)
        {
            await _context.TokenCancelacions.AddAsync(objeto);
        }

        public async Task<TokenCancelacion> Get(int id)
        {
            return await _context.TokenCancelacions.FindAsync(id);
        }

        public async Task<List<TokenCancelacion>> GetAll()
        {
            return await _context.TokenCancelacions.ToListAsync();
        }

        public async Task Update(TokenCancelacion objeto)
        {
            _context.TokenCancelacions.Update(objeto);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<TokenCancelacion?> GetByGuidAsync(Guid token)
        {
            return await _context.TokenCancelacions
                .Include(t => t.Reserva)       
                    .ThenInclude(r => r.Usuario) 
                .FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}

using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface ITokenRepositorio : IRepositorio<TokenCancelacion>
    {

        public Task<TokenCancelacion?> GetByGuidAsync(Guid token);
       }
}

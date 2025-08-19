using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Loggers
{
    public class DatabaseLogger : ILoggerStrategy
    {
        private  IRepositorio<Log> _repo;

      public DatabaseLogger (IRepositorio<Log> repo)
        {
            _repo = repo;
        }

        public async Task Registrar(Log log)
        {
            await _repo.Create(log);
            await _repo.SaveChangesAsync();
        }

       
    }
}

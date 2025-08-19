using Dominio.Entidades;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface ILoggerStrategy
    {

        public Task Registrar(Log log);
        
    }
}

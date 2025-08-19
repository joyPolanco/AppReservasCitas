using Aplicacion.Interfaces;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IDiaHabilitadoRepo:IRepositorio<DiaHabilitado>
    {
        public Task<DiaHabilitado> GetByDate(DateOnly fecha);
    }
}

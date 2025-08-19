using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IPlanificacionRepositorio:IRepositorio<Planificacion>
    {
        public Task<bool> ExistePlanificacionEnFechasYTurnos(List<DateOnly> fechas, List<string> nombresTurnos);

    }
}

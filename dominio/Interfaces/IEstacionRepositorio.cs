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
    public interface IEstacionRepositorio :IRepositorio<Estacion>
    {
        public Task<Estacion?> ObtenerEstacionDisponiblePorSlotAsync(int slotId, int idDiaHabilitado);

    }
}

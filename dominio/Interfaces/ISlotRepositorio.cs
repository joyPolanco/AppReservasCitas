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
    public interface ISlotRepositorio : IRepositorio<Slot>

    {
        public  Task<List<Slot>> ObtenerSlotsDisponiblesAfterDate(DateOnly fechaSolicitud);
        public Task<Slot?> GetSlotByPublicIdForUpdateAsync(Guid publicId);
        public Task<List<Slot>> GetActivos();

    }
}

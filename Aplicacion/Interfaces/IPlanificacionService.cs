using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IPlanificacionService
    {
        public Task<Usuario> CrearPlanificacion(DatosPlanificacion data);

        public Task<List<DiaHabilitadoDTO>> ObtenerActivos();
        public Task<List<SlotDTO>> ObtenerSlotsDisponibles();



    }
}

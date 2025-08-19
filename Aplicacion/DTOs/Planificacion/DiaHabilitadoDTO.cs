using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Planificacion
{
    public class DiaHabilitadoDTO
    {
        public string Fecha { get; set; }
        public List<TurnoDTO>Turnos{get;set ;}
    }
}

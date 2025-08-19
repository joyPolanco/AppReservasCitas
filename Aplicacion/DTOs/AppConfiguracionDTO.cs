using Aplicacion.DTOs.Planificacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{
    public class AppConfiguracionDTO
    {
        public int DuracionCitas { get; set; }

        public int CupoEstaciones { get; set; }
        public List<TurnoDTO> Turnos { get; set; }
    }
}

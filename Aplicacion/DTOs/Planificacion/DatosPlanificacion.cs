using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Planificacion
{
    public class DatosPlanificacion
    {
        public List<string> Fechas { get; set; }
        public int Estaciones { get; set; }
        public List<TurnoDTO> Turnos { get;  set; }
        public string CorreoUsuario { get; set; }
    }
}

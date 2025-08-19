using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{
    public class EmailDataDTO
    {
        public string destinatario { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public int idUser { get; set; }
    }
}

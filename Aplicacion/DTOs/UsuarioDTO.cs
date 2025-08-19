using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{
    public class UsuarioDTO
    {
        public string Email { get; set; } = null!;
        public Guid PublicId { get; set; }
        public string ContrasenaHasheada { get; set; } = null!; //contrasena no hasheada
        public string Rol { get; set; }
    }
}

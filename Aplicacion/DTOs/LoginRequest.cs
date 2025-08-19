using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs
{//se recibe del login
    public class LoginRequest
    {
        public string Email { get; set; } = null!;

        public string Contrasena { get; set; } = null!; //contrasena no hasheada
    }
}

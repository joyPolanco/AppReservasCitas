using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class AuthService : IAuthService

    {
        public readonly IPasswordManager _encryptor;
        private readonly IUsuarioService _service;
   
        public AuthService 
            (IPasswordManager passwordEncryptor, 
            IUsuarioService usuarioService)
        {
            _encryptor = passwordEncryptor;
        
            _service = usuarioService;

        }

       

        public   bool ValidarDatos(LoginRequest usuario, string contrasena)
        {
          return _encryptor.Verify(usuario.Contrasena, contrasena);
           
        }

        }
    }


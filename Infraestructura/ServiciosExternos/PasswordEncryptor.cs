using System;
using Aplicacion.Interfaces;
using BCrypt.Net;

namespace Infraestructura.ServiciosExternos
{
    public  class PasswordEncryptor:IPasswordManager
    {
       
        public  string Encrypt(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede estar vacía");
            }

            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

     
        public  bool Verify(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "La contraseña no puede estar vacía");
            }

            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                throw new ArgumentNullException(nameof(hashedPassword), "El hash de contraseña no puede estar vacío");
            }

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
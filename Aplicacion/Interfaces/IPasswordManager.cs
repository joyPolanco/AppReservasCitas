using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IPasswordManager
    {
        public string Encrypt(string password);
        
       

        public bool Verify(string password, string hashedPassword);
    }
    }


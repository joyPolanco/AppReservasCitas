using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Casosdeuso
{
  
        public class JwtConfiguration
        {
            public static string Secret { get { return "EstaEsUnaClaveMuySegura1234567890"; } }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public int ExpirationMinutes { get { return 45; } set { } }
        }

    }


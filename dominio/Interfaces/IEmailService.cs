using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string destino, string asunto, string mensaje);
    }
}

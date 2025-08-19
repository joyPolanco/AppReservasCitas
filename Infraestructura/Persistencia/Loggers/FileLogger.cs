using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia.Loggers
{
    public class FileLogger : ILoggerStrategy
    {
        private readonly string _ruta;

        public FileLogger()
        {
            string carpetaEjecucion = AppDomain.CurrentDomain.BaseDirectory;
            _ruta = Path.Combine(carpetaEjecucion, "logs.txt");

            if (!File.Exists(_ruta))
            {
                using (var stream = File.Create(_ruta)) { }
            }
        }

        public async Task Registrar(Log log)
        {

            string linea = $"{log.FechaHora} [{log.Tipo}] {log.Mensaje}";

            await File.AppendAllTextAsync(_ruta, linea + Environment.NewLine, Encoding.UTF8);
        }
    }
}

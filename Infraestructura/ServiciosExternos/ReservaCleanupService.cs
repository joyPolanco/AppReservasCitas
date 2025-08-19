using Aplicacion.Interfaces;
using Dominio.Entidades.Models;
using Infraestructura.Persistencia.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.ServiciosExternos
{
    public class ReservaCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(5);

        public ReservaCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reservaService = scope.ServiceProvider.GetRequiredService<IReservaService>();
                        await reservaService.CancelarReservasExpiradas();
                    }
                }
                catch (Exception ex)
                {
                    LoggerContext.GetLogger().SetFileLogger();
                    await LoggerContext.GetLogger().Registrar(new Log
                    {
                         FechaHora = DateTime.UtcNow
                         ,Tipo="Error",
                         Mensaje= "Error al ejecutar la limpieza de reservas expiradas"

                     });
                }

                try
                {
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                }
            }

        }
    }
}

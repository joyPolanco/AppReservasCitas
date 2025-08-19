using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Interfaces
{
    public interface IReservaRepositorio:IRepositorio<Reserva>
    {
        public Task<bool> UsuarioTieneReservaEnFecha(int usuarioId, DateOnly fecha);
        public  Task<Reserva> ObtenerPorTokenCancelacionAsync(Guid token);
        public Task<List<Reserva>> ObtenerReservasExpiradas();
        public Task<List<Reserva>> ObtenerReservasUsuario(int id);


    }

}

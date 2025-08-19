using Aplicacion.DTOs;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IReservaService
    {

        public Task<EmailDataDTO> Reservar(ReservaPostDTO DATA);

        public  Task CancelarReservasExpiradas();

        public Task<Usuario> CancelarReserva(string token);
        public Task<Usuario> ConfirmarReserva(string token);

        public Task< List<ReservaDTO> >ObtenerReservasUsuario(string correo);

    }
}

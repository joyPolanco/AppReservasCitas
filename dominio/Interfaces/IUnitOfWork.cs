using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
       IReservaRepositorio ReservaRepo { get; }

         IDiaHabilitadoRepo DiaHabilitadoRepo { get; }
        IEstacionRepositorio EstacionRepo { get; }
        IPlanificacionRepositorio PlanificacionRepo { get; }
        ISlotRepositorio SlotRepo { get; }
        IRepositorio<Turno> TurnoRepo { get; }
        ITokenRepositorio TokenCancelacionRepo { get; }

        IUsuarioRepository UsuarioRepository { get; }

        Task<int> SaveChangesAsync();

       Task<IDbContextTransaction> BeginTransactionAsync();
    }

}

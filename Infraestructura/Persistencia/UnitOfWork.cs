using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using Infraestructura.Persistencia.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Persistencia
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SistemaReservasContext _context;

        public IDiaHabilitadoRepo DiaHabilitadoRepo { get; }
        public IEstacionRepositorio  EstacionRepo { get; }
        public IPlanificacionRepositorio PlanificacionRepo { get; }
        public ISlotRepositorio SlotRepo{ get; }
        public IRepositorio<Turno> TurnoRepo { get; }

        public ITokenRepositorio TokenCancelacionRepo { get; }

        public IUsuarioRepository UsuarioRepository { get; }

        public IReservaRepositorio ReservaRepo { get; }


        public UnitOfWork(
            SistemaReservasContext context,
            IDiaHabilitadoRepo diaHabilitadoRepo,
            IEstacionRepositorio  estacionRepo,
           IPlanificacionRepositorio planificacionRepo,
            ISlotRepositorio slotRepo,
            IUsuarioRepository usuarioRepository,
            IRepositorio<Turno> turnoRepo,
            IReservaRepositorio reservaRepositorio,
             ITokenRepositorio tokenCancelacionRepo

            )
        {
            _context = context;
            DiaHabilitadoRepo = diaHabilitadoRepo;
            EstacionRepo = estacionRepo;
            PlanificacionRepo = planificacionRepo;
            SlotRepo = slotRepo;
            UsuarioRepository = usuarioRepository;
            ReservaRepo = reservaRepositorio;
            TokenCancelacionRepo = tokenCancelacionRepo;

        }
     
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        
        public void Dispose() => _context.Dispose();

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }

}

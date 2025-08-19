using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Reserva
{
    public int ReservaId { get; set; }

    public int UsuarioId { get; set; }

    public int SlotId { get; set; }

    public DateTime FechaReserva { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime? FechaExpiracion { get; set; }

    public int? IdEstacion { get; set; }

    public virtual Estacion? IdEstacionNavigation { get; set; }

    public virtual Slot Slot { get; set; } = null!;

    public virtual ICollection<TokenCancelacion> TokenCancelacions { get; set; } = new List<TokenCancelacion>();

    public virtual Usuario Usuario { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class TokenCancelacion
{
    public int Id { get; set; }

    public int ReservaId { get; set; }

    public Guid Token { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool Usado { get; set; }

    public virtual Reserva Reserva { get; set; } = null!;
}

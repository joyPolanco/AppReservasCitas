using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Estacion
{
    public int EstacionId { get; set; }

    public int DiaHabilitadoId { get; set; }

    public string? Nombre { get; set; }

    public virtual DiaHabilitado DiaHabilitado { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

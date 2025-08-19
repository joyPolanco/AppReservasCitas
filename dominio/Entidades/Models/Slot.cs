using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Slot
{
    public int SlotId { get; set; }

    public Guid PublicId { get; set; }

    public int IdDiaHabilitado { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public int CapacidadMaxima { get; set; }

    public int ReservasActuales { get; set; }

    public virtual DiaHabilitado IdDiaHabilitadoNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

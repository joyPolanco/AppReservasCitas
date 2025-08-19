using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Turno
{
    public int TurnoId { get; set; }

    public int IdDiaHabilitado { get; set; }

    public string Nombre { get; set; } = null!;

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public DateOnly? FechaCreacion { get; set; }

    public virtual DiaHabilitado IdDiaHabilitadoNavigation { get; set; } = null!;
}

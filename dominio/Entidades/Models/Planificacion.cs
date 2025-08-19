using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Planificacion
{
    public int PlanificacionId { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public int IdUsuario { get; set; }

    public virtual ICollection<DiaHabilitado> DiaHabilitados { get; set; } = new List<DiaHabilitado>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}

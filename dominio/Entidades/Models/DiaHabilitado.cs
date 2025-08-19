using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class DiaHabilitado
{
    public int DiaHabilitadoId { get; set; }

    public int PlanificacionId { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual ICollection<Estacion> Estacions { get; set; } = new List<Estacion>();

    public virtual Planificacion Planificacion { get; set; } = null!;

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}

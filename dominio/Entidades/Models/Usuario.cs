using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public Guid PublicId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string HashContrasena { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual Role IdRolNavigation { get; set; } = null!;


    public virtual ICollection<Planificacion> Planificacions { get; set; } = new List<Planificacion>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}

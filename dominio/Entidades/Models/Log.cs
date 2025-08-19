using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class Log
{
    public int LogId { get; set; }

    public DateTime FechaHora { get; set; }

    public string Tipo { get; set; } = null!;

    public int? UsuarioId { get; set; }

    public string Mensaje { get; set; } = null!;
}

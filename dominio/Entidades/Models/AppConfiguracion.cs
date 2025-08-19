using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Models;

public partial class AppConfiguracion
{
    public int Id { get; set; }

    public int CupoEstaciones { get; set; }

    public TimeOnly HoraInicioMatutino { get; set; }

    public TimeOnly HoraFinMatutino { get; set; }

    public TimeOnly HoraInicioVespertino { get; set; }

    public TimeOnly HoraFinVespertino { get; set; }

    public TimeOnly HoraInicioNocturno { get; set; }

    public TimeOnly HoraFinNocturno { get; set; }

    public int DuracionCitas { get; set; }
}

using System;
using System.Collections.Generic;

namespace API_WebLabCon_test.Models;

public partial class Estacione
{
    public string IdEstacion { get; set; } = null!;

    public string NombreEstacion { get; set; } = null!;

    public double Longitud { get; set; }

    public double Latitud { get; set; }

    public double Altitud { get; set; }

    public string Municipio { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Dato> Datos { get; set; } = new List<Dato>();

    public virtual Municipio MunicipioNavigation { get; set; } = null!;
}
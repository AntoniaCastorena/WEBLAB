using System;
using System.Collections.Generic;

namespace API_WebLabCon_test.Models;

public partial class Municipio
{
    public string IdMunicipio { get; set; } = null!;

    public string Nmunicipio { get; set; } = null!;

    public string? Estado { get; set; }

    public virtual ICollection<Estacione> Estaciones { get; set; } = new List<Estacione>();

    public virtual Estado? EstadoNavigation { get; set; }
}

using System;
using System.Collections.Generic;

namespace API_WebLabCon_test.Models;

public partial class Estado
{
    public string IdEstado { get; set; } = null!;

    public string Nestado { get; set; } = null!;

    public virtual ICollection<Municipio> Municipios { get; set; } = new List<Municipio>();
}

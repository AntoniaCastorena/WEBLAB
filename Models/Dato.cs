using System;
using System.Collections.Generic;

namespace API_WebLabCon_test.Models;

public partial class Dato
{
    public int IdDato { get; set; }

    public string Estacion { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public double Prec { get; set; }

    public double Temp { get; set; }

    public double Dirv { get; set; }

    public double Velv { get; set; }

    public double Rad { get; set; }

    public double Humr { get; set; }

    public double Humh { get; set; }

    public double Eto { get; set; }

    public virtual Estacione EstacionNavigation { get; set; } = null!;
}

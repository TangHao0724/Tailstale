﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class medicine
{
    public int id { get; set; }

    public string generic_name { get; set; }

    public string brand_name { get; set; }

    public string route { get; set; }

    public string timing { get; set; }

    public string indication { get; set; }

    public string side_effects { get; set; }

    public decimal? dosage { get; set; }

    public string dosage_type { get; set; }

    public string frequency { get; set; }

    public virtual ICollection<medicine_order> medicine_orders { get; set; } = new List<medicine_order>();
}
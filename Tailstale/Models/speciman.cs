﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class speciman
{
    public int id { get; set; }

    public int? pet_type { get; set; }

    public string specimen_name { get; set; }

    public string test_name { get; set; }

    public decimal? normal_range_min { get; set; }

    public decimal? normal_range_max { get; set; }

    public virtual ICollection<biological_test_order> biological_test_orders { get; set; } = new List<biological_test_order>();

    public virtual ICollection<biological_test> biological_tests { get; set; } = new List<biological_test>();

    public virtual pet_type pet_typeNavigation { get; set; }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class biological_test_order
{
    public int id { get; set; }

    public DateTime order_time { get; set; }

    public DateTime test_time { get; set; }

    public int? specimen_id { get; set; }

    public int? medical_orders_id { get; set; }

    public virtual medical_order medical_orders { get; set; }

    public virtual speciman specimen { get; set; }
}
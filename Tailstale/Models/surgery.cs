﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class surgery
{
    public int id { get; set; }

    public DateTime start_time { get; set; }

    public string technique { get; set; }

    public string anesthesia { get; set; }

    public string findings { get; set; }

    public string op_duration { get; set; }

    public int? medical_orders_id { get; set; }

    public virtual medical_order medical_orders { get; set; }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class nursing_record
{
    public int id { get; set; }

    public DateTime datetime { get; set; }

    public decimal? weight { get; set; }

    public string memo { get; set; }

    public int? hosp_id { get; set; }

    public virtual hosp_record hosp { get; set; }
}
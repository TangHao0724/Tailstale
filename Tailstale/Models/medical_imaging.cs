﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class medical_imaging
{
    public int id { get; set; }

    public int? pet_id { get; set; }

    public DateTime? examined_at { get; set; }

    public string type { get; set; }

    public string examined_area { get; set; }

    public string findings { get; set; }

    public virtual pet pet { get; set; }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class biological_test
{
    public int id { get; set; }

    public int? pet_id { get; set; }

    public DateTime? created_at { get; set; }

    public int? specimen_id { get; set; }

    public int lab_data { get; set; }

    public string findings { get; set; }

    public virtual pet pet { get; set; }

    public virtual speciman specimen { get; set; }
}
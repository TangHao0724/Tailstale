﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class keeper_img_type
{
    public int ID { get; set; }

    public int? FK_Keeper_id { get; set; }

    public string typename { get; set; }

    public DateTime? created_at { get; set; }

    public virtual keeper FK_Keeper { get; set; }

    public virtual ICollection<keeper_img> keeper_imgs { get; set; } = new List<keeper_img>();
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class business_img_type
{
    public int ID { get; set; }

    public int? FK_business_id { get; set; }

    public string typename { get; set; }

    public DateTime? created_at { get; set; }

    public virtual business FK_business { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<business_img> business_imgs { get; set; } = new List<business_img>();
}
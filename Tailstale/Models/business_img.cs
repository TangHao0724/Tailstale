﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class business_img
{
    public int ID { get; set; }

    public int? img_type_id { get; set; }

    public string URL { get; set; }

    public string name { get; set; }

    public DateTime? created_at { get; set; }

    public virtual business_img_type img_type { get; set; }
}
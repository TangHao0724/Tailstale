﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tailstale.Models;

public partial class Service
{
    public int id { get; set; }

    [Display(Name = "門市")]
    public int business_ID { get; set; }

    [Display(Name = "寵物種類")]
    public string category { get; set; }

    [Display(Name = "服務項目")]
    public string service_name { get; set; }

    [Display(Name = "服務內容")]
    public string service_content { get; set; }

    [Display(Name = "服務圖片")]
    public string service_img { get; set; }

    [Display(Name = "價格")]
    public string price { get; set; }

    [Display(Name = "創建日期")]
    public DateTime? created_at { get; set; }

    public virtual business business { get; set; }
}
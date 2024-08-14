﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tailstale.Models;

public partial class Business_hour
{
    public int id { get; set; }

    [Display(Name = "門市")]
    public int business_ID { get; set; }

    [Display(Name = "營業日期")]
    public DateOnly business_day { get; set; }

    [Display(Name = "開始時間")]
    public TimeOnly? open_time { get; set; }

    [Display(Name = "結束時間")]
    public TimeOnly? close_time { get; set; }

    [Display(Name = "預約時段人數")]
    public int people_limit { get; set; }

    public virtual business business { get; set; }
}
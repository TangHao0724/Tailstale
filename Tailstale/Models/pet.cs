﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class pet
{
    public int pet_ID { get; set; }

    public int? pet_type_ID { get; set; }

    public int? keeper_ID { get; set; }

    public string name { get; set; }

    public string chip_ID { get; set; }

    public bool? gender { get; set; }

    public DateOnly? birthday { get; set; }

    public int? age { get; set; }

    public decimal? pet_weight { get; set; }

    public string vaccine { get; set; }

    public bool? neutered { get; set; }

    public string allergy { get; set; }

    public string chronic_dis { get; set; }

    public string memo { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<CheckinDetail> CheckinDetails { get; set; } = new List<CheckinDetail>();

    public virtual keeper keeper { get; set; }

    public virtual ICollection<medical_order> medical_orders { get; set; } = new List<medical_order>();

    public virtual ICollection<medical_record> medical_records { get; set; } = new List<medical_record>();

    public virtual pet_type pet_type { get; set; }
}
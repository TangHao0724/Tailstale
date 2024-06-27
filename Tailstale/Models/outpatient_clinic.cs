﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class outpatient_clinic
{
    public int outpatient_clinic_ID { get; set; }

    public string name { get; set; }

    public int? business_ID { get; set; }

    public int? department_ID { get; set; }

    public int? vet_ID { get; set; }

    public DateOnly date { get; set; }

    public TimeOnly time { get; set; }

    public int max_patients { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual business business { get; set; }

    public virtual department department { get; set; }

    public virtual ICollection<medical_record> medical_records { get; set; } = new List<medical_record>();

    public virtual vet_information vet { get; set; }
}
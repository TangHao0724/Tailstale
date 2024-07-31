﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class outpatient_clinic
{
    public int outpatient_clinic_ID { get; set; }

    public int? outpatient_clinic_timeslot_ID { get; set; }

    public string outpatient_clinic_name { get; set; }

    public int? vet_ID { get; set; }

    public string dayofweek { get; set; }

    public int max_patients { get; set; }

    public virtual ICollection<daily_outpatient_clinic_schedule> daily_outpatient_clinic_schedules { get; set; } = new List<daily_outpatient_clinic_schedule>();

    public virtual ICollection<medical_record> medical_records { get; set; } = new List<medical_record>();

    public virtual outpatient_clinic_timeslot outpatient_clinic_timeslot { get; set; }

    public virtual vet_Information vet { get; set; }
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class outpatient_clinic_timeslot
{
    public int outpatient_clinic_timeslot_ID { get; set; }

    public string outpatient_clinic_timeslot_name { get; set; }

    public TimeOnly? startat { get; set; }

    public TimeOnly? endat { get; set; }

    public virtual ICollection<outpatient_clinic> outpatient_clinics { get; set; } = new List<outpatient_clinic>();
}
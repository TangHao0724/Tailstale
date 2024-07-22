﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class keeper
{
    public int ID { get; set; }

    public string password { get; set; }

    public string salt { get; set; }

    public string name { get; set; }

    public string phone { get; set; }

    public string email { get; set; }

    public string address { get; set; }

    public int? status { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Consumption_Record> Consumption_Records { get; set; } = new List<Consumption_Record>();

    public virtual ICollection<Message> MessageFK_Targets { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageFK_Users { get; set; } = new List<Message>();

    public virtual ICollection<PaymentInfo> PaymentInfos { get; set; } = new List<PaymentInfo>();

    public virtual ICollection<Reserve> Reserves { get; set; } = new List<Reserve>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<article> articles { get; set; } = new List<article>();

    public virtual ICollection<keeper_img_type> keeper_img_types { get; set; } = new List<keeper_img_type>();

    public virtual ICollection<medical_record> medical_records { get; set; } = new List<medical_record>();

    public virtual ICollection<memo> memos { get; set; } = new List<memo>();

    public virtual ICollection<person_tag> person_tags { get; set; } = new List<person_tag>();

    public virtual ICollection<pet> pets { get; set; } = new List<pet>();

    public virtual member_status statusNavigation { get; set; }

    public virtual ICollection<using_person_tag> using_person_tags { get; set; } = new List<using_person_tag>();
}
﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Tailstale.Models;

public partial class roomType
{
    public int roomType_ID { get; set; }

    public string roomType1 { get; set; }

    public int? FK_businessID { get; set; }

    public virtual business FK_business { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
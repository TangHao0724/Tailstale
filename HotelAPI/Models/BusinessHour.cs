﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HotelAPI.Models;

public partial class BusinessHour
{
    public int Id { get; set; }

    public int BusinessId { get; set; }

    public DateOnly BusinessDay { get; set; }

    public int PeopleLimit { get; set; }

    public virtual Business Business { get; set; }
}
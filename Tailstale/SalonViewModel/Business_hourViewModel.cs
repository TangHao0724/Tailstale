﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Tailstale.Models
{
    public class Business_hourViewModel
    {
        public int id { get; set; }

        public int business_ID { get; set; }


        [Required(ErrorMessage = " business_day欄位未填寫")]
        public DateOnly business_day { get; set; }

        [Required(ErrorMessage = " open_time欄位未填寫")]
        public TimeOnly? open_time { get; set; }

        [Required(ErrorMessage = " close_time欄位未填寫")]
        public TimeOnly? close_time { get; set; }

        public int people_limit { get; set; }

        public virtual business business { get; set; }
    }
}

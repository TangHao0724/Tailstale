﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace Tailstale.Models
{
    public class ReserveViewModel
    {
        public int id { get; set; }


        [Required(ErrorMessage = "keeper_id欄位未填寫")]
        public int keeper_id { get; set; }

        [StringLength(maximumLength: 10, MinimumLength = 1, ErrorMessage = "長度不合法,最多10字")]//,最多8個字,最少3個字,可以防攻擊
        [Required(ErrorMessage = "pet_name欄位未填寫")]
        public string pet_name { get; set; }


        [Required(ErrorMessage = "business_ID欄位未填寫")]
        public int business_ID { get; set; }


        [Required(ErrorMessage = "time欄位未填寫")]
        public DateTime time { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "長度不合法,最多100字")]//,最多8個字,最少3個字,可以防攻擊
        [Required(ErrorMessage = "service_name欄位未填寫")]
        public string service_name { get; set; }

        public DateTime? created_at { get; set; }

        public int? status { get; set; }

        public virtual business business { get; set; }

        public virtual keeper keeper { get; set; }

        public virtual order_status statusNavigation { get; set; }
    }
}

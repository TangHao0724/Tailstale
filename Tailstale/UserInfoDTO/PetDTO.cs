namespace Tailstale.UserInfoDTO
{
    public class PostPetDTO
    {
        public int? pet_type_ID { get; set; }

        public int? keeper_ID { get; set; }
        public string name { get; set; }

        public bool gender { get; set; }

        public DateOnly? birthday { get; set; }

        public int? age { get; set; }



    }
    public class GetPetDTO
    {
        public int? pet_type_ID { get; set; }

        public string name { get; set; }


        public DateOnly? birthday { get; set; }

        public int? age { get; set; }
    }
    public class GetPetTypeDTO
    {
        public int ID { get; set; }

        public string species { get; set; }

        public string breed { get; set; }



    }
    public class PetInfoDTO
    {
        public int ID { get; set; }
        public string name { get; set; }
        public int pet_type_ID { get; set; }
        public bool gender { get; set; }
        public string chip_ID { get; set; }
        public int? age { get; set; }
        public DateOnly? birthday { get; set; }
        public decimal? pet_weight { get; set; }
        public string vaccine { get; set; }

        public bool? neutered { get; set; }

        public string allergy { get; set; }

        public string chronic_dis { get; set; }

        public string memo { get; set; }
    }

}

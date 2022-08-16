using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eCareApi.Entities
{
    [Table("HOSPITAL_TRACHEAL_SUCTION_METHOD")] 
    public class HospitalTrachealSuctionMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_tracheal_suction_method_id { get; set; }
        public string suction_method { get; set; }
        public string suction_method_code { get; set; }
        public string suction_method_indication { get; set; }
        public string suction_method_recommendations { get; set; }

    }
}

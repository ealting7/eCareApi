using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_FI02_LEVELS")] 
    public class HospitalFi02Levels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_fi02_levels_id { get; set; }
        public string liters_per_minute { get; set; }
        public decimal approximate_fi02 { get; set; }

    }
}

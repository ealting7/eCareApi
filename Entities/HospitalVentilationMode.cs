using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_VENTILATION_MODE")] 
    public class HospitalVentilationMode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_ventilation_mode_id { get; set; }

        public string ventilation_mode { get; set; }
        public string ventilation_mode_code { get; set; }
        public string ventilation_mode_indication { get; set; }
        public string ventilation_mode_recommendations { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("HOSPITAL_VENTILATION_TYPE")] 
    public class HospitalVentilationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_ventilation_type_id { get; set; }
        
        public string ventilation_type { get; set; }
        public string ventilation_type_code { get; set; }
        public string ventilation_type_indication { get; set; }
        public string ventilation_type_recommendations { get; set; }

    }
}

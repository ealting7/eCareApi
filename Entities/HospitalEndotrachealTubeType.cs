using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_ENDOTRACHEAL_TUBE_TYPE")] 
    public class HospitalEndotrachealTubeType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_endotracheal_tube_type_id { get; set; }
        public string tube_type { get; set; }
        public string tube_type_code { get; set; }
        public string intubation_indication { get; set; }
        public string intubation_recommendations { get; set; }

    }
}

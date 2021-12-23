using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_CAREPLAN_INTERVENTION_TYPE")]    
    public class HospitalCareplanInterventionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_careplan_intervention_type_id { get; set; }
        public string intervention_type { get; set; }
        public DateTime? creation_date { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CAREPLAN_INTERVENTION_ADMINISTERED")]    
    public class HospitalInpatientAdmissionCareplanInterventionAdministered
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_careplan_intervention_administered_id { get; set; }
        public int hospital_inpatient_admission_careplan_intervention_id { get; set; }
        public DateTime creation_date { get; set; }

    }
}

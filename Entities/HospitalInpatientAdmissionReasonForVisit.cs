using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_REASON_FOR_VISIT")]

    public class HospitalInpatientAdmissionReasonForVisit
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int hospital_inpatient_admission_reason_for_visit_id { get; set; }

            public int? hospital_inpatient_admission_id { get; set; }
            
            public string reason_for_visit { get; set; }
            public Guid? creation_user_id { get; set; }
            public DateTime? creation_date { get; set; }
            public Guid? update_user_id { get; set; }
            public DateTime? update_date { get; set; }
    }
}

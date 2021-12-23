using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CAREPLAN_DIAGNOSIS")]
    public class HospitalInpatientAdmissionCareplanDiagnosis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_careplan_diagnosis_id { get; set; }
        public int member_disease_condition_careplan_id { get; set; }
        public int hospital_inpatient_admission_id { get; set; }
        public string diagnosis { get; set; }
        public int? task_priority_id { get; set; }
        public int? hospital_nursing_diagnosis_domain_id { get; set; }
        public int? hospital_nursing_diagnosis_class_id { get; set; }
        public bool? is_re_diagnosis { get; set; }
        public bool? resolved {get; set;}
        public DateTime? resolved_date { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }        

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_DIAGNOSIS")]
    public class HospitalInpatientAdmissionOrderDiagnosis
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_diagnosis_diagnosis_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int hospital_inpatient_admission_order_id { get; set; }
        public int? diagnosis_codes_10_id { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public int? hospital_inpatient_admission_order_followup_id { get; set; }
        public bool? hospital_acquired { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public Guid? last_update_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
    }

}

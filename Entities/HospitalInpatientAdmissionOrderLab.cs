using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_LAB")]
    public class HospitalInpatientAdmissionOrderLab
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_lab_id { get; set; }
        public int hospital_inpatient_admission_order_id { get; set; }
        public int? hospital_order_test_id { get; set; }
        public bool? specimen { get; set; }
        public DateTime? specimen_collection_date { get; set; }
        public int? hospital_order_specimen_type_id { get; set; }
        public string? specimen_amount { get; set; }
        public string? specimen_storage { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public bool? deleted_flag { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public bool? completed { get; set; }
        public Guid? completed_user_id { get; set; }
        public DateTime? completed_date { get; set; }
        public string? reason_for_lab { get; set; }
        public bool? is_followup_order { get; set; }

    }
}

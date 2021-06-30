using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_RESULT")]
    public class HospitalInpatientAdmissionOrderResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_result_id { get; set; }
        public int hospital_inpatient_admission_order_id { get; set; }
        public DateTime? collected_date { get; set; }
        public DateTime? report_date { get; set; }
        public DateTime? received_date { get; set; }
        public int? cpt_codes_2015_id { get; set; }
        public string? result { get; set; }
        public string? units { get; set; }
        public int? hospital_order_result_flag_id { get; set; }
        public DateTime? result_date { get; set; }
        public int? hospital_order_result_status_id { get; set; }
        public bool? deleted_flag { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public bool? patient_is_positive { get; set; }
    }
}

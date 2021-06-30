using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_HCPCS")] 
    public class HospitalInpatientAdmissionOrderHcpcs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_hcpcs_id { get; set; }
        public int hospital_inpatient_admission_id { get; set; }
        public int hospital_inpatient_admission_order_id { get; set; }
        public int? hcpcs_codes_2015_id { get; set; }
        public bool? deleted_flag { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public int? hospital_inpatient_admission_order_followup_id { get; set; }             
    }
}

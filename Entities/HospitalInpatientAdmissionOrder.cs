using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER")]
    public class HospitalInpatientAdmissionOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int? order_number { get; set; }
        public string? accession_number { get; set; }
        public string? ordering_provider { get; set; }
        public string? vendor { get; set; }
        public string? order_note { get; set; }
        public bool? results_received { get; set; }
        public DateTime? results_received_date { get; set; }
        public bool? deleted_flag { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public int? hospital_order_type_id { get; set; }
        public Guid? ordering_provider_pcp_id { get; set; }
        public int? hospital_inpatient_admission_order_followup_id { get; set; }
        public Guid? referring_provider_pcp_id { get; set; }
        public int? referring_provider_address_id { get; set; }
    }

}

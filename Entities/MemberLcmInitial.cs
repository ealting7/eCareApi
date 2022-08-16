using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_LCM_INITIAL")] 
    public class MemberLcmInitial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lcn_case_number { get; set; }

         public Guid? member_id { get; set; }
        public DateTime lcm_open_date { get; set; }
        public DateTime? lcm_close_date { get; set; }
        public string referral_number { get; set; }
        public string cancer_related { get; set; }
        public string staging { get; set; }
        public string staging_status { get; set; }
        public string hospitalized { get; set; }
        public string hospital_five_days { get; set; }
        public int? facility_id { get; set; }
        public string facility_type { get; set; }
        public DateTime? next_report_date { get; set; }
        public Guid? system_user_id { get; set; }
        public bool? senttoadmin { get; set; }
        public bool? senttorein { get; set; }
        public bool? report_complete { get; set; }
        public bool? um_flag { get; set; }
        public bool? cm_flag { get; set; }
        public bool? lcm_flag { get; set; }
        public bool? trigger_flag { get; set; }
        public string report_type { get; set; }
        public string primary_diagnosis { get; set; }
        public string secondary_diagnosis { get; set; }
        public string other_diagnosis { get; set; }
        public string procedure { get; set; }
        public string auth_number { get; set; }
        public string tpa_name { get; set; }
        public string reinsurer_name { get; set; }
        public byte? report_billed { get; set; }
        public DateTime? report_billed_date { get; set; }
        public byte? generated_via_cm_dashboard { get; set; }
        public Guid? last_update_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
    }
}

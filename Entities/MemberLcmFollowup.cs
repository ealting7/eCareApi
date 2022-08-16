using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_LCM_FOLLOWUP")] 
    public class MemberLcmFollowup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lcm_followup_id { get; set; }

        public int lcn_case_number { get; set; }
        public DateTime lcm_followup_date { get; set; }
        public DateTime? lcm_close_date { get; set; }
        public DateTime? date_of_diagnosis { get; set; }
        public string primary_dx { get; set; }
        public string secondary_dx { get; set; }
        public string other_dx { get; set; }
        public string cancer_related { get; set; }
        public string staging { get; set; }
        public string staging_status { get; set; }
        public string hospitalized { get; set; }
        public string hospital_five_days { get; set; }
        public int? facility_id { get; set; }
        public string facility_type { get; set; }
        public string acuity { get; set; }
        public string acuity_changed { get; set; }
        public DateTime? acuity_date { get; set; }
        public DateTime? next_report_date { get; set; }
        public string current_treatments { get; set; }
        public string future_treatments { get; set; }
        public string psycho_social_summary { get; set; }
        public string nurse_summary { get; set; }
        public string physician_prognosis { get; set; }
        public string previous_treatments { get; set; }
        public Guid? system_user_id { get; set; }
        public bool? report_complete { get; set; }
        public string Procedure { get; set; }
        public int? lcm_type_id { get; set; }
        public byte? in_qa { get; set; }
        public byte? document_approved { get; set; }
        public DateTime? in_qa_date { get; set; }
        public DateTime? document_approved_date { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public string last_update_reason { get; set; }
        public int? lcm_activity_followup_id { get; set; }
        public Guid? attending_physician_pcp_id { get; set; }
        public Guid? referring_physician_pcp_id { get; set; }
        public Guid? resident_pcp_id { get; set; }
        public int? system_role_r_services_id { get; set; }
        public byte? this_month { get; set; }
        public byte? days_30 { get; set; }
        public byte? days_60 { get; set; }
        public byte? days_90 { get; set; }
        public byte? report_billed { get; set; }
        public DateTime? report_billed_date { get; set; }
        public byte? generated_via_cm_dashboard { get; set; }
        public string prognosis_report { get; set; }
        public string plan_of_care { get; set; }
        public string newly_identified_cm_summary { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
    }
}

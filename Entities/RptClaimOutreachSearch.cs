using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RPT_CLAIM_OUTREACH_SEARCH")] 
    public class RptClaimOutreachSearch
    {
        public int? report_id { get; set; }
        public int? list_order { get; set; }
        public Guid? member_id { get; set; }
        public string member_last_name { get; set; }
        public string member_first_name { get; set; }
        public string member_full_name { get; set; }
        public DateTime? member_birth { get; set; }
        public string member_ssn { get; set; }
        public string employer_name { get; set; }
        public string tpa_name { get; set; }
        public string member_in_lcm { get; set; }
        public string member_in_cm { get; set; }
        public string member_in_dm { get; set; }
        public int? tpa_medical_claims_id { get; set; }
        public string diag_1 { get; set; }
        public string diag_2 { get; set; }
        public string diag_3 { get; set; }
        public string diag_4 { get; set; }
        public string diag_5 { get; set; }
        public string claim_number { get; set; }
        public string hcpcs_code { get; set; }
        public string claim_line_number { get; set; }
        public DateTime? service_date { get; set; }
        public DateTime? check_date { get; set; }
        public DateTime? hospital_in_date { get; set; }
        public DateTime? hospital_out_date { get; set; }
        public DateTime? creation_date { get; set; }
        public DateTime? last_update_date { get; set; }
        public string provider_tin { get; set; }
        public string pos_name { get; set; }
        public string pos_phone { get; set; }
        public int? claim_paid_amount { get; set; }
        public DateTime? file_date { get; set; }
        public string cpt_code { get; set; }
        public string provider_name { get; set; }
        public string provider_phone { get; set; }
        public int? tpa_id { get; set; }
        public string condition_name { get; set; }
        public int? member_age { get; set; }
        public string diag_1_description { get; set; }
        public string diag_2_description { get; set; }
        public int? icd_10_codes { get; set; }
        public string diag_3_description { get; set; }
        public string diag_4_description { get; set; }
        public string diag_5_description { get; set; }
        public DateTime? member_death { get; set; }
        public DateTime? lcm_optout_date { get; set; }
        public DateTime? member_disenroll_date { get; set; }
        public byte? member_active { get; set; }
        public Guid? report_user_id { get; set; }
    }
}

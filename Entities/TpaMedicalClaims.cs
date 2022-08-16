using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("TPA_MEDICAL_CLAIMS")] 
    public class TpaMedicalClaims
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tpa_medical_claims_id { get; set; }

        public int? TPA_ID { get; set; }
        public string GROUPNUM { get; set; }
        public string MEM_NAME { get; set; }
        public string MEMBERID { get; set; }
        public string REL_CD { get; set; }
        public DateTime? BIRTH { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string SUB_ADD1 { get; set; }
        public string SUB_ADD2 { get; set; }
        public string SUB_ADD3 { get; set; }
        public string SUB_ZIP { get; set; }
        public string SUB_PHONE { get; set; }
        public string DIAG_1 { get; set; }
        public string DIAG_2 { get; set; }
        public string DIAG_3 { get; set; }
        public bool? MEMBER_IDENTIFIED { get; set; }
        public string DX_DESC_1 { get; set; }
        public string DX_DESC_2 { get; set; }
        public string DX_DESC_3 { get; set; }
        public decimal? CHARGES { get; set; }
        public decimal? TOTAL_PAID { get; set; }
        public DateTime? PAID_DATE { get; set; }
        public DateTime? FILE_DATE { get; set; }
        public string Gender { get; set; }
        public DateTime? CreationDate { get; set; }
        public string PLAN_NAME { get; set; }
        public string PLAN_CODE { get; set; }
        public string CPT_CODE { get; set; }
        public string REVENUE_CODE { get; set; }
        public decimal? NET_PAID_AMOUNT { get; set; }
        public DateTime? ADMIT_DOS { get; set; }
        public DateTime? DISCHARGE_DOS { get; set; }
        public string PLACE_OF_SERVICE { get; set; }
        public string PROVIDER_NAME { get; set; }
        public string PROVIDER_PHONE { get; set; }
        public string CLAIM_NUMBER { get; set; }
        public byte? needs_net_paid_amount_check { get; set; }
        public string patient_id { get; set; }
        public string hcpcs_code { get; set; }
        public string claim_line_number { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public DateTime? service_date { get; set; }
        public DateTime? check_date { get; set; }
        public DateTime? hospital_in_date { get; set; }
        public DateTime? hospital_out_date { get; set; }
        public DateTime? creation_date { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? icms_member_id { get; set; }
        public string provider_tin { get; set; }
        public string DIAG_4 { get; set; }
        public string DIAG_5 { get; set; }
        public string group_name { get; set; }
        public string pos_name { get; set; }
        public string pos_address1 { get; set; }
        public string pos_address2 { get; set; }
        public string pos_city { get; set; }
        public string pos_state { get; set; }
        public string pos_zip { get; set; }
        public string pos_phone { get; set; }
        public string employee_ssn { get; set; }
        public string claimant_ssn { get; set; }
        public decimal? claim_paid_amount { get; set; }
        public string diag_desc_1 { get; set; }
        public string cpt_desc { get; set; }
        public string SUB_CITY { get; set; }
        public string SUB_STATE { get; set; }
        public string cpt_code2 { get; set; }
        public string cpt_code3 { get; set; }
        public string cpt_code4 { get; set; }
        public string cpt_code5 { get; set; }
        public string hcpcs_code2 { get; set; }
        public string hcpcs_code3 { get; set; }
        public string medicare { get; set; }
        public string member_ssn { get; set; }
        public string patient_member_id { get; set; }
        public string service_start_date { get; set; }
        public string service_end_date { get; set; }
        public string payee_name { get; set; }
        public string provider_fed_tax_id { get; set; }
        public string provider_sub_code { get; set; }
        public string provider_reference_number { get; set; }
        public string service_address1 { get; set; }
        public string service_address2 { get; set; }
        public string service_city { get; set; }
        public string service_state { get; set; }
        public string service_zip { get; set; }
        public string provider_specialty { get; set; }
        public string patient_stay_category { get; set; }
        public string service_description { get; set; }
        public string type_of_service_code { get; set; }
        public string discharge_status { get; set; }
        public string drg { get; set; }
        public string drg_amount { get; set; }
        public string diagnosis_category { get; set; }
        public string modifier_1 { get; set; }
        public string icd_procedure_1 { get; set; }
        public string icd_procedure_2 { get; set; }
        public string icd_procedure_3 { get; set; }
        public string drug_description { get; set; }
        public string drug_identifier { get; set; }
        public string drug_class { get; set; }
        public string drug_category { get; set; }
        public string days_supply { get; set; }
        public string daw_code { get; set; }
        public string drug_type { get; set; }
        public string bi_member_id { get; set; }
        public string provider_npi { get; set; }
        public string employer_group_id { get; set; }
        public string claimant_first_name { get; set; }
        public string claimant_last_name { get; set; }
        public decimal? claim_bill_amount { get; set; }
        public decimal? claimant_paid_amount { get; set; }

    }
}

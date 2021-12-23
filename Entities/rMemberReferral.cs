using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    
    [Table("r_MEMBER_REFERRAL")]
    public class rMemberReferral
    {
        [Key]
        public int r_member_referral_id { get; set; }

        public Guid member_id { get; set; }

        [StringLength(50)]
        public string referral_number { get; set; }

        public int? priority_id { get; set; }

        public int? context_id { get; set; }

        public int? type_id { get; set; }

        public int? reason_id { get; set; }

        public Guid? referring_pcp_id { get; set; }

        public int? referring_locationpos_id { get; set; }

        public Guid? referred_to_pcp_id { get; set; }

        public int? referred_to_department_id { get; set; }

        public int? referred_to_vendor_id { get; set; }

        public int? referred_to_locationpos_id { get; set; }

        public int? decision_id { get; set; }

        [StringLength(50)]
        public string auth_number { get; set; }

        public DateTime? auth_start_date { get; set; }

        public DateTime? auth_end_date { get; set; }

        public DateTime? ibnr_pay_until_date { get; set; }

        public DateTime? bed_days_admit_date { get; set; }

        public DateTime? bed_days_discharge_date { get; set; }

        public int? bed_days_guideline_days { get; set; }

        public int? review_request_id { get; set; }

        public DateTime? created_date { get; set; }

        public int? referring_provider_address_id { get; set; }

        public int? referred_provider_address_id { get; set; }

        public int? facility_address_id { get; set; }

        public int? vendor_address_id { get; set; }

        public bool? referring_in_network { get; set; }

        public bool? referred_in_network { get; set; }

        public bool? facility_in_network { get; set; }

        public bool? vendor_in_network { get; set; }

        public int? referral_category { get; set; }

        public DateTime? referral_qa { get; set; }

        public int? referred_by_department_id { get; set; }

        [StringLength(25)]
        public string claim_number { get; set; }

        public int? facility_by_address_id { get; set; }

        public byte? referred_by_facility_in_network { get; set; }

        [StringLength(100)]
        public string po_number { get; set; }

        public int? system_role_r_category_groups_id { get; set; }

        public Guid? last_update_user_id { get; set; }

        public DateTime? last_update_date { get; set; }

        public Guid? created_user_id { get; set; }

        public DateTime? received_date { get; set; }

        public DateTime? info_requested_date { get; set; }

        public DateTime? info_received_date { get; set; }

        [StringLength(10)]
        public string revenue_code { get; set; }

        [StringLength(3)]
        public string discharge_plan_required { get; set; }

        [StringLength(3)]
        public string auth_provider_type { get; set; }

        public int? auth_service_category_type { get; set; }

        public byte? sent_in_auth_file { get; set; }

        public DateTime? sent_in_auth_file_date { get; set; }

        public DateTime? sent_in_auth_file_update_date { get; set; }

        public byte? reconsideration { get; set; }

        public byte? level_1 { get; set; }

        public byte? level_2 { get; set; }

        public byte? external_pacca { get; set; }

        public byte? level_1_upheld { get; set; }

        public byte? level_1_overturned { get; set; }

        public byte? level_2_upheld { get; set; }

        public byte? level_2_overturned { get; set; }

        public byte? no_cpt_needed { get; set; }

        public Guid? no_cpt_needed_user_id { get; set; }

        public DateTime? no_cpt_needed_update_date { get; set; }

        public int? system_role_r_clinical_service_types_id { get; set; }

        public Guid? referral_case_owner { get; set; }

        public byte? cpt_preservice_nonurgent { get; set; }

        public DateTime? cpt_preservice_nonurgent_date { get; set; }

        public Guid? cpt_preservice_nonurgent_user_id { get; set; }

        public byte? cpt_preservice_urgent { get; set; }

        public DateTime? cpt_preservice_urgent_date { get; set; }

        public Guid? cpt_preservice_urgent_user_id { get; set; }

        public byte? force_authorization_type { get; set; }

        [StringLength(5)]
        public string forced_authorization_type { get; set; }

        public byte? external_review_upheld { get; set; }

        public byte? external_review_overturned { get; set; }

        public byte? auto_status_verified { get; set; }

        public byte? discharged { get; set; }

        public int? discharge_facility_type { get; set; }

        public int? discharge_department_id { get; set; }

        public DateTime? discharge_updated_date { get; set; }

        public Guid? discharge_updated_user_id { get; set; }

        public DateTime? discharge_assessment_creation_date { get; set; }

        public Guid? discharge_assessment_creation_user_id { get; set; }

        public byte? std_billed { get; set; }

        public DateTime? std_billed_date { get; set; }

        [StringLength(50)]
        public string std_office_location { get; set; }

        public Guid? referral_qa_user_id { get; set; }

        public Guid? referral_lock_system_user_id { get; set; }

        public DateTime? referral_lock_date { get; set; }
    }

}

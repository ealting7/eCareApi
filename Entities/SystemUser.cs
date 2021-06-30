using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SYSTEMUSER")]
    public class SystemUser
    {
        public Guid system_user_id { get; set; }
        public string system_user_last_name { get; set; }
        public string system_user_first_name { get; set; }
        public string? system_user_middle_name { get; set; }
        public string? system_user_password { get; set; }
        public string system_user_login_id { get; set; }
        public DateTime date_updated { get; set; }
        public int? discipline_id { get; set; }
        public bool data_admin_flag { get; set; }
        public bool security_admin_flag { get; set; }
        public Guid? user_updated { get; set; }
        public bool user_inactive_flag { get; set; }
        public bool? user_admin_flag { get; set; }
        public bool? review_admin_flag { get; set; }
        public bool? house_admin_flag { get; set; }
        public bool billing_admin_flag { get; set; }
        public bool? contract_admin_flag { get; set; }
        public bool? employer_admin_flag { get; set; }
        public byte? review_md { get; set; }
        public byte? appeal_md { get; set; }
        public string? email_address { get; set; }
        public byte? scheduler_dm_manager { get; set; }
        public byte? scheduler_cm_manager { get; set; }
        public byte? scheduler_ls_manager { get; set; }
        public string? coaching_email_address { get; set; }
        public string? coaching_phone_number { get; set; }
        public byte qa_notes { get; set; }
        public byte qa_reports { get; set; }
        public byte lcm_report_qa { get; set; }
        public byte billing_code_setup_flag { get; set; }
        public byte? client_services_admin_flag { get; set; }
        public byte? default_missing_report_case_owner { get; set; }
        public byte? lcm_report_save_prompts { get; set; }
        public byte? member_batch_import_assigner { get; set; }
        public byte? member_batch_import { get; set; }
        public byte? md_review_company { get; set; }
        public byte? default_auto_approval_manager { get; set; }
        public byte? send_billing_refresh_email { get; set; }
        public byte? referral_qa { get; set; }
        public string? user_role_name { get; set; }
        public byte? resource_pool_flag { get; set; }
        public string? coaching_phone_ext { get; set; }
        public byte? is_wishard_dr { get; set; }
        public byte? is_shpg_dr { get; set; }
        public byte? add_code_flag { get; set; }
        public byte? case_owner_setup_flag { get; set; }
        public byte? allow_test_member_setup { get; set; }
        public byte? allow_recalculate_acuity { get; set; }
        public byte? dm_auto_letter_default_coach { get; set; }
        public byte? high_risk_code_setup { get; set; }
        public byte? referral_auto_close_email { get; set; }
        public byte? is_columbia_doctor { get; set; }
        public byte? show_in_letter_dropdown { get; set; }
        public byte? recieves_employer_no_um_email { get; set; }
        public byte? icms_user_management_admin_flag { get; set; }
        public byte? assign_online_referral_creation { get; set; }
        public byte? allow_running_um_monthly_billing_report { get; set; }
        public byte? receives_employer_creation_request_email { get; set; }
        public byte? received_lcn_trigger_creation_email { get; set; }
        public byte? allow_case_owner_assignment { get; set; }
        public byte? allow_all_fax_queues { get; set; }
        public string? fax_queue_allowed_to_see { get; set; }
        public byte? high_dollar_drug_task_default { get; set; }
        public byte? allow_unsuspend_notes { get; set; }
    }
}

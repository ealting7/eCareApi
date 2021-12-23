using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("LCM_BILLING_WORKTABLE")] 
    public class LcmBillingWorktable
    {
        [Key]
        public int lcm_billing_worktable_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid lcm_record_id { get; set; }
        public bool print_lcm { get; set; }
        public Guid? member_id { get; set; }
        public string? patient { get; set; }
        public string? memberid { get; set; }
        public string? dob { get; set; }
        public string? employer { get; set; }
        public string? tpa { get; set; }
        public string? case_manager { get; set; }
        public DateTime? record_date { get; set; }

        [Column(TypeName = "ntext")]
        public string? notes { get; set; }
        public int? time_code { get; set; }
        public double? time_length { get; set; }
        public double? lcm_rate { get; set; }
        public DateTime date_updated { get; set; }
        public Guid? user_updated { get; set; }
        public bool? disable_flag { get; set; }
        public string comments { get; set; }
        public string? LCM_Invoice_Number { get; set; }
        public bool? refreshed { get; set; }
        public DateTime? refreshed_date { get; set; }
        public Guid? refreshed_user_id { get; set; }
        public bool? keep_in_billing { get; set; }
        public Guid? updated_user_id { get; set; }
        public DateTime? updated_date { get; set; }
        public DateTime? bill_due_date { get; set; }
        public byte? sending_item { get; set; }
        public DateTime? sending_date { get; set; }
        public byte? sent_item { get; set; }
        public DateTime? sent_date { get; set; }
        public byte? um_note { get; set; }
        public string? referral_number { get; set; }
        public byte? has_activity_report { get; set; }
        public int? lcm_activity_followup_id { get; set; }
        public bool? line_item_qa { get; set; }
        public Guid? line_item_qa_user_id { get; set; }
        public DateTime? line_item_qa_date { get; set; }
    }
}

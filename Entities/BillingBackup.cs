using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("BILLING_BACKUP")] 
    public class BillingBackup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int billing_backup_id { get; set; }

        [StringLength(50)]
        public string billing_type { get; set; }

        [StringLength(255)]
        public string backup_period_id { get; set; }

        public Guid? lcm_record_id { get; set; }

        public bool? print_lcm { get; set; }

        public Guid? member_id { get; set; }

        [StringLength(200)]
        public string patient { get; set; }

        [StringLength(50)]
        public string memberid { get; set; }

        [StringLength(12)]
        public string dob { get; set; }

        [StringLength(250)]
        public string employer { get; set; }

        [StringLength(250)]
        public string tpa { get; set; }

        [StringLength(100)]
        public string case_manager { get; set; }

        public DateTime? record_date { get; set; }

        [Column(TypeName = "text")]
        public string notes { get; set; }

        public int? time_code { get; set; }

        public double? time_length { get; set; }

        public double? lcm_rate { get; set; }

        public DateTime? date_updated { get; set; }

        public Guid? user_updated { get; set; }

        public bool? disable_flag { get; set; }

        [StringLength(1024)]
        public string comments { get; set; }

        [StringLength(50)]
        public string LCM_Invoice_Number { get; set; }

        public bool? refreshed { get; set; }

        public DateTime? refreshed_date { get; set; }

        public Guid? refreshed_user_id { get; set; }

        public bool? keep_in_billing { get; set; }

        public Guid? std_record_id { get; set; }

        public bool? print_std { get; set; }

        [StringLength(50)]
        public string referral_number { get; set; }

        [StringLength(9)]
        public string member_ssn { get; set; }

        public DateTime? member_dob { get; set; }

        [StringLength(110)]
        public string member_name { get; set; }

        [StringLength(50)]
        public string member_last_name { get; set; }

        [StringLength(50)]
        public string member_first_name { get; set; }

        [StringLength(50)]
        public string employer_name { get; set; }

        [StringLength(255)]
        public string tpa_name { get; set; }

        public DateTime? auth_start_date { get; set; }

        [StringLength(50)]
        public string referral_type { get; set; }

        public double? std_rate { get; set; }

        [StringLength(50)]
        public string invoice_id { get; set; }

        public DateTime? search_start_date { get; set; }

        public DateTime? search_end_date { get; set; }

        public DateTime? creation_date { get; set; }

        public Guid? user_created { get; set; }

        [StringLength(110)]
        public string note_entered_by { get; set; }

        public Guid? cr_record_id { get; set; }

        public bool? print_cr { get; set; }

        [StringLength(255)]
        public string description { get; set; }

        public double? cr_rate { get; set; }

        [StringLength(110)]
        public string bill_entered_by { get; set; }

        public DateTime? bill_due_date { get; set; }

        public Guid? updated_user_id { get; set; }

        public DateTime? updated_date { get; set; }

        public byte? sending_item { get; set; }

        public DateTime? sending_date { get; set; }

        public byte? sent_item { get; set; }

        public DateTime? sent_date { get; set; }

        public byte? um_note { get; set; }

        public byte? has_activity_report { get; set; }

        public int? lcm_activity_followup_id { get; set; }

        public byte? reprint { get; set; }

        public int? employer_id { get; set; }

        public int? tpa_id { get; set; }

        public int? um_bill_count { get; set; }

        public Guid? um_record_id { get; set; }

        [Column(TypeName = "money")]
        public decimal? um_billing_rate { get; set; }

        public byte? auto_print { get; set; }

        public byte? possible_delete { get; set; }

        public DateTime? received_date { get; set; }

        public Guid? ccm_record_id { get; set; }

        [Column(TypeName = "money")]
        public decimal? ccm_billing_rate { get; set; }

        public int? ccm_bill_count { get; set; }

        public DateTime? auth_end_date { get; set; }

        [StringLength(50)]
        public string care_mode { get; set; }

        [StringLength(110)]
        public string case_owner_created_by { get; set; }

        public byte? use_altered_bill_amount { get; set; }

        [Column(TypeName = "money")]
        public decimal? altered_bill_amount { get; set; }

        public Guid? altered_bill_amount_user_id { get; set; }

        public DateTime? altered_bill_amount_date { get; set; }
    }
}

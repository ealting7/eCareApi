using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eCareApi.Entities
{
    [Table("r_UTILIZATION_DAYS_NOTES")] 
    public class rUtilizationDaysNotes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int r_utilization_days_notes_id { get; set; }

        public Guid member_id { get; set; }
        public string referral_number { get; set; }
        public string referral_type { get; set; }
        public int line_number { get; set; }
        public int record_seq_num { get; set; }
        public DateTime record_date { get; set; }
        public Guid system_user_id { get; set; }
        public string? evaluation_text { get; set; }
        public bool? onletter { get; set; }
        public int? RN_notes { get; set; }
        public int? billing_id { get; set; }
        public byte? note_billed { get; set; }
        public DateTime? note_billed_marked_date { get; set; }
        public byte? benefit_disclaimer { get; set; }
        public DateTime? date_lcm_report_generated { get; set; }
        public int? lcn_case_number { get; set; }
        public int? lcm_followup_id { get; set; }
        public Guid? updated_user_id { get; set; }
        public DateTime? updated_date { get; set; }
        public DateTime? date_lcm_activity_report_generated { get; set; }
        public int? lcm_activity_followup_id { get; set; }

    }
}

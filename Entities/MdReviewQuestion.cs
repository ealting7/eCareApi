using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MD_REVIEW_QUESTION")] 
    public class MdReviewQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int md_review_question_id { get; set; }
        public DateTime? record_date { get; set; }
        public int? mined_tpa_id { get; set; }
        public int? task_id { get; set; }
        public Guid? assigned_to_system_user_id { get; set; }
        public DateTime? start_action_date { get; set; }
        public DateTime? end_action_date { get; set; }
        public bool? completed { get; set; }
        public Guid? entered_by_system_user_id { get; set; }
        public DateTime? date_entered { get; set; }
        public string? task_note { get; set; }
        public DateTime? completed_date { get; set; }
        public Guid? completed_by_system_user_id { get; set; }
        public DateTime? actual_start_action_date { get; set; }
        public DateTime? actual_end_action_date { get; set; }
        public int? taskoutcome_id { get; set; }
        public bool? disabled { get; set; }
        public DateTime? creation_date { get; set; }
        public string? md_question_note { get; set; }
        public string? referral_number { get; set; }
        public Guid? member_id { get; set; }
        public string? md_answer_note { get; set; }
        public byte? user_verified { get; set; }
        public DateTime? date_verified { get; set; }
        public byte? cm_request { get; set; }
        public DateTime? cm_request_date { get; set; }
        public int? md_review_request_id { get; set; }

        public int? question_email_outbound_id { get; set; }
        public int? answer_email_outbound_id { get; set; }

    }
}

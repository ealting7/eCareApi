using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MD_REVIEW_REQUEST")] 
    public class MdReviewRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int md_review_request_id { get; set; }
         
        public DateTime? record_date { get; set; }
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
        public Guid? member_id { get; set; }
        public string? referral_number { get; set; }
        public DateTime? creation_date { get; set; }
        public string? md_review_appeal_note { get; set; }
        public byte? cm_request { get; set; }
        public DateTime? cm_request_date { get; set; }

        public int? md_review_determination_id { get; set; }

        public int? request_email_outbound_id { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_MEMBER_REFERRAL_WORKFLOW")] 
    public class rMemberReferralWorkflow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int r_member_referral_workflow_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid workflow_id { get; set; }
        public int eventtype_id { get; set; }
        public Guid member_id { get; set; }
        public string referral_number { get; set; }
        public Guid? created_by_user_id { get; set; }
        public DateTime created_date { get; set; }
        public Guid? assigned_to_user_id { get; set; }
        public DateTime? to_be_completed_date { get; set; }
        public Guid? completed_by_user_id { get; set; }
        public DateTime? completed_date { get; set; }
        public int? determination_id { get; set; }
        public bool? latest_record { get; set; }
        public int? r_workflow_xref_id { get; set; }
        public byte? do_not_show { get; set; }
        public string? referral_workflow_description { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("MEMBER_LCM_FOLLOWUP_SAVINGS")] 
    public class MemberLcmFollowupSavings
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_lcm_followup_savings_id { get; set; }

        public int? line_number { get; set; }
        public int? lcn_case_number { get; set; }
        public int? lcm_followup_id { get; set; }
        public Guid? member_id { get; set; }
        public decimal? amount { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? member_lcm_followup_savings_type_id { get; set; }
    }
}

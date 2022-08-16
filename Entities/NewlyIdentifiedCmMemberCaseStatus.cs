using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("NEWLY_IDENTIFIED_CM_MEMBER_CASE_STATUS")] 
    public class NewlyIdentifiedCmMemberCaseStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int newly_identified_cm_member_case_status_id { get; set; }
        public string description { get; set; }
        public byte? disabled { get; set; }
    }
}

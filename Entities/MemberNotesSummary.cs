using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("MEMBER_NOTES_SUMMARY")] 
    public class MemberNotesSummary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_notes_summary_id { get; set; }

        public Guid member_id { get; set; }
        public DateTime? record_date { get; set; }
        public string evaluation_text { get; set; }
        public bool? month_closed { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_system_user_id { get; set; }

    }
}

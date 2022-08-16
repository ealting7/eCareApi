using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SUSPENDED_NOTES")] 
    public class SuspendedNotes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int suspended_note_id { get; set; }

         public Guid? member_id { get; set; }
        public string? referral_number { get; set; }
        public string? note_type { get; set; }
        public string? note_text { get; set; }
        public int? RN_notes { get; set; }        
        public DateTime? creation_date { get; set; }        
        public Guid? creation_user_id { get; set; }
        public byte? mid_month_suspend { get; set; }
        public DateTime? mid_month_allow_save_date { get; set; }
        public int? billing_id { get; set; }

    }
}

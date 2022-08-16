using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_NOTES_ATTACHMENT")] 
    public class MemberNotesAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_notes_attachment_id { get; set; }

        public Guid member_id { get; set; }
        public DateTime record_date { get; set; }
        public string file_identifier { get; set; }
        public byte[]? file_blob { get; set; }
        public bool? internal_patient { get; set; }
        public bool? care_coordination { get; set; }
        public Guid? creation_user_id  { get; set; }
        public byte? entered_via_web { get; set; }

    }
}

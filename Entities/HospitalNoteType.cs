using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_NOTE_TYPE")] 
    public class HospitalNoteType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_note_type_id { get; set; }
        public string note_type_name { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_MDT_NOTE")] 
    public class HospitalInpatientAdmissionMdtNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_mdt_note { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int hospital_note_type_id { get; set; }
        public string hospital_note { get; set; }
        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }

    }
}

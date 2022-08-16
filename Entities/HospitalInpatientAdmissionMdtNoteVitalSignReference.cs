using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_MDT_NOTE_VITAL_SIGN_REFERENCE")]
    public class HospitalInpatientAdmissionMdtNoteVitalSignReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_mdt_note_vital_sign_reference_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int hosptial_inpatient_admission_mdt_note {get; set;}
        public int member_vitals_id { get; set;}
        public int hospital_inpatient_admission_chart_source_id { get; set; }

        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }

        public bool? highlight { get; set; }
        public string highlight_color { get; set; }
    }
}

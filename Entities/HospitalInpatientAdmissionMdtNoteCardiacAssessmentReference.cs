using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_MDT_NOTE_CARDIAC_ASSESSMENT_REFERENCE")] 
    public class HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_mdt_note_cardiac_assessment_reference_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int hosptial_inpatient_admission_mdt_note { get; set; }
        public int hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id { get; set; }
        public int hospital_inpatient_admission_chart_source_id { get; set; }

        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }

        public bool? highlight { get; set; }
        public string highlight_color { get; set; }
    }
}

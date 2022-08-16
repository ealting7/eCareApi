using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC")] 
    public class HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }

        public bool? is_re_assessment { get; set; }
        public bool? nail_clubbing { get; set; }
        public string nail_clubbing_description { get; set; }
        public bool? edema_symptoms { get; set; }
        public string edema_symptoms_description { get; set; }
        public bool? pulses_normal { get; set; }
        public string pulses_normal_description { get; set; }
        public bool? aortic_sound_normal { get; set; }
        public bool? pulmonic_sound_normal { get; set; }
        public bool? erb_point_sound_normal { get; set; }
        public bool? tricuspid_sound_normal { get; set; }
        public bool? apical_pulse_sound_normal { get; set; }
        public int? hospital_reassessment_timeframe_id { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public string cardiac_output { get; set; }
        public string cardiac_index { get; set; }
        public string stroke_volume { get; set; }
        public string stroke_volume_index { get; set; }

        public DateTime? date_measured { get; set; }

        public bool? highlight { get; set; }
        public string highlight_color { get; set; }

    }
}

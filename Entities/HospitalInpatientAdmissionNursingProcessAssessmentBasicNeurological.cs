using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL")] 
    public class HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id { get; set; }

         public int hospital_inpatient_admission_id { get; set; }
        public int? is_re_assessment { get; set; }
        public int? hand_squeeze_strength_id { get; set; }
        public bool? able_to_pull { get; set; }
        public bool? able_to_push { get; set; }
        public bool? able_to_answer_simple_questons { get; set; }
        public bool? able_to_wiggle_toes { get; set; }
        public bool? able_to_push_away_with_feet { get; set; }
        public bool? able_to_pull_in_with_feet { get; set; }
        public bool? pupils_dialate { get; set; }
        public bool? cardinal_field_of_vision_normal { get; set; }
        public bool? extra_ocular_movement_normal { get; set; }
        public bool? able_to_smile { get; set; }
        public bool? able_to_stick_tongue_out { get; set; }
        public bool? able_to_move_tongue { get; set; }
        public bool? able_to_puff_cheeks { get; set; }
        public bool? able_to_keep_eyes_shut { get; set; }
        public bool? able_to_raise_eyelids { get; set; }
        public bool? able_to_keep_shoulders_shrugged { get; set; }
        public int? hospital_reassessment_timeframe_id { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }

        public DateTime? date_measured { get; set; }

        public bool? highlight { get; set; }
        public string highlight_color { get; set; }

    }
}

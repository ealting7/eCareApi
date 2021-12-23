using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_GENERAL")] 
    public class HospitalInpatientAdmissionNursingProcessAssessmentBasicGeneral
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_nursing_process_assessment_basic_general_id { get; set; }
        public int hospital_inpatient_admission_id { get; set; }
        public bool? is_re_assessment { get; set; }
        public bool? answered_name_correctly { get; set; }
        public bool? answered_dob_correctly { get; set; }
        public int? hospital_chronological_development_appearance_id { get; set; }
        public int? hospital_alertness_state_id { get; set; }
        public bool? state_of_health_thin { get; set; }
        public bool? state_of_health_cachectic { get; set; }
        public bool? state_of_health_temporal_wasting { get; set; }
        public bool? state_of_health_pale { get; set; }
        public bool? state_of_health_diaphoretic { get; set; }
        public bool? state_of_health_signs_of_pain { get; set; }
        public bool? complains_of_discomfort { get; set; }
        public bool? signs_of_discomfort { get; set; }
        public int? hospital_pain_level_id { get; set; }
        public string? pain_details { get; set; }
        public int? hospital_breathing_rate_id { get; set; }
        public int? hospital_breathing_type_id { get; set; }
        public int? hospital_mental_status_id { get; set; }
        public int? hospital_reassessment_timeframe_id { get; set; }
        public string? assessment_result { get; set; }
        public bool? request_medication { get; set; }
        public bool? needs_wound_care { get; set; }
        public bool? request_lab { get; set; }
        public bool? needs_social_servie { get; set; }
        public bool? request_therapy { get; set; }
        public bool? call_dr { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }


    }
}

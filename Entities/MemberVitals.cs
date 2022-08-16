using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_VITALS")]
    public class MemberVitals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_vitals_id { get; set; }
        public Guid member_id { get; set; }
        public DateTime date_measured { get; set; }


        public int? seated_blood_pressure_systolic { get; set; }
        public int? seated_blood_pressure_diastolic { get; set; }
        public int? standing_blood_pressure_systolic { get; set; }
        public int? standing_blood_pressure_diastolic { get; set; }
        public int? pulse_per_minute { get; set; }


        public bool? pulse_is_regular { get; set; }
        public bool? respiration_is_regular { get; set; }


        public int? respiration_per_minute { get; set; }
        public decimal? height_in_inches { get; set; }
        public decimal? weight_in_pounds { get; set; }
        public decimal? temperature_in_fahrenheit { get; set; }


        public bool deleted_flag { get; set; }


        public Guid? user_deleted { get; set; }
        public DateTime? date_deleted { get; set; }
        public decimal? bmi { get; set; }


        public bool? physician_reported_flag { get; set; }


        public decimal? body_fat_percent { get; set; }
        public decimal? waist_girth { get; set; }
        public decimal? hip_girth { get; set; }
        public decimal? waist_hip_ratio { get; set; }
        public string? bp_method { get; set; }
        public string? sp02 { get; set; }
        public string? heart_rate { get; set; }
        public byte? heart_is_regular { get; set; }
        public string? richmond_rass_scale { get; set; }
        public string? ramsay_scale { get; set; }
        public string? neuro_scale { get; set; }
        public int? r_member_admission_id { get; set; }
        public Guid? web_user_id { get; set; }
        public int? glasgow_motor_response { get; set; }
        public int? glasgow_verbal_response { get; set; }
        public int? glasgow_eye_opening { get; set; }
        public string? visual_scale { get; set; }
        public string? cof { get; set; }
        public string? height_type { get; set; }
        public string? weight_type { get; set; }
        public string? temperature_type { get; set; }
        public string? note { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? hospital_inpatient_admission_id { get; set; }
        public decimal? temperature_in_celsius { get; set; }
        public int? hospital_temperature_site_id { get; set; }
        public int? hospital_respiration_regularity_id { get; set; }
        public int? hospital_respiration_depth_id { get; set; }
        public int? hospital_pulse_rhythm_id { get; set; }
        public int? hospital_pulse_intensity_id { get; set; }
        public int? hospital_pulse_position_for_reading_id { get; set; }
        public int? lying_down_blood_pressure_systolic { get; set; }
        public int? lying_down_blood_pressure_diastolic { get; set; }
        public bool? is_re_assessment { get; set; }
        public int? hospital_reassessment_timeframe_id { get; set; }
        public bool? alert_high_temperature { get; set; }
        public bool? alert_low_temperature { get; set; }
        public bool? alert_high_pulse_rate { get; set; }
        public bool? alert_low_pulse_rate { get; set; }
        public bool? alert_high_respiration_rate { get; set; }
        public bool? alert_low_respiration_rate { get; set; }
        public bool? alert_high_blood_pressure { get; set; }
        public bool? alert_low_blood_pressure { get; set; }
        public string temperature_management { get; set; }
        public int? mean_arterial_pressure { get; set; }
        public string non_invasive_blood_pressure { get; set; }
        public int? fi02 { get; set; }
        public string etc02 { get; set; }


        public bool? highlight { get; set; }
        public string highlight_color { get; set; }

        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
    }
}

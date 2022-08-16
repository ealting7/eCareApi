using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_SAFETY")] 
    public class HospitalInpatientAdmissionSafety
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_safety_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public bool? id_band { get; set; }
        public bool? bed { get; set; }
        public bool? phone { get; set; }
        public bool? air_condition { get; set; }
        public bool? no_smoking_policy { get; set; }
        public bool? visiting_hours { get; set; }
        public bool? health_record { get; set; }
        public bool? call_system { get; set; }
        public bool? lights { get; set; }
        public bool? television { get; set; }
        public bool? toilet_supplies { get; set; }
        public bool? meal_hours { get; set; }
        public bool? patient_rights { get; set; }
        public bool? valuables_nill { get; set; }
        public bool? valuables_sent_home { get; set; }
        public bool? valuable_shospital_safe { get; set; }
        public bool? has_glasses { get; set; }
        public bool? has_hearing_aid_right { get; set; }
        public bool? has_hearing_aid_left { get; set; }
        public bool? has_dentures_upper { get; set; }
        public bool? has_dentures_lower { get; set; }
        public bool? has_contact_lenses { get; set; }
        public bool? has_walking_aid { get; set; }
        public string walking_aid { get; set; }
        public DateTime? id_band__explained_date { get; set; }
        public DateTime? bed_explained_date { get; set; }
        public DateTime? phone_explained_date { get; set; }
        public DateTime? air_condition_explained_date { get; set; }
        public DateTime? no_smoking_policy_explained_date { get; set; }
        public DateTime? visitinghours_explained_date { get; set; }
        public DateTime? healthrecord_explained_date { get; set; }
        public DateTime? call_system_explained_date { get; set; }
        public DateTime? lights_explained_date { get; set; }
        public DateTime? television_explained_date { get; set; }
        public DateTime? toilet_supplies_explained_date { get; set; }
        public DateTime? meal_hours_explained_date { get; set; }
        public DateTime? patient_rights_explained_date { get; set; }
        public DateTime? valuables_nill_explained_date { get; set; }
        public DateTime? valuables_sent_home_explained_date { get; set; }
        public DateTime? valuables_hospital_safe_explained_date { get; set; }
        public DateTime? has_glasses_explained_date { get; set; }
        public DateTime? has_hearing_aid_right_explained_date { get; set; }
        public DateTime? has_hearing_aid_left_explained_date { get; set; }
        public DateTime? has_dentures_upper_explained_date { get; set; }
        public DateTime? has_dentures_lower_explained_date { get; set; }
        public DateTime? has_contact_lenses_explained_date { get; set; }
        public DateTime? has_walking_aid_explained_date { get; set; }
        public bool? disabled { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public string property_list { get; set; }


    }
}

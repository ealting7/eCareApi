using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_HEALTH_PLAN_REFERENCE")] 
    public class MemberHealthPlanReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_health_plan_reference { get; set; }

        public Guid member_id { get; set; }

        public byte? wishard_health_plan { get; set; }
        public byte? commercial { get; set; }
        public byte? mdwise_hhw { get; set; }
        public byte? mdwise_hip { get; set; }
        public byte? wishard_advantage { get; set; }
        public string mco_id_number { get; set; }
        public string mco_region_id { get; set; }
        public string recipient_id_number { get; set; }
        public string primary_medical_provider_number { get; set; }
        public string pmp_group_number { get; set; }
        public DateTime? pmp_start_date { get; set; }
        public DateTime? pmp_end_date { get; set; }
        public string capitation_category { get; set; }
        public string medicaid_eligibility { get; set; }
        public string case_number { get; set; }
        public string case_worker_number { get; set; }
        public string location_code { get; set; }
        public string delivery_system { get; set; }
        public string auto_assigned_indicator { get; set; }
        public string benefit_package_indicator { get; set; }
        public DateTime? benefit_package_date { get; set; }
        public DateTime? first_steps_start_date { get; set; }
        public DateTime? first_steps_end_date { get; set; }
        public Guid? created_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? update_user_id { get; set; }
        public string mrn { get; set; }
        public DateTime? hip_start_date { get; set; }
        public DateTime? hip_end_date { get; set; }
        public byte? right_choice { get; set; }
        public string alternate_id { get; set; }
        public string network { get; set; }
        public string enterprise { get; set; }
        public string plan_id { get; set; }
        public string plan_description { get; set; }
        public DateTime? pre_existing_eff_date { get; set; }
        public DateTime? pre_existing_term_date { get; set; }
        public byte? cobra { get; set; }
        public byte? is_sands_shpg { get; set; }
        public int? patient_acuity { get; set; }
        public int? patient_status { get; set; }
        public string tpa_name { get; set; }
        public string cm_program { get; set; }
        public string group_id { get; set; }
        public string group_location { get; set; }
        public int? primary_number { get; set; }
        public int? secondary_number { get; set; }
        public int? entry_id { get; set; }
        public Guid? user_entry_id { get; set; }
        public string id_number { get; set; }
        public string student_id { get; set; }
        public string hospital_number { get; set; }


    }
}

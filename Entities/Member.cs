using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER")]
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid member_id { get; set; }

        [Required]
        [StringLength(9)]
        public string member_ssn { get; set; }

        [StringLength(50)]
        public string member_first_name { get; set; }

        [StringLength(50)]
        public string member_middle_name { get; set; }

        [StringLength(50)]
        public string member_last_name { get; set; }

        [StringLength(20)]
        public string member_medicaid_num { get; set; }

        [StringLength(20)]
        public string member_medicare_num { get; set; }

        [StringLength(1)]
        public string gender_code { get; set; }

        [StringLength(50)]
        public string member_import_file { get; set; }

        [StringLength(255)]
        public string member_email { get; set; }

        [StringLength(25)]
        public string username { get; set; }

        [StringLength(50)]
        public string cid { get; set; }

        [StringLength(50)]
        public string maiden_name { get; set; }

        [StringLength(50)]
        public string temporal_key { get; set; }

        [StringLength(100)]
        public string network { get; set; }

        [StringLength(50)]
        public string colombia_web_usertypes_id { get; set; }

        [StringLength(25)]
        public string newly_identified_cm_member_method_of_identification { get; set; }





        public DateTime? member_updated { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? member_birth { get; set; }

        public DateTime? member_death { get; set; }

        public DateTime member_effective_date { get; set; }

        public DateTime? member_disenroll_date { get; set; }

        public DateTime? member_import_date { get; set; }

        public DateTime? dm_effective_date { get; set; }

        public DateTime? dm_disenroll_date { get; set; }

        public DateTime? cm_effective_date { get; set; }

        public DateTime? cm_disenroll_date { get; set; }

        public DateTime? lcm_effective_date { get; set; }

        public DateTime? lcm_disenroll_date { get; set; }

        public DateTime? lifestyle_effective_date { get; set; }

        public DateTime? lifestyle_disenroll_date { get; set; }

        public DateTime? careplan_effective_date { get; set; }

        public DateTime? careplan_disenroll_date { get; set; }

        public DateTime? creaion_date { get; set; }

        public DateTime? lcm_sr_effective_date { get; set; }

        public DateTime? lcm_sr_disenroll_date { get; set; }

        public DateTime? eligibility_load_date { get; set; }

        public DateTime? eligibility_load_update_date { get; set; }

        public DateTime? dm_program_flagged_by_download_date { get; set; }

        public DateTime? cm_program_flagged_by_download_date { get; set; }

        public DateTime? lifestyle_program_flagged_by_download_date { get; set; }

        public DateTime? dbms_claim_load_date { get; set; }

        public DateTime? medicare_part_a_opt_out { get; set; }

        public DateTime? medicare_part_b_opt_out { get; set; }

        public DateTime? medicare_part_d_opt_out { get; set; }

        public DateTime? medicare_part_a_opt_in { get; set; }

        public DateTime? medicare_part_b_opt_in { get; set; }

        public DateTime? medicare_part_d_opt_in { get; set; }

        public DateTime? lcm_rpt_start_date { get; set; }

        public DateTime? ccm_effective_date { get; set; }

        public DateTime? ccm_disenroll_date { get; set; }

        public DateTime? medicare_effective_date { get; set; }

        public DateTime? care_coordination_effective_date { get; set; }

        public DateTime? care_coordination_disenroll_date { get; set; }

        public DateTime? newly_identified_cm_member_date_of_referral { get; set; }





        public byte? member_status { get; set; }

        public byte? dm_program_flagged_by_download { get; set; }

        public byte? cm_program_flagged_by_download { get; set; }

        public byte? lifestyle_program_flagged_by_download { get; set; }

        public byte? dbms_claim_load { get; set; }

        public byte? uses_medicare_part_a { get; set; }

        public byte? uses_medicare_part_b { get; set; }

        public byte? uses_medicare_part_d { get; set; }

        public byte? is_wishard { get; set; }

        public byte? is_sands_shpg { get; set; }

        public byte? incentive_eligible { get; set; }

        public byte? education_only { get; set; }

        public byte? is_test_member { get; set; }

        public byte? recalculate_member_acuity { get; set; }

        public byte? lcm_rpt_none { get; set; }

        public byte? lcm_rpt_monthly { get; set; }

        public byte? lcm_rpt_bimonthly { get; set; }

        public byte? lcm_rpt_quarterly { get; set; }

        public byte? lcm_rpt_yearly { get; set; }

        public byte? is_columbia { get; set; }

        public byte? member_in_ccm { get; set; }

        public byte? patient_vip { get; set; }

        public byte? added_via_sims { get; set; }

        public byte? is_forja { get; set; }

        public byte? medicare_primary { get; set; }

        public byte? member_in_care_coordination { get; set; }






        public bool download { get; set; }

        public bool member_active_flag { get; set; }

        public bool? created_in_ICMS { get; set; }

        public bool? member_in_lcm_sr { get; set; }

        public bool? eligibility_load { get; set; }

        public bool? eligibility_load_update { get; set; }

        public bool? stop_loss { get; set; }

        public bool no_contact_phone { get; set; }

        public bool no_contact_mail { get; set; }

        public bool member_in_lcm { get; set; }

        public bool member_in_dm { get; set; }

        public bool member_in_cm { get; set; }

        public bool member_in_lifestyle { get; set; }

        public bool member_in_careplan { get; set; }





        public int? disenroll_reason_id { get; set; }

        public int? locatable_via_id { get; set; }

        public int? member_inactive_reason_id { get; set; }

        public int? relationship_id { get; set; }

        public int? legacy_id { get; set; }

        public int? JUST_ADDED { get; set; }

        public int? contact_methods_id { get; set; }

        public int? contact_time_id { get; set; }

        public int? incentive_eligible_reason_id { get; set; }

        public int? patient_types_id { get; set; }

        public int? columbia_relationships_id { get; set; }

        public int? columbia_id_type_id { get; set; }

        public int? columbia_occupations_id { get; set; }

        public int? columbia_patient_status_id { get; set; }

        public int? newly_identified_cm_member_case_status_id { get; set; }

        public int? newly_identified_cm_member_rub_id { get; set; }








        public Guid? checked_out_by { get; set; }

        public Guid? member_parent_id { get; set; }

        public Guid? creation_user_id { get; set; }

        public Guid? update_user_id { get; set; }

        public Guid? system_role_id { get; set; }

        public Guid? last_update_user_id { get; set; }

        public int? hospital_marital_status_types_id { get; set; }

        public int? languages_id { get; set; }

        public Member()
        {

            this.member_updated = null;
            this.member_birth = null;
            this.member_death = null;
            this.member_effective_date = DateTime.Now;
            this.member_disenroll_date = null;
            this.member_import_date = null;
            this.dm_effective_date = null;
            this.dm_disenroll_date = null;
            this.cm_effective_date = null;
            this.cm_disenroll_date = null;
            this.lcm_effective_date = null;
            this.lcm_disenroll_date = null;
            this.lifestyle_effective_date = null;
            this.lifestyle_disenroll_date = null;
            this.careplan_effective_date = null;
            this.careplan_disenroll_date = null;
            this.creaion_date = null;
            this.lcm_sr_effective_date = null;
            this.lcm_sr_disenroll_date = null;
            this.eligibility_load_date = null;
            this.eligibility_load_update_date = null;
            this.dm_program_flagged_by_download_date = null;
            this.cm_program_flagged_by_download_date = null;
            this.lifestyle_program_flagged_by_download_date = null;
            this.dbms_claim_load_date = null;
            this.medicare_part_a_opt_out = null;
            this.medicare_part_b_opt_out = null;
            this.medicare_part_d_opt_out = null;
            this.medicare_part_a_opt_in = null;
            this.medicare_part_b_opt_in = null;
            this.medicare_part_d_opt_in = null;
            this.lcm_rpt_start_date = null;
            this.ccm_effective_date = null;
            this.ccm_disenroll_date = null;
            this.medicare_effective_date = null;
            this.care_coordination_effective_date = null;
            this.care_coordination_disenroll_date = null;
            this.newly_identified_cm_member_date_of_referral = null;


            this.download = false;
            this.member_active_flag = true;
            this.no_contact_phone = false;
            this.no_contact_mail = false;
            this.member_in_lcm = false;
            this.member_in_dm = false;
            this.member_in_cm = false;
            this.member_in_lifestyle = false;
            this.member_in_careplan = false;
        }


    }
}

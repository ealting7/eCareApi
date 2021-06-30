using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION")] 
    public class HospitalInpatientAdmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_id { get; set; }
        public Guid member_id { get; set; }
        public int? hospital_id { get; set; }
        public string? registration_number { get; set; }
        public int? admission_diagnosis_code_id { get; set; }
        public int? hospital_department_id { get; set; }
        public int? admitting_hospital_department_provider_id { get; set; }
        public int? hospital_department_rooms_id { get; set; }
        public DateTime? registered_date { get; set; }
        public DateTime? discharged_date { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public Guid? admitting_provider_pcp_id { get; set; }
        public bool? readmission { get; set; }
        public bool? mode_of_admission_other_assisted_device { get; set; }
        public string? mode_of_admission_other_assisted_device_specify { get; set; }
        public bool? mode_of_admission_parents { get; set; }
        public bool? travel_abroad_last_3_months { get; set; }
        public string? travel_abroad_last_3_months_specify { get; set; }
        public bool? admitted_through_admission_office { get; set; }
        public bool? admitted_through_er { get; set; }
        public bool? admitted_through_transfer { get; set; }
        public int? admitted_through_transfer_department_id { get; set; }
        public string? guardian_first_name { get; set; }
        public string? guardian_last_name { get; set; }
        public string? guardian_phone_number { get; set; }
        public int? guardian_relationship_id { get; set; }
        public string? mother_blood_group { get; set; }
        public bool? mother_blood_group_verbal { get; set; }
        public bool? mother_blood_group_with_evidence { get; set; }
        public string? newborn_blood_group { get; set; }
        public bool? newborn_blood_group_verbal { get; set; }
        public bool? newborn_blood_group_with_evidence { get; set; }
        public bool? history_of_blood_reaction { get; set; }
        public bool? history_of_blood_transfusion { get; set; }
        public bool? willing_to_accept_blood { get; set; }
        public bool? deleted { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public decimal? admission_temperature { get; set; }
        public bool? admission_temperature_use_celsius { get; set; }
        public bool? mode_of_admission_wheelchair { get; set; }
        public bool? mode_of_admission_stretcher { get; set; }
        public bool? mode_of_admission_ambulatory { get; set; }


    }
}

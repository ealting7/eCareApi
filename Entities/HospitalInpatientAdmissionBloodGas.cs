using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS")]
    public class HospitalInpatientAdmissionBloodGas
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_blood_gas_id {get; set;}
        public int hospital_inpatient_admission_id { get; set; }
        public string ph { get; set; }
        public string pco2 { get; set; }
        public string po2 { get; set; }
        public string hco3 { get; set; }
        public string base_excess { get; set; }
        public string hb { get; set; }
        public string o2_saturation { get; set; }
        public string sodium { get; set; }
        public string potassium { get; set; }
        public string calcium { get; set; }
        public string blood_sugar { get; set; }
        public string lactate { get; set; }
        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public DateTime? date_measured { get; set; }

    }
}

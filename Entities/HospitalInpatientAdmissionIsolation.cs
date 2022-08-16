using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ISOLATION")] 
    public class HospitalInpatientAdmissionIsolation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_isolation_id { get; set; }

        public int hosptial_inpatient_admission_id { get; set; }
        public string goal { get; set; }
        public string plan { get; set; }
        public bool? isolate_patient { get; set; }
        public string infection_status { get; set; }

        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
    }
}

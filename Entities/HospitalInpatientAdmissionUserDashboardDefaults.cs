using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_USER_DASHBOARD_DEFAULTS")] 
    public class HospitalInpatientAdmissionUserDashboardDefaults
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_user_dashboard_default_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public Guid userId { get; set; }
        public string dashboard_button_id { get; set; }

    }
}

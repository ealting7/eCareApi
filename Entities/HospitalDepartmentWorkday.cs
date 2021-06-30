using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT_WORKDAY")]
    public class HospitalDepartmentWorkday
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_workday_id { get; set; }

        public int hospital_department_id { get; set; }

        public TimeSpan workday_start_time { get; set; }
        public TimeSpan workday_end_time { get; set; }
        public int appointment_minute_interval { get; set; }

        public TimeSpan workday_last_appointment_schedule_time { get; set; }
    }
}

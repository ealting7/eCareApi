using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT_APPOINTMENT_TYPES_DURATION_REFERENCE")] 
    public class HospitalDepartmentAppointmentTypesDurationReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_appointment_types_duration_reference_id { get; set; }
        public int? hospital_department_appointment_types_id { get; set; }
        public int? minimum_minutes { get; set; }

    }
}

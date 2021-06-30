using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT_APPOINTMENT_TYPES")] 
    public class HospitalDepartmentAppointmentTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_appointment_types_id { get; set; }
        public int? hospital_department_id { get; set; }
        public string? appointment_type_name { get; set; }
        public bool? appointment_type_disabled { get; set; }
        public int? hospital_order_test_id { get; set; }
    }
}

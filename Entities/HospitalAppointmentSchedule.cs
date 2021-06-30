using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_APPOINTMENT_SCHEDULE")] 
    public class HospitalAppointmentSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_appointment_schedule_id { get; set; }
        public Guid member_id { get; set; }
        public int? hospital_id { get; set; }
        public int? hospital_department_id { get; set; }
        public int? hospital_department_provider_id { get; set; }
        public DateTime? appointment_start_date { get; set; }
        public DateTime? appointment_end_date { get; set; }
        public int? appointment_type_id { get; set; }
        public string? note { get; set; }
        public DateTime? scheduled_on_date { get; set; }
        public Guid? scheduled_by_user_id { get; set; }
        public bool? cancelled_appointment { get; set; }
        public DateTime? cancelled_on_date { get; set; }
        public Guid? cancelled_by_user_id { get; set; }
        public bool? checked_in_appointment { get; set; }
        public DateTime? checked_in_date { get; set; }
        public Guid? checked_in_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? hospital_department_rooms_id { get; set; }
        public DateTime? assigned_to_room_date { get; set; }
        public Guid? assigned_to_room_user_id { get; set; }
        public DateTime? estimated_delivery_date { get; set; }
        public int? hospital_inpatient_admission_id { get; set; }

    }
}

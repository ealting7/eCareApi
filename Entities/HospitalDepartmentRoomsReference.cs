using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT_ROOMS_REFERENCE")] 
    public class HospitalDepartmentRoomsReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_rooms_reference_id { get; set; }

        public int? hospital_department_rooms_id { get; set; }
        public Guid? member_id { get; set; }
        public DateTime? admitted_to_room_date { get; set; }
        public Guid? admitted_by_user_id { get; set; }
    }
}

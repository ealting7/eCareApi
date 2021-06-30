using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT_ROOMS")] 
    public class HospitalDepartmentRooms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_rooms_id { get; set; }
        public int? hospital_department_id { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int? occupancy { get; set; }
        public bool? room_available { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_DEPARTMENT")] 
    public class HospitalDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_department_id { get; set; }
        public int? hospital_id { get; set; }
        public string? hospital_department_name { get; set; }
        public bool? hospital_department_disabled { get; set; }
        public DateTime? hospital_department_disabled_date { get; set; }
        public Guid? hospital_department_disabled_user_id { get; set; }
        public Guid? medical_directory_pcp_id { get; set; }
        public string? department_phone_number { get; set; }
        public string? department_extension { get; set; }
        public int? total_beds { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? nurse_director_id { get; set; }
        public bool? has_specimen_collection_capabilities { get; set; }

    }
}

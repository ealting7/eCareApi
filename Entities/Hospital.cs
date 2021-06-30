using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL")]
    public class Hospital
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_id { get; set; }
        public Guid system_role_id { get; set; }
        public string? name { get; set; }
        public DateTime? effective_date { get; set; }
        public int? termination_date { get; set; }
        public string? address1 { get; set; }
        public string? address2 { get; set; }
        public string? city { get; set; }
        public string? state_abbrev { get; set; }
        public string? zip { get; set; }
        public int? uses_hl7 { get; set; }
        public int? hospital_disabled { get; set; }
        public int? country_id { get; set; }
        public int? columbia_deptId { get; set; }
        public int? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? hospital_type_id { get; set; }
        public int? hospital_specialty_id { get; set; }
        public bool? specimen_collection_equipped { get; set; }

    }
}

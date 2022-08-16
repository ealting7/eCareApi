using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_DEPARTMENT")] 
    public class rDepartment
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string code { get; set; }
        public string label { get; set; }
        public string specialty { get; set; }
        public string tax_id { get; set; }
        public string npi { get; set; }
        public string facility_notes { get; set; }
        public bool? disable_flag { get; set; }
        public DateTime? date_updated { get; set; }
        public Guid? user_updated { get; set; }
        public string ihcp { get; set; }
        public string location_id { get; set; }
        public string billing_ihcp { get; set; }
        public string billing_npi { get; set; }
        public byte? wishard_file_load { get; set; }
        public byte? sands_file_load { get; set; }
        public int? system_user_provider_specialty_id { get; set; }
        public string shpg_facility_id { get; set; }
    }
}

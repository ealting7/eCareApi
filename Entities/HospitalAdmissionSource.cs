using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_ADMISSION_SOURCE")] 
    public class HospitalAdmissionSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_admission_source_id { get; set; }

        public string admission_source_name { get; set; }
        public string? source_code { get; set; }
        public string? description { get; set; }

    }
}

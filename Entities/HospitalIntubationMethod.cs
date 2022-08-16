using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INTUBATION_METHOD")] 
    public class HospitalIntubationMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_intubation_method_id { get; set; }
        public string intubation_method { get; set; }
        public string intubation_code { get; set; }
        public string intubation_description { get; set; }
    }
}

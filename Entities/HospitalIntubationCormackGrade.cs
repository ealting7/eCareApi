using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INTUBATION_CORMACK_GRADE")] 
    public class HospitalIntubationCormackGrade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_intubation_cormack_grade_id { get; set; }

        public string intubation_grade { get; set; }
        public string intubation_code { get; set; }
        public string intubation_description { get; set; }

    }
}

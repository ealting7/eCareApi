using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_SPECIALTY")]
    public class HospitalSpecialty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_specialty_id { get; set; }
        public string specialty_descr { get; set; }
        public bool disabled { get; set; }
        public DateTime creation_date { get; set; }
    }
}

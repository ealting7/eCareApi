using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SPECIALTYS")] 
    public class Specialtys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int specialty_id { get; set; }
        public string? specialty_code { get; set; }
        public string? specialty_desc { get; set; }
    }
}

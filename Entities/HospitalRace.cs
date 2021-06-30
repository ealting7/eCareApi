using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_RACE")]
    public class HospitalRace
    {
        [Key]
        public int hospital_race_ID { get; set; }
        public string? race_name { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_MEDICATION_FREQUENCY_ADMINISTRATION")] 
    public class HospitalMedicationFrequencyAdministration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_medication_frequency_administration_id  { get; set; }
        public string? administration_frequency { get; set; }
        public string? frequency_abbrev { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public Guid? update_user_id { get; set; }

    }
}

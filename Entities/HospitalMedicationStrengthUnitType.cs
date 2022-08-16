using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_MEDICATION_STRENGTH_UNIT_TYPE")] 
    public class HospitalMedicationStrengthUnitType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_medication_strength_unit_type_id { get; set; }
        public string strength_name { get; set; }
        public string strength_abbrev { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }

    }
}

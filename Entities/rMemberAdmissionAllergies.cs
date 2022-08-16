using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_MEMBER_ADMISSION_ALLERGIES")] 
    public class rMemberAdmissionAllergies
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int r_member_admission_allergies_id { get; set; }

        public int? r_member_admission_id { get; set; }
        public string drug_allergies { get; set; }
        public string food_allergies { get; set; }
        public string environment_allergies { get; set; }
        public string other_allergies { get; set; }

    }
}

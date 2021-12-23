using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_DIFFERENTIAL_ICD")] 
    public class MemberDiseaseConditionDifferentialIcd
    {
        [Key]
        public int member_disease_condition_differential_icd_id { get; set; }
        public int member_disease_condition_reference_id { get; set; }
        public int diagnosis_codes_10_id { get; set; }
        public Guid creation_user_id { get; set; }
        public DateTime creation_date { get; set; }

    }
}

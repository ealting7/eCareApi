using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_REFERENCE")]
    public class MemberDiseaseConditionReference
    {
        [Key]
        public int member_disease_condition_reference_id { get; set; }

        public Guid member_id { get; set; }
        public int disease_condition_id { get; set; }
        public int? diagnosis_codes_10_id { get; set; }
        public DateTime? diagnosis_date { get; set; }
        public Guid? user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public int? hospital_inpatient_admission_id { get; set; }

    }
}

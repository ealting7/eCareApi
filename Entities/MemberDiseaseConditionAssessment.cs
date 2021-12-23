using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_ASSESSMENT")] 
    public class MemberDiseaseConditionAssessment
    {
        [Key]
        public int member_disease_condition_assessment_id { get; set; }
         public int member_disease_condition_reference_id { get; set; }
        public string assessment_name { get; set; }
        public int member_disease_condition_assessment_template_id { get; set; }
        public DateTime? assessed_date { get; set; }
        public Guid? assessed_by_user_id { get; set; }
        public bool? completed { get; set; }
        public DateTime? completed_date { get; set; }
        public Guid? completed_user_id { get; set; }

        public DateTime? creation_date { get; set; }


    }
}

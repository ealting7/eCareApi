using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("MEMBER_DISEASE_CONDITION_ASSESSMENT_TEMPLATE")]
    public class MemberDiseaseConditionAssessmentTemplate
    {
        [Key]
        public int member_disease_condition_assessment_template_id { get; set; }

        public string template_name { get; set; }
        public DateTime? creation_date { get; set; }

        public int disease_condition_id { get; set; }

    }
}

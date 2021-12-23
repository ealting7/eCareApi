using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_ASSESSMENT_TEMPLATE_QUESTION")]
    public class MemberDiseaseConditionAssessmentTemplateQuestion
    {
        [Key]
        public int member_disease_condition_assessment_template_question_id { get; set; }
        public int member_disease_condition_assessment_template_id { get; set; }
        public string question { get; set; }
        public int question_order { get; set; }
        public int? parent_question_id { get; set; }

        public bool? is_episode_of_care_question { get; set; }


    }
}

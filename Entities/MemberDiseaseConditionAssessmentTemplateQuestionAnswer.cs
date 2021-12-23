using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_ASSESSMENT_TEMPLATE_QUESTION_ANSWER")] 
    public class MemberDiseaseConditionAssessmentTemplateQuestionAnswer
    {
        [Key]
        public int member_disease_condition_assessment_template_question_answer_id { get; set; }

        public int member_disease_condition_assessment_template_question_id { get; set; }
        public string control_type { get; set; }
        public string answer_type { get; set; }
        public string answer { get; set; }
        public int answer_order { get; set; }
        public string? control_attribute_1 { get; set; }
        public string? control_attribute_value_1 { get; set; }
        public string? control_attribute_2 { get; set; }
        public string? control_attribute_value_2 { get; set; }

    }
}

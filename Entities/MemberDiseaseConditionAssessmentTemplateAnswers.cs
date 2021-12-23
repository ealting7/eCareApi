using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_ASSESSMENT_TEMPLATE_ANSWERS")]
    public class MemberDiseaseConditionAssessmentTemplateAnswers
    {
        [Key]
        public int member_disease_condition_assessment_template_answers_id  {get; set;}

        public int member_disease_condition_assessment_id { get; set;}
        public int member_disease_condition_assessment_template_id { get; set;}
        public int member_disease_condition_assessment_template_question_answer_id { get; set;}
        public string item_type { get; set; }
        public bool? answer_checked { get; set; }
        public int? answer_id { get; set; }
        public DateTime? answer_date { get; set; }
        public string? answer_small { get; set; }
        public string? answer_medium { get; set; }
        public string? answer_large { get; set; }        

    }
}

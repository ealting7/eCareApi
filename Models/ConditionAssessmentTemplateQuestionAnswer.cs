using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ConditionAssessmentTemplateQuestionAnswer
    {
        public int answerId { get; set; }
        public int templateId { get; set; }
        public int questionId { get; set; }
        public string controlType { get; set; }
        public string answerType { get; set; }
        public string answerText { get; set; }
        public int answerOrder { get; set; }
        public string controlAttributeType1 { get; set; }
        public string controlAttributeValue1 { get; set; }
        public string controlAttributeType2 { get; set; }
        public string controlAttributeValue2 { get; set; }

    }
}

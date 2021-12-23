using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ConditionAssessmentTemplateQuestion
    {
        public int questionId { get; set; }
        public int templateId { get; set; }
        public string questionText { get; set; }
        public int questionOrder { get; set; }
        public int parentId { get; set; }

        public bool? isEpisode { get; set; }

        public List<ConditionAssessmentTemplateQuestionAnswer> questionAnswers { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ConditionAssessmentTemplateAnswers
    {
        public int templateAnswersId { get; set; }

        public int patientConditionAsessId { get; set; }

        public int templateId { get; set; }

        public int answerId { get; set; }

        public int questionId { get; set; }

        public string itemType { get; set; }

        public bool? itemChecked { get; set; }

        public DateTime? itemDate { get; set; }

        public int? itemId { get; set; }

        public string? itemText { get; set; }
    }
}

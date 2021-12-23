using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ConditionAssessment
    {
        public int patientConditionAsessId { get; set; }

        public int patientConditionRefId { get; set; }
        public string assessName { get; set; }

        public int templateId { get; set; }
        public string templateName { get; set; }

        public DateTime? templateCreationDate { get; set; }
        public List<ConditionAssessmentTemplateQuestion> assessmentQuestions { get; set; }

        public DateTime? assessDate { get; set; }
        public string  displayAssessDate { get; set; }
 
    }
}

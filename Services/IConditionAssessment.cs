using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;


namespace eCareApi.Services
{
    public interface IConditionAssessment
    {

        public ConditionAssessment getAssessmentTemplate(int templateId);

        public ConditionAssessmentTemplateQuestion getTemplateQuestionWithAnswers(int templateId, int questionId);
        public List<ConditionAssessmentTemplateQuestion> getAssessmentTemplateQuestionsAnswers(int templateId);
        public ConditionAssessmentTemplateQuestionAnswer getTemplateQuestionAnswer(int templateId, int questionId, int answerId);

        public List<ConditionAssessmentTemplateAnswers> getAssessmentAnswers(int assessId);



        public ConditionAssessment saveAssessmentTemplate(ConditionAssessment template);
        public ConditionAssessmentTemplateQuestion saveTemplateQuestion(ConditionAssessmentTemplateQuestion question);
        public ConditionAssessmentTemplateQuestionAnswer saveTemplateAnswer(ConditionAssessmentTemplateQuestionAnswer answer);
        public List<ConditionAssessmentTemplateQuestion> updateAssessmentAnswers(List<ConditionAssessmentTemplateAnswers> answers);
    }
}

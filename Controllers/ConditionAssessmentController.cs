using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ConditionAssessmentController : ControllerBase
    {
        private readonly IConditionAssessment _conditionAssessmentInterface;

        public ConditionAssessmentController(IConditionAssessment conditionAssessmentInterface)
        {
            _conditionAssessmentInterface = conditionAssessmentInterface ?? throw new ArgumentNullException(nameof(conditionAssessmentInterface));
        }


        [HttpGet("dbms/get/assessmenttemplates/{templateId}")]
        public IActionResult getTemplate(int templateId)
        {
            var questionsWithAnswers = _conditionAssessmentInterface.getAssessmentTemplate(templateId);

            if (questionsWithAnswers == null)
            {
                return NoContent();
            }

            return Ok(questionsWithAnswers);
        }

        [HttpGet("dbms/get/assessmenttemplates/{templateId}/questions/{questionId}")]
        public IActionResult getTemplateQuestion(int templateId, int questionId)
        {
            var questionsWithAnswers = _conditionAssessmentInterface.getTemplateQuestionWithAnswers(templateId, questionId);

            if (questionsWithAnswers == null)
            {
                return NoContent();
            }

            return Ok(questionsWithAnswers);
        }


        [HttpGet("dbms/get/assessmenttemplates/{templateId}/questions/{questionId}/answers/{answerId}")]
        public IActionResult getQuestionAnswer(int templateId, int questionId, int answerId)
        {
            var questionsWithAnswers = _conditionAssessmentInterface.getTemplateQuestionAnswer(templateId, questionId, answerId);

            if (questionsWithAnswers == null)
            {
                return NoContent();
            }

            return Ok(questionsWithAnswers);
        }



        [HttpGet("dbms/get/assessmenttemplates/{templateId}/questions")]
        public IActionResult getTemplateQuestionWithAnswers(int templateId)
        {
            var questionsWithAnswers = _conditionAssessmentInterface.getAssessmentTemplateQuestionsAnswers(templateId);

            if (questionsWithAnswers == null || questionsWithAnswers.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(questionsWithAnswers);
        }

        [HttpGet("dbms/get/assessmenttemplates/{assessId}/answers")]
        public IActionResult getAssessmentAnswers(int assessId)
        {
            var answers = _conditionAssessmentInterface.getAssessmentAnswers(assessId);

            if (answers == null || answers.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(answers);
        }




        [HttpPost("dbms/assessmenttemplates/template/name/update/")]
        public IActionResult updateAssessmentTemplateName(ConditionAssessment template)
        {
            var returnTemplate = _conditionAssessmentInterface.saveAssessmentTemplate(template);

            if (returnTemplate == null)
            {
                return NoContent();
            }

            return Ok(returnTemplate);
        }

        [HttpPost("dbms/assessmenttemplates/template/question/update/")]
        public IActionResult updateAssessmentTemplateQuestion(ConditionAssessmentTemplateQuestion question)
        {
            var returnTemplate = _conditionAssessmentInterface.saveTemplateQuestion(question);

            if (returnTemplate == null)
            {
                return NoContent();
            }

            return Ok(returnTemplate);
        }

        [HttpPost("dbms/assessmenttemplates/template/answer/update/")]
        public IActionResult updateAssessmentTemplateAnswer(ConditionAssessmentTemplateQuestionAnswer answer)
        {
            var returnAnswer = _conditionAssessmentInterface.saveTemplateAnswer(answer);

            if (returnAnswer == null)
            {
                return NoContent();
            }

            return Ok(returnAnswer);
        }
        



        [HttpPost("dbms/assessmenttemplates/update/")]
        public IActionResult updateAssessmentTemplateAnswers(List<ConditionAssessmentTemplateAnswers> answers)
        {
            var returnAssessment = _conditionAssessmentInterface.updateAssessmentAnswers(answers);

            if (returnAssessment == null)
            {
                return NoContent();
            }

            return Ok(returnAssessment);
        }

    }
}

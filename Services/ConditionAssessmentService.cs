using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace eCareApi.Services
{
    public class ConditionAssessmentService : IConditionAssessment
    {

        private readonly IcmsContext _icmsContext;

        public ConditionAssessmentService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }


        public ConditionAssessment getAssessmentTemplate(int templateId)
        {

            ConditionAssessment returnTemplate = null;

            returnTemplate = (
                from templates in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                where templates.member_disease_condition_assessment_template_id.Equals(templateId)
                select new ConditionAssessment
                {
                    templateId = templates.member_disease_condition_assessment_template_id,
                    templateName = templates.template_name,
                    templateCreationDate = templates.creation_date
                }
                )
                .FirstOrDefault();



            return returnTemplate;

        }

        public ConditionAssessmentTemplateQuestion getTemplateQuestionWithAnswers(int templateId, int questionId)
        {

            ConditionAssessmentTemplateQuestion returnQuestionWithAnswers = null;

            MemberDiseaseConditionAssessmentTemplateQuestion question = getAssessmentTemplateQuestion(questionId);

            if (question != null)
            {

                //initialize the question
                ConditionAssessmentTemplateQuestion addQuestion = initializeTemplateQuestion(question);

                //initialize the answers
                List<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> questionAnswers = getTemplateQuestionAnswers(question);
                addQuestion.questionAnswers = initializeTemplateAnswers(questionAnswers);

                //add the question/answers to the return question/answer list
                returnQuestionWithAnswers = addQuestion;

            }

            return returnQuestionWithAnswers;

        }

        public ConditionAssessmentTemplateQuestionAnswer getTemplateQuestionAnswer(int templateId, int questionId, int answerId)
        {

            ConditionAssessmentTemplateQuestionAnswer returnAnswer = null;

            MemberDiseaseConditionAssessmentTemplateQuestionAnswer answer = (
                    from memdiscondasstemquestanswr in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers
                    where memdiscondasstemquestanswr.member_disease_condition_assessment_template_question_answer_id.Equals(answerId)
                    select memdiscondasstemquestanswr
                )
                .FirstOrDefault();

            if (answer != null)
            {
                returnAnswer = new ConditionAssessmentTemplateQuestionAnswer();
                returnAnswer.answerId = answer.member_disease_condition_assessment_template_question_answer_id;
                returnAnswer.answerOrder = answer.answer_order;
                returnAnswer.answerText = answer.answer;
                returnAnswer.answerType = answer.answer_type;
                returnAnswer.controlAttributeType1 = answer.control_attribute_1;
                returnAnswer.controlAttributeType2 = answer.control_attribute_2;
                returnAnswer.controlAttributeValue1 = answer.control_attribute_value_1;
                returnAnswer.controlAttributeValue2 = answer.control_attribute_value_2;
                returnAnswer.controlType = answer.control_type;
                returnAnswer.questionId = answer.member_disease_condition_assessment_template_question_id;
                returnAnswer.templateId = templateId;
            }

            return returnAnswer;

        }


        public List<ConditionAssessmentTemplateQuestion> getAssessmentTemplateQuestionsAnswers(int templateId)
        {

            List<ConditionAssessmentTemplateQuestion> returnQuestionsWithAnswers = null;

            List<MemberDiseaseConditionAssessmentTemplateQuestion> templateQuestions = getAssessmentTemplateQuestions(templateId);


            if (templateQuestions != null && templateQuestions.Count > 0)
            {

                returnQuestionsWithAnswers = new List<ConditionAssessmentTemplateQuestion>();


                foreach (MemberDiseaseConditionAssessmentTemplateQuestion quest in templateQuestions)
                {

                    //initialize the question
                    ConditionAssessmentTemplateQuestion addQuestion = initializeTemplateQuestion(quest);

                    //initialize the answers
                    List<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> questionAnswers = getTemplateQuestionAnswers(quest);                    
                    addQuestion.questionAnswers = initializeTemplateAnswers(questionAnswers);

                    //add the question/answers to the return question/answer list
                    returnQuestionsWithAnswers.Add(addQuestion);

                }

            }

            return returnQuestionsWithAnswers;

        }                



        private List<MemberDiseaseConditionAssessmentTemplateQuestion> getAssessmentTemplateQuestions(int templateId)
        {

            List<MemberDiseaseConditionAssessmentTemplateQuestion> questions = null;

             questions = (
                            from memdiscondasstemquest in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestions
                            where memdiscondasstemquest.member_disease_condition_assessment_template_id.Equals(templateId)
                            select memdiscondasstemquest
                         )
                         .OrderBy(quest => quest.question_order)
                         .ToList();

            return questions;

        }

        private List<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> getTemplateQuestionAnswers(MemberDiseaseConditionAssessmentTemplateQuestion question)
        {
            
            List<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> answers = null;

            answers = (
    from memdiscondtempquestanswr in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers
    where memdiscondtempquestanswr.member_disease_condition_assessment_template_question_id.Equals(question.member_disease_condition_assessment_template_question_id)
    select memdiscondtempquestanswr
                      )
                      .OrderBy(ansr => ansr.answer_order)
                      .ToList();

            return answers;

        }

        private ConditionAssessmentTemplateQuestion initializeTemplateQuestion(MemberDiseaseConditionAssessmentTemplateQuestion dbQuestion)
        {
            ConditionAssessmentTemplateQuestion addQuestion = null;

            if (dbQuestion != null && 
                dbQuestion.member_disease_condition_assessment_template_question_id > 0 && 
                !string.IsNullOrEmpty(dbQuestion.question))
            {
                
                addQuestion = new ConditionAssessmentTemplateQuestion();

                addQuestion.questionId = dbQuestion.member_disease_condition_assessment_template_question_id;
                addQuestion.templateId = dbQuestion.member_disease_condition_assessment_template_id;
                addQuestion.questionText = dbQuestion.question;
                addQuestion.questionOrder = dbQuestion.question_order;
                addQuestion.parentId = dbQuestion.parent_question_id.Equals(null) ? 0 : (int)dbQuestion.parent_question_id;
                addQuestion.isEpisode = (dbQuestion.is_episode_of_care_question != null) ? dbQuestion.is_episode_of_care_question : false;

            }

            return addQuestion;

        }

        private List<ConditionAssessmentTemplateQuestionAnswer> initializeTemplateAnswers(List<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> questionAnswers)
        {

            List<ConditionAssessmentTemplateQuestionAnswer> returnAnswerList = null;

            if (questionAnswers != null)
            {

                returnAnswerList = new List<ConditionAssessmentTemplateQuestionAnswer>();

                foreach (MemberDiseaseConditionAssessmentTemplateQuestionAnswer answr in questionAnswers)
                {

                    ConditionAssessmentTemplateQuestionAnswer addAnswer = new ConditionAssessmentTemplateQuestionAnswer();

                    addAnswer.answerId = answr.member_disease_condition_assessment_template_question_answer_id;
                    addAnswer.questionId = answr.member_disease_condition_assessment_template_question_id;
                    addAnswer.answerType = answr.answer_type;
                    addAnswer.controlType = answr.control_type;
                    addAnswer.answerText = answr.answer;
                    addAnswer.answerOrder = answr.answer_order;
                    addAnswer.controlAttributeType1 = answr.control_attribute_1;
                    addAnswer.controlAttributeValue1 = answr.control_attribute_value_1;
                    addAnswer.controlAttributeType2 = answr.control_attribute_2;
                    addAnswer.controlAttributeValue2 = answr.control_attribute_value_2;

                    returnAnswerList.Add(addAnswer);

                }

            }


            return returnAnswerList;

        }

        private int getNextTemplateQuestionOrder(ConditionAssessmentTemplateQuestion question)
        {

            int nextQuestionOrder = 0;

            MemberDiseaseConditionAssessmentTemplateQuestion dbQuestion = (
                    from memdisconasstemquest in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestions
                    where memdisconasstemquest.member_disease_condition_assessment_template_id.Equals(question.templateId)
                    select memdisconasstemquest
                )
                .OrderBy(quest => quest.question_order)
                .FirstOrDefault();

            if (dbQuestion != null)
            {
                nextQuestionOrder = dbQuestion.question_order + 1;
            }
            else
            {
                nextQuestionOrder = 1;
            }

            return nextQuestionOrder;

        }

        private int getNextTemplateAnswerOrder(ConditionAssessmentTemplateQuestionAnswer answer)
        {

            int nextAnswerOrder = 0;

            MemberDiseaseConditionAssessmentTemplateQuestionAnswer dbAnswer = (
                    from memdisconasstemquest in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers
                    where memdisconasstemquest.member_disease_condition_assessment_template_question_id.Equals(answer.questionId)
                    select memdisconasstemquest
                )
                .OrderBy(answ => answ.answer_order)
                .FirstOrDefault();

            if (dbAnswer != null)
            {
                nextAnswerOrder = dbAnswer.answer_order + 1;
            }
            else
            {
                nextAnswerOrder = 1;
            }

            return nextAnswerOrder;

        }




        private MemberDiseaseConditionAssessmentTemplateQuestion getAssessmentTemplateQuestion(int questionId)
        {

            MemberDiseaseConditionAssessmentTemplateQuestion questions = null;

            questions = (
                           from memdiscondasstemquest in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestions
                           where memdiscondasstemquest.member_disease_condition_assessment_template_question_id.Equals(questionId)
                           select memdiscondasstemquest
                        )
                        .FirstOrDefault();

            return questions;

        }

        private MemberDiseaseConditionAssessmentTemplateQuestionAnswer getAssessmentQuestionAnswer(int answerId)
        {

            MemberDiseaseConditionAssessmentTemplateQuestionAnswer answer = null;

            answer = (
                        from memdiscondasstempquestanswer in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers
                        where memdiscondasstempquestanswer.member_disease_condition_assessment_template_question_answer_id.Equals(answerId)
                        select memdiscondasstempquestanswer
                    )
                    .FirstOrDefault();

            return answer;

        }





        public List<ConditionAssessmentTemplateAnswers> getAssessmentAnswers(int assessId)
        {

            List<ConditionAssessmentTemplateAnswers> returnAnswers = null;


            returnAnswers = (
                    from memdiscondasstemans in _icmsContext.MemberDiseaseConditionAssessmentTemplateAnswers

                    join questanswrs in _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers
                    on memdiscondasstemans.member_disease_condition_assessment_template_question_answer_id equals questanswrs.member_disease_condition_assessment_template_question_answer_id

                    where memdiscondasstemans.member_disease_condition_assessment_id.Equals(assessId)
                    select new ConditionAssessmentTemplateAnswers
                    {
                        templateAnswersId = memdiscondasstemans.member_disease_condition_assessment_template_answers_id,
                        questionId = questanswrs.member_disease_condition_assessment_template_question_id,
                        answerId = memdiscondasstemans.member_disease_condition_assessment_template_question_answer_id,
                        itemType = memdiscondasstemans.item_type,
                        itemChecked = (bool)memdiscondasstemans.answer_checked,
                        itemDate = (DateTime)memdiscondasstemans.answer_date,
                        itemId = (int)memdiscondasstemans.answer_id,
                        itemText = (!string.IsNullOrEmpty(memdiscondasstemans.answer_small)) ? memdiscondasstemans.answer_small : 
                                        (!string.IsNullOrEmpty(memdiscondasstemans.answer_medium)) ? memdiscondasstemans.answer_medium :
                                            (!string.IsNullOrEmpty(memdiscondasstemans.answer_large)) ? memdiscondasstemans.answer_large : null
                    }
                )
                .OrderBy(ans => ans.questionId)
                .ThenBy(ans => ans.answerId)
                .ToList();
                        

            return returnAnswers;

        }





        public ConditionAssessment saveAssessmentTemplate(ConditionAssessment template)
        {
            
            ConditionAssessment returnTemplate = null;

            if (template.templateId > 0)
            {
                returnTemplate = updateAssessmentTemplate(template);
            }
            else
            {
                returnTemplate = addAssessmentTemplate(template);
            }
            

            return returnTemplate;

        }

        public ConditionAssessmentTemplateQuestion saveTemplateQuestion(ConditionAssessmentTemplateQuestion question)
        {

            ConditionAssessmentTemplateQuestion returnQuestion = null;

            if (question.templateId > 0)
            {

                if (question.questionId > 0)
                {
                    returnQuestion = updateTemplateQuestion(question);
                }
                else
                {
                    returnQuestion = addTemplateQuestion(question);
                }                

            }

            return returnQuestion;

        }

        public ConditionAssessmentTemplateQuestionAnswer saveTemplateAnswer(ConditionAssessmentTemplateQuestionAnswer answer)
        {

            ConditionAssessmentTemplateQuestionAnswer returnQuestion = null;

            if (answer.answerId > 0)
            {
                returnQuestion = updateTemplateQuestionAnswer(answer);               
            } 
            else
            {
                returnQuestion = addTemplateQuestionAnswer(answer);
            }

            return returnQuestion;

        }



        public List<ConditionAssessmentTemplateQuestion> updateAssessmentAnswers(List<ConditionAssessmentTemplateAnswers> answers)
        {
            
            List<ConditionAssessmentTemplateQuestion> returnQuestionsAnswers = null;

            int templateId = 0;

            foreach (ConditionAssessmentTemplateAnswers questionAnswer in answers)
            {

                if (templateId.Equals(0))
                {
                    templateId = questionAnswer.templateId;
                }


                if (questionAnswer.templateAnswersId > 0)
                {
                    updateTemplateQuestionAnswer(questionAnswer);
                } 
                else
                {
                    addTemplateQuestionAnswer(questionAnswer);
                }

            }

            if (templateId > 0)
            {
                returnQuestionsAnswers = getAssessmentTemplateQuestionsAnswers(templateId);
            }


            return returnQuestionsAnswers;

        }


        private void addTemplateQuestionAnswer(ConditionAssessmentTemplateAnswers questionAnswer)
        {

            MemberDiseaseConditionAssessmentTemplateAnswers addAnswer = new MemberDiseaseConditionAssessmentTemplateAnswers();

            addAnswer.answer_checked = questionAnswer.itemChecked;
            addAnswer.answer_date = (questionAnswer.itemDate != null && !questionAnswer.itemDate.Equals(DateTime.MinValue)) ? questionAnswer.itemDate : null;
            addAnswer.answer_id = (questionAnswer.itemId > 0) ? questionAnswer.itemId : null;
            addAnswer.answer_large = (!string.IsNullOrEmpty(questionAnswer.itemText) && questionAnswer.itemText.Length > 500) ? questionAnswer.itemText : null;
            addAnswer.answer_medium = (!string.IsNullOrEmpty(questionAnswer.itemText) && (questionAnswer.itemText.Length > 50 && questionAnswer.itemText.Length <= 500)) ? questionAnswer.itemText : null;
            addAnswer.answer_small = (!string.IsNullOrEmpty(questionAnswer.itemText) && questionAnswer.itemText.Length <= 50) ? questionAnswer.itemText : null;
            addAnswer.item_type = questionAnswer.itemType;
            addAnswer.member_disease_condition_assessment_id = questionAnswer.patientConditionAsessId;
            addAnswer.member_disease_condition_assessment_template_id = questionAnswer.templateId;
            addAnswer.member_disease_condition_assessment_template_question_answer_id = questionAnswer.answerId;

            _icmsContext.MemberDiseaseConditionAssessmentTemplateAnswers.Add(addAnswer);
            int result = _icmsContext.SaveChanges();

        }
        private void updateTemplateQuestionAnswer(ConditionAssessmentTemplateAnswers questionAnswer)
        {

            MemberDiseaseConditionAssessmentTemplateAnswers updateAnswer = null;

            updateAnswer = (
                from memdiscondasstempanswers in _icmsContext.MemberDiseaseConditionAssessmentTemplateAnswers
                where memdiscondasstempanswers.member_disease_condition_assessment_template_answers_id.Equals(questionAnswer.templateAnswersId)
                select memdiscondasstempanswers
            )
            .FirstOrDefault();


            if (updateAnswer != null)
            {

                updateAnswer.answer_checked = questionAnswer.itemChecked;
                updateAnswer.answer_date = (questionAnswer.itemDate != null && !questionAnswer.itemDate.Equals(DateTime.MinValue)) ? questionAnswer.itemDate : null;
                updateAnswer.answer_id = (questionAnswer.answerId > 0) ? questionAnswer.answerId : 0;
                updateAnswer.answer_large = (!string.IsNullOrEmpty(questionAnswer.itemText) && questionAnswer.itemText.Length > 500) ? questionAnswer.itemText : null;
                updateAnswer.answer_medium = (!string.IsNullOrEmpty(questionAnswer.itemText) && (questionAnswer.itemText.Length > 50 && questionAnswer.itemText.Length <= 500)) ? questionAnswer.itemText : null;
                updateAnswer.answer_small = (!string.IsNullOrEmpty(questionAnswer.itemText) && questionAnswer.itemText.Length <= 50) ? questionAnswer.itemText : null;
                updateAnswer.item_type = questionAnswer.itemType;
                updateAnswer.member_disease_condition_assessment_id = questionAnswer.patientConditionAsessId;
                updateAnswer.member_disease_condition_assessment_template_id = questionAnswer.templateId;
                updateAnswer.member_disease_condition_assessment_template_question_answer_id = questionAnswer.templateAnswersId;

                _icmsContext.MemberDiseaseConditionAssessmentTemplateAnswers.Update(updateAnswer);
                int result = _icmsContext.SaveChanges();

            }

        }


        private ConditionAssessment addAssessmentTemplate(ConditionAssessment template)
        {
            
            ConditionAssessment returnTemplate = null;

            MemberDiseaseConditionAssessmentTemplate addTemplate = new MemberDiseaseConditionAssessmentTemplate();

            addTemplate.creation_date = DateTime.Now;
            addTemplate.disease_condition_id = template.patientConditionRefId;
            addTemplate.member_disease_condition_assessment_template_id = template.templateId;
            addTemplate.template_name = template.templateName;

            _icmsContext.MemberDiseaseConditionAssessmentTemplates.Add(addTemplate);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                returnTemplate = getAssessmentTemplate(template.templateId);
            }

            return returnTemplate;

        }
        private ConditionAssessment updateAssessmentTemplate(ConditionAssessment template)
        {

            ConditionAssessment returnTemplate = null;

            MemberDiseaseConditionAssessmentTemplate dbTemplate = (
                                from memdiscondasstempl in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                                where memdiscondasstempl.member_disease_condition_assessment_template_id.Equals(template.templateId)
                                select memdiscondasstempl
                            )
                            .FirstOrDefault();

            if (dbTemplate != null)
            {
                dbTemplate.template_name = template.templateName;

                _icmsContext.MemberDiseaseConditionAssessmentTemplates.Update(dbTemplate);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnTemplate = getAssessmentTemplate(template.templateId);
                }
            }

            return returnTemplate;

        }


        private ConditionAssessmentTemplateQuestion addTemplateQuestion(ConditionAssessmentTemplateQuestion question)
        {

            ConditionAssessmentTemplateQuestion returnQuestion = null;

            MemberDiseaseConditionAssessmentTemplateQuestion addQuestion = new MemberDiseaseConditionAssessmentTemplateQuestion();

            addQuestion.is_episode_of_care_question = question.isEpisode;
            addQuestion.member_disease_condition_assessment_template_id = question.templateId;
            addQuestion.parent_question_id = question.parentId;
            addQuestion.question = question.questionText;
            addQuestion.question_order = getNextTemplateQuestionOrder(question);

            _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestions.Add(addQuestion);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                returnQuestion = getTemplateQuestionWithAnswers(question.templateId, addQuestion.member_disease_condition_assessment_template_question_id);
            }

            return returnQuestion;

        }
        private ConditionAssessmentTemplateQuestion updateTemplateQuestion(ConditionAssessmentTemplateQuestion question)
        {

            ConditionAssessmentTemplateQuestion returnQuestion = null;

            MemberDiseaseConditionAssessmentTemplateQuestion dbQuestion = getAssessmentTemplateQuestion(question.questionId);

            if (dbQuestion != null)
            {

                dbQuestion.question = question.questionText;
                dbQuestion.is_episode_of_care_question = question.isEpisode;

                _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestions.Update(dbQuestion);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnQuestion = getTemplateQuestionWithAnswers(question.templateId, question.questionId);
                }

            }

            return returnQuestion;

        }


        private ConditionAssessmentTemplateQuestionAnswer addTemplateQuestionAnswer(ConditionAssessmentTemplateQuestionAnswer answer)
        {

            ConditionAssessmentTemplateQuestionAnswer returnAnswer = null;

            MemberDiseaseConditionAssessmentTemplateQuestionAnswer addAnswer = new MemberDiseaseConditionAssessmentTemplateQuestionAnswer();

            addAnswer.answer = answer.answerText;
            addAnswer.answer_order = getNextTemplateAnswerOrder(answer);
            addAnswer.answer_type = answer.answerType;
            addAnswer.control_attribute_1 = answer.controlAttributeType1;
            addAnswer.control_attribute_2 = answer.controlAttributeType2;
            addAnswer.control_attribute_value_1 = answer.controlAttributeValue1;
            addAnswer.control_attribute_value_2 = answer.controlAttributeValue2;
            addAnswer.control_type = answer.controlType;
            addAnswer.member_disease_condition_assessment_template_question_id = answer.questionId;

            _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers.Add(addAnswer);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                returnAnswer = getTemplateQuestionAnswer(answer.templateId, answer.questionId, addAnswer.member_disease_condition_assessment_template_question_answer_id);
            }

            return returnAnswer;

        }
        private ConditionAssessmentTemplateQuestionAnswer updateTemplateQuestionAnswer(ConditionAssessmentTemplateQuestionAnswer answer)
        {

            ConditionAssessmentTemplateQuestionAnswer returnQuestion = null;

            MemberDiseaseConditionAssessmentTemplateQuestionAnswer dbAnswer = getAssessmentQuestionAnswer(answer.answerId);

            if (dbAnswer != null)
            {

                dbAnswer.answer = answer.answerText;
                dbAnswer.answer_type = answer.answerType;
                dbAnswer.control_attribute_1 = answer.controlAttributeType1;
                dbAnswer.control_attribute_2 = answer.controlAttributeType2;
                dbAnswer.control_attribute_value_1 = answer.controlAttributeValue1;
                dbAnswer.control_attribute_value_2 = answer.controlAttributeValue2;
                dbAnswer.control_type = answer.controlType;

                _icmsContext.MemberDiseaseConditionAssessmentTemplateQuestionAnswers.Update(dbAnswer);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnQuestion = getTemplateQuestionAnswer(answer.templateId, answer.questionId, answer.answerId);
                }

            }

            return returnQuestion;
        }
        

    }
    
}

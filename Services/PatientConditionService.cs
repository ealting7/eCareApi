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
    public class PatientConditionService : IPatientCondition
    {

        private readonly IcmsContext _icmsContext;

        public PatientConditionService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }

        public List<Condition> getConditions()
        {
            List<Condition> conditionsReturned = new List<Condition>();

            List<DiseaseCondition> conditionsList = new List<DiseaseCondition>();

            conditionsList = (
                            from discond in _icmsContext.DiseaseConditions
                            where discond.disable_flag.Equals(false)
                            select discond
                         )
                         .Distinct()
                         .OrderBy(cond => cond.descr)
                         .ToList();


            if (conditionsList != null && conditionsList.Count > 0)
            {
                foreach(DiseaseCondition condition in conditionsList)
                {
                    
                    Condition addCondition = new Condition();
                    addCondition.conditionId = condition.disease_condition_id;
                    addCondition.conditionName = condition.descr;

                    conditionsReturned.Add(addCondition);

                }
            }


            return conditionsReturned;

        }

        public List<ConditionAssessment> getAssessmentTemplates(string conditionId)
        {
            List<ConditionAssessment> TemplatesReturned = new List<ConditionAssessment>();

            int condition = 0;

            if (!string.IsNullOrEmpty(conditionId) && int.TryParse(conditionId, out condition))
            {

                List<MemberDiseaseConditionAssessmentTemplate> templateList = new List<MemberDiseaseConditionAssessmentTemplate>();

                templateList = (
                                    from temps in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                                    where temps.disease_condition_id.Equals(condition)
                                    select temps
                               )
                               .Distinct()
                               .OrderBy(template => template.template_name)
                               .ToList();


                if (templateList != null && templateList.Count > 0)
                {
                    foreach (MemberDiseaseConditionAssessmentTemplate temps in templateList)
                    {

                        ConditionAssessment addTemplate = new ConditionAssessment();
                        addTemplate.templateId = temps.member_disease_condition_assessment_template_id;
                        addTemplate.templateName = temps.template_name;
                        addTemplate.templateCreationDate = temps.creation_date;

                        TemplatesReturned.Add(addTemplate);

                    }
                }

            }


            return TemplatesReturned;

        }

        public ConditionAssessment getAssessmentTemplate(string assessid)
        {
            ConditionAssessment TemplatesReturned = null;

            int assessmentId = 0;

            if (!string.IsNullOrEmpty(assessid) && int.TryParse(assessid, out assessmentId))
            {

                TemplatesReturned = (
                                        from assess in _icmsContext.MemberDiseaseConditionAssessments

                                        join assessTemp in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                                        on assess.member_disease_condition_assessment_template_id equals assessTemp.member_disease_condition_assessment_template_id into memTemps
                                        from membertemplate in memTemps.DefaultIfEmpty()

                                        where assess.member_disease_condition_assessment_id.Equals(assessmentId)
                                        select new ConditionAssessment
                                        {
                                            patientConditionAsessId = assess.member_disease_condition_assessment_id,
                                            patientConditionRefId = assess.member_disease_condition_reference_id,
                                            templateId = assess.member_disease_condition_assessment_template_id,
                                            templateName = membertemplate.template_name
                                        }
                                     )
                                     .FirstOrDefault();


            }


            return TemplatesReturned;

        }


        public List<Condition> getPatientMemberConditionReferences(string id)
        {
            List<Condition> conditionsReturned = new List<Condition>();

            Guid memberId = Guid.Empty;

            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out memberId))
            {
                List<Condition> conditionList = (
                                                    from memdiscondref in _icmsContext.MemberDiseaseConditionReferences

                                                    join discond in _icmsContext.DiseaseConditions
                                                    on memdiscondref.disease_condition_id equals discond.disease_condition_id into diseaseCond
                                                    from disCondition in diseaseCond.DefaultIfEmpty()   
                                                    
                                                    join diag in _icmsContext.DiagnosisCodes10
                                                    on memdiscondref.diagnosis_codes_10_id equals diag.diagnosis_codes_10_id into diag10
                                                    from diags in diag10.DefaultIfEmpty()

                                                    where memdiscondref.member_id.Equals(memberId)
                                                    select new Condition
                                                    {
                                                        patientConditionRefId = memdiscondref.member_disease_condition_reference_id,
                                                        conditionId = memdiscondref.disease_condition_id,
                                                        hospitalInpatientAdmissionId = memdiscondref.hospital_inpatient_admission_id,
                                                        conditionName = disCondition.descr,
                                                        diagnosisDate = memdiscondref.diagnosis_date,
                                                        displayDiagnosisDate = memdiscondref.diagnosis_date.Value.ToShortDateString(),
                                                        diagnosisCodes10Id = memdiscondref.diagnosis_codes_10_id,
                                                        diagnosisCode = diags.diagnosis_code,
                                                        diagnosisCodeDesc = diags.short_description
                                                    }
                                                ).ToList();


                if (conditionList != null && conditionList.Count > 0)
                {
                    foreach (Condition condition in conditionList)
                    {

                        Condition addCondition = new Condition();
                        addCondition.patientConditionRefId = condition.patientConditionRefId;
                        addCondition.hospitalInpatientAdmissionId = condition.hospitalInpatientAdmissionId;
                        addCondition.registrationNumber = condition.registrationNumber;
                        addCondition.conditionId = condition.conditionId;
                        addCondition.conditionName = condition.conditionName;
                        addCondition.diagnosisDate = condition.diagnosisDate;
                        addCondition.displayDiagnosisDate = condition.displayDiagnosisDate;
                        addCondition.diagnosisCodes10Id = condition.diagnosisCodes10Id;
                        addCondition.diagnosisCode = condition.diagnosisCode;
                        addCondition.diagnosisCodeDesc = condition.diagnosisCodeDesc;

                        conditionsReturned.Add(addCondition);

                    }
                }

            }


            return conditionsReturned;

        }        

        public Condition getPatientMemberConditionReference(string id, string condid)
        {
            Condition conditionReturned = null;

            Guid memberId = Guid.Empty;
            int memCondRefId = 0;

            if ((!string.IsNullOrEmpty(id) && Guid.TryParse(id, out memberId)) &&
                (!string.IsNullOrEmpty(condid) && int.TryParse(condid, out memCondRefId)))
            {
                Condition condition = (
                                        from memdiscondref in _icmsContext.MemberDiseaseConditionReferences

                                        join discond in _icmsContext.DiseaseConditions
                                        on memdiscondref.disease_condition_id equals discond.disease_condition_id into diseaseCond
                                        from disCondition in diseaseCond.DefaultIfEmpty()

                                        join diag in _icmsContext.DiagnosisCodes10
                                        on memdiscondref.diagnosis_codes_10_id equals diag.diagnosis_codes_10_id into diag10
                                        from diags in diag10.DefaultIfEmpty()

                                        join admit in _icmsContext.HospitalInpatientAdmissions
                                        on memdiscondref.hospital_inpatient_admission_id equals admit.hospital_inpatient_admission_id into admits
                                        from inptadmit in admits.DefaultIfEmpty()

                                        where memdiscondref.member_disease_condition_reference_id.Equals(memCondRefId)
                                        select new Condition
                                        {
                                            patientConditionRefId = memdiscondref.member_disease_condition_reference_id,
                                            patientId = memdiscondref.member_id,
                                            conditionId = memdiscondref.disease_condition_id,
                                            hospitalInpatientAdmissionId = memdiscondref.hospital_inpatient_admission_id,
                                            registrationNumber = inptadmit.registration_number,
                                            conditionName = disCondition.descr,
                                            diagnosisDate = memdiscondref.diagnosis_date,
                                            displayDiagnosisDate = memdiscondref.diagnosis_date.Value.ToShortDateString(),
                                            diagnosisCodes10Id = memdiscondref.diagnosis_codes_10_id,
                                            diagnosisCode = diags.diagnosis_code,
                                            diagnosisCodeDesc = !string.IsNullOrEmpty(diags.medium_description) ? diags.medium_description : diags.short_description
                                        }
                                    ).FirstOrDefault();


                if (condition != null)
                {
                    conditionReturned = new Condition();
                    conditionReturned.patientConditionRefId = condition.patientConditionRefId;
                    conditionReturned.patientId = condition.patientId;
                    conditionReturned.hospitalInpatientAdmissionId = condition.hospitalInpatientAdmissionId;
                    conditionReturned.registrationNumber = condition.registrationNumber;
                    conditionReturned.conditionId = condition.conditionId;
                    conditionReturned.conditionName = condition.conditionName;
                    conditionReturned.diagnosisDate = condition.diagnosisDate;
                    conditionReturned.displayDiagnosisDate = condition.displayDiagnosisDate;
                    conditionReturned.diagnosisCodes10Id = condition.diagnosisCodes10Id;
                    conditionReturned.diagnosisCode = condition.diagnosisCode;
                    conditionReturned.diagnosisCodeDesc = condition.diagnosisCodeDesc;
                }

            }


            return conditionReturned;

        }

        public Condition getConditionDifferentialIcds(string condid)
        {
            Condition returnCondition = null;

            int memDisCondRefId = 0;

            if (int.TryParse(condid, out memDisCondRefId))
            {

                List<MedicalCode> differentialIcds = (
                                                        from conddifficd in _icmsContext.MemberDiseaseConditionDifferentialIcds

                                                        join icd10 in _icmsContext.DiagnosisCodes10
                                                        on conddifficd.diagnosis_codes_10_id equals icd10.diagnosis_codes_10_id into icd10s
                                                        from diagcodes in icd10s.DefaultIfEmpty()

                                                        where conddifficd.member_disease_condition_reference_id.Equals(memDisCondRefId)
                                                        select new MedicalCode
                                                        {
                                                            CodeId = diagcodes.diagnosis_codes_10_id,
                                                            Code = diagcodes.diagnosis_code,
                                                            DisplayDescription = !string.IsNullOrEmpty(diagcodes.medium_description) ? diagcodes.medium_description : diagcodes.short_description,
                                                            LongDescription = diagcodes.long_description,
                                                            MediumDescription = diagcodes.medium_description,
                                                            ShortDescription = diagcodes.short_description
                                                        }
                                                     ).ToList();


                returnCondition = new Condition();
                returnCondition.patientConditionRefId = memDisCondRefId;

                if (differentialIcds != null)
                {                 
                    returnCondition.differentialIcds = differentialIcds;
                }

            }

            return returnCondition;
        }

        public Condition updateMemberConditionReference(Condition condition)
        {

            Condition returnCondition = null;

            if (condition.patientConditionRefId > 0)
            {
                returnCondition = UpdateExistingMemberConditionReference(condition);
            } 
            else
            {
                returnCondition= AddNewMemberConditionReference(condition);
            }
            
            return returnCondition;

        }

        public Condition AddNewMemberConditionReference(Condition condition)
        {

            Condition returnCondition = null;

            MemberDiseaseConditionReference memDisCondRef = new MemberDiseaseConditionReference();
            memDisCondRef.member_id = condition.patientId;
            memDisCondRef.hospital_inpatient_admission_id = condition.hospitalInpatientAdmissionId;
            memDisCondRef.disease_condition_id = condition.conditionId;

            if (condition.diagnosisCodes10Id > 0 && !string.IsNullOrEmpty(condition.displayDiagnosisDate))
            {
                DateTime diagDate = DateTime.MinValue;

                if (DateTime.TryParse(condition.displayDiagnosisDate, out diagDate))
                {
                    memDisCondRef.diagnosis_codes_10_id = condition.diagnosisCodes10Id;
                    memDisCondRef.diagnosis_date = diagDate;
                }

            }

            memDisCondRef.creation_date = DateTime.Now;
            memDisCondRef.user_id = Guid.Empty;

            if (memDisCondRef != null)
            {

                _icmsContext.MemberDiseaseConditionReferences.Update(memDisCondRef);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnCondition = new Condition();
                    returnCondition.patientConditionRefId = memDisCondRef.member_disease_condition_reference_id;
                    returnCondition.patientId = memDisCondRef.member_id;
                    returnCondition.hospitalInpatientAdmissionId = memDisCondRef.hospital_inpatient_admission_id;
                    returnCondition.conditionId = memDisCondRef.disease_condition_id;
                    returnCondition.diagnosisCodes10Id = memDisCondRef.diagnosis_codes_10_id;
                    returnCondition.diagnosisDate = memDisCondRef.diagnosis_date;

                }

            }

            return returnCondition;

        }

        public Condition UpdateExistingMemberConditionReference(Condition condition)
        {

            Condition returnCondition = null;

            MemberDiseaseConditionReference memDisCondRef = (
                                                                from memdiscondrefer in _icmsContext.MemberDiseaseConditionReferences
                                                                where memdiscondrefer.member_disease_condition_reference_id.Equals(condition.patientConditionRefId)
                                                                select memdiscondrefer
                                                             )
                                                             .FirstOrDefault();

            if (memDisCondRef != null)
            {

                memDisCondRef.hospital_inpatient_admission_id = condition.hospitalInpatientAdmissionId;
                memDisCondRef.disease_condition_id = condition.conditionId;
                memDisCondRef.diagnosis_codes_10_id = condition.diagnosisCodes10Id;

                if (!string.IsNullOrEmpty(condition.displayDiagnosisDate))
                {
                    DateTime diagDate = DateTime.MinValue;

                    if (DateTime.TryParse(condition.displayDiagnosisDate, out diagDate))
                    {
                        memDisCondRef.diagnosis_date = diagDate;
                    }

                }
                else
                {
                    memDisCondRef.diagnosis_date = null;
                }


                _icmsContext.MemberDiseaseConditionReferences.Update(memDisCondRef);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnCondition = new Condition();
                    returnCondition.patientConditionRefId = memDisCondRef.member_disease_condition_reference_id;
                    returnCondition.patientId = memDisCondRef.member_id;
                    returnCondition.hospitalInpatientAdmissionId = memDisCondRef.hospital_inpatient_admission_id;
                    returnCondition.conditionId = memDisCondRef.disease_condition_id;
                    returnCondition.diagnosisCodes10Id = memDisCondRef.diagnosis_codes_10_id;
                    returnCondition.diagnosisDate = memDisCondRef.diagnosis_date;

                }

            }


            return returnCondition;

        }



        public Condition updateMemberConditionReferenceIcd(Condition condition)
        {

            Condition returnCondition = null;

            MemberDiseaseConditionReference memDisCondRef = (
                                                                from memdiscondrefer in _icmsContext.MemberDiseaseConditionReferences
                                                                where memdiscondrefer.member_disease_condition_reference_id.Equals(condition.patientConditionRefId)
                                                                select memdiscondrefer
                                                             )
                                                             .FirstOrDefault();

            if (memDisCondRef != null)
            {

                if (condition.diagnosisCodes10Id > 0)
                {
                    memDisCondRef.diagnosis_codes_10_id = condition.diagnosisCodes10Id;
                }
                else
                {
                    memDisCondRef.diagnosis_codes_10_id = null;
                }


                if (!string.IsNullOrEmpty(condition.displayDiagnosisDate))
                {
                    DateTime diagDate = DateTime.MinValue;

                    if (DateTime.TryParse(condition.displayDiagnosisDate, out diagDate))
                    {
                        memDisCondRef.diagnosis_date = diagDate;
                    }

                }
                else
                {
                    memDisCondRef.diagnosis_date = null;
                }


                _icmsContext.MemberDiseaseConditionReferences.Update(memDisCondRef);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnCondition = new Condition();
                    returnCondition.patientConditionRefId = memDisCondRef.member_disease_condition_reference_id;
                    returnCondition.patientId = memDisCondRef.member_id;
                    returnCondition.conditionId = memDisCondRef.disease_condition_id;
                    returnCondition.diagnosisCodes10Id = memDisCondRef.diagnosis_codes_10_id;
                    returnCondition.diagnosisDate = memDisCondRef.diagnosis_date;

                }

            }


            return returnCondition;

        }


        public Condition updateMemberConditionDifferentialIcd(Condition condition)
        {

            Condition returnCondition = null;

            List<MemberDiseaseConditionDifferentialIcd> memDisCondDiffIcds = (
                                                                                from memdisconddifficds in _icmsContext.MemberDiseaseConditionDifferentialIcds
                                                                                where memdisconddifficds.member_disease_condition_reference_id.Equals(condition.patientConditionRefId)
                                                                                select memdisconddifficds
                                                                             ).ToList();

            if (memDisCondDiffIcds != null && memDisCondDiffIcds.Count > 0)
            {

                foreach (MemberDiseaseConditionDifferentialIcd icd in memDisCondDiffIcds)
                {
                    if (icd.diagnosis_codes_10_id.Equals(condition.diagnosisCodes10Id))
                    {
                        break;
                    }
                }


                returnCondition = addMemberConditionDifferentialIcd(condition);                

            }
            else
            {
                returnCondition = addMemberConditionDifferentialIcd(condition);
            }


            return returnCondition;

        }

        public Condition addMemberConditionDifferentialIcd(Condition condition)
        {
            Condition returnCondition = null;

            MemberDiseaseConditionDifferentialIcd addDifferential = new MemberDiseaseConditionDifferentialIcd();
            addDifferential.member_disease_condition_reference_id = condition.patientConditionRefId;
            addDifferential.diagnosis_codes_10_id = (int)condition.diagnosisCodes10Id;
            addDifferential.creation_date = DateTime.Now;
            addDifferential.creation_user_id = Guid.Empty;

            _icmsContext.MemberDiseaseConditionDifferentialIcds.Add(addDifferential);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                returnCondition = new Condition();
                returnCondition = getConditionDifferentialIcds(condition.patientConditionRefId.ToString());

            }

            return returnCondition;
        }

        public Condition removeMemberConditionDifferentialIcd(Condition condition)
        {
            Condition returnCondition = null;

            if (condition.patientConditionRefId > 0)
            {
                MemberDiseaseConditionDifferentialIcd removeDiffIcd = (
                                                                        from memdisconddiff in _icmsContext.MemberDiseaseConditionDifferentialIcds
                                                                        where memdisconddiff.member_disease_condition_reference_id.Equals(condition.patientConditionRefId)
                                                                        && memdisconddiff.diagnosis_codes_10_id.Equals(condition.diagnosisCodes10Id)
                                                                        select memdisconddiff
                                                                      )
                                                                      .FirstOrDefault();

                if (removeDiffIcd != null)
                {
                    _icmsContext.MemberDiseaseConditionDifferentialIcds.Remove(removeDiffIcd);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        returnCondition = new Condition();
                        returnCondition = getConditionDifferentialIcds(condition.patientConditionRefId.ToString());
                    }

                }
            }


            return returnCondition;
        }





        public List<ConditionAssessment> getPatientMemberConditionAssessments(string id, string condid)
        {
            List<ConditionAssessment> assessmentsReturned = new List<ConditionAssessment>();

            Guid memberId = Guid.Empty;
            int memCondRefId = 0;

            if ((!string.IsNullOrEmpty(id) && Guid.TryParse(id, out memberId)) &&
                (!string.IsNullOrEmpty(condid) && int.TryParse(condid, out memCondRefId)))
            {
                List<ConditionAssessment> assessmentList = (
                                                                from memdiscondass in _icmsContext.MemberDiseaseConditionAssessments

                                                                join memdiscondref in _icmsContext.MemberDiseaseConditionReferences
                                                                on memdiscondass.member_disease_condition_reference_id equals memdiscondref.member_disease_condition_reference_id

                                                                join templ in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                                                                on memdiscondass.member_disease_condition_assessment_template_id equals templ.member_disease_condition_assessment_template_id into assessTemp
                                                                from assmtTemp in assessTemp.DefaultIfEmpty()

                                                                where memdiscondass.member_disease_condition_reference_id.Equals(memCondRefId)
                                                                && memdiscondref.member_id.Equals(memberId)
                                                                select new ConditionAssessment
                                                                {
                                                                    patientConditionAsessId = memdiscondass.member_disease_condition_assessment_id,
                                                                    patientConditionRefId = memdiscondass.member_disease_condition_reference_id,
                                                                    assessName = memdiscondass.assessment_name,
                                                                    templateId = memdiscondass.member_disease_condition_assessment_template_id,
                                                                    templateName = assmtTemp.template_name,
                                                                    assessDate = memdiscondass.assessed_date,
                                                                    displayAssessDate = memdiscondass.assessed_date.Value.ToShortDateString(),
                                                                }
                                                          ).ToList();


                if (assessmentList != null && assessmentList.Count > 0)
                {
                    foreach (ConditionAssessment assess in assessmentList)
                    {

                        ConditionAssessment addCondition = new ConditionAssessment();
                        addCondition.patientConditionAsessId = assess.patientConditionAsessId;
                        addCondition.patientConditionRefId = assess.patientConditionRefId;
                        addCondition.assessName = assess.assessName;
                        addCondition.templateId = assess.templateId;
                        addCondition.templateName = assess.templateName;
                        addCondition.assessDate = assess.assessDate;
                        addCondition.displayAssessDate = assess.displayAssessDate;

                        assessmentsReturned.Add(addCondition);

                    }
                }

            }


            return assessmentsReturned;

        }

        public ConditionAssessment getPatientMemberConditionAssessment(string id, string condid, string assessid)
        {
            ConditionAssessment assessmentReturned = null;

            Guid memberId = Guid.Empty;
            int memCondRefId = 0;
            int memCondAssessId = 0;

            if ((!string.IsNullOrEmpty(id) && Guid.TryParse(id, out memberId)) &&
                (!string.IsNullOrEmpty(condid) && int.TryParse(condid, out memCondRefId)) &&
                (!string.IsNullOrEmpty(assessid) && int.TryParse(assessid, out memCondAssessId)))
            {
                ConditionAssessment assessment = (
                                                    from memdiscondass in _icmsContext.MemberDiseaseConditionAssessments

                                                    join memdiscondref in _icmsContext.MemberDiseaseConditionReferences
                                                    on memdiscondass.member_disease_condition_reference_id equals memdiscondref.member_disease_condition_reference_id

                                                    join templ in _icmsContext.MemberDiseaseConditionAssessmentTemplates
                                                    on memdiscondass.member_disease_condition_assessment_template_id equals templ.member_disease_condition_assessment_template_id into assessTemp
                                                    from assmtTemp in assessTemp.DefaultIfEmpty()

                                                    where  memdiscondass.member_disease_condition_assessment_id.Equals(memCondAssessId)
                                                    select new ConditionAssessment
                                                    {
                                                        patientConditionAsessId = memdiscondass.member_disease_condition_assessment_id,
                                                        patientConditionRefId = memdiscondass.member_disease_condition_reference_id,
                                                        assessName = memdiscondass.assessment_name,
                                                        templateId = memdiscondass.member_disease_condition_assessment_template_id,
                                                        templateName = assmtTemp.template_name,
                                                        assessDate = memdiscondass.assessed_date,
                                                        displayAssessDate = memdiscondass.assessed_date.Value.ToShortDateString(),
                                                    }
                                                ).FirstOrDefault();


                if (assessment != null)
                {
                    assessmentReturned = new ConditionAssessment();
                    assessmentReturned.patientConditionAsessId = assessment.patientConditionAsessId;
                    assessmentReturned.patientConditionRefId = assessment.patientConditionRefId;
                    assessmentReturned.assessName = assessment.assessName;
                    assessmentReturned.templateId = assessment.templateId;
                    assessmentReturned.templateName = assessment.templateName;
                    assessmentReturned.assessDate = assessment.assessDate;
                    assessmentReturned.displayAssessDate = assessment.displayAssessDate;
                }

            }


            return assessmentReturned;

        }




        public ConditionAssessment updateMemberConditionAssessment(ConditionAssessment assessment)
        {

            ConditionAssessment returnAssessment = null;

            if (assessment.patientConditionAsessId.Equals(0))
            {
                returnAssessment = AddNewMemberConditionAssessment(assessment); 
            }
            else
            {
                returnAssessment = UpdateExistingMemberConditionAssessment(assessment);
            }

            return returnAssessment;

        }


        public ConditionAssessment AddNewMemberConditionAssessment(ConditionAssessment assessment)
        {

            ConditionAssessment returnAssessment = null;
            
            MemberDiseaseConditionAssessment memDisCondAssessment = new MemberDiseaseConditionAssessment();
            memDisCondAssessment.member_disease_condition_reference_id = assessment.patientConditionRefId;
            memDisCondAssessment.member_disease_condition_assessment_template_id = assessment.templateId;
            memDisCondAssessment.assessment_name = assessment.assessName;
            memDisCondAssessment.assessed_date = assessment.assessDate;
            memDisCondAssessment.creation_date = DateTime.Now;


            if (!string.IsNullOrEmpty(assessment.displayAssessDate))
            {
                DateTime assessDate = DateTime.MinValue;

                if (DateTime.TryParse(assessment.displayAssessDate, out assessDate))
                {
                    memDisCondAssessment.assessed_date = assessDate;
                }

            }

            if (memDisCondAssessment != null)
            {

                _icmsContext.MemberDiseaseConditionAssessments.Update(memDisCondAssessment);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnAssessment = new ConditionAssessment();
                    returnAssessment.patientConditionAsessId = memDisCondAssessment.member_disease_condition_assessment_id;
                    returnAssessment.patientConditionRefId = memDisCondAssessment.member_disease_condition_reference_id;
                    returnAssessment.assessName = memDisCondAssessment.assessment_name;
                    returnAssessment.templateId = memDisCondAssessment.member_disease_condition_assessment_template_id;
                    returnAssessment.assessDate = memDisCondAssessment.assessed_date;
                    returnAssessment.displayAssessDate = memDisCondAssessment.assessed_date.Value.ToShortDateString();

                }

            }

            return returnAssessment;

        }

        public ConditionAssessment UpdateExistingMemberConditionAssessment(ConditionAssessment assessment)
        {

            ConditionAssessment returnAssessment = null;
            
            MemberDiseaseConditionAssessment memCondAssessment = (
                                                                    from memCondAssess in _icmsContext.MemberDiseaseConditionAssessments
                                                                    where memCondAssess.member_disease_condition_assessment_id.Equals(assessment.patientConditionAsessId)
                                                                    select memCondAssess
                                                                 )
                                                                 .FirstOrDefault();

            if (memCondAssessment != null)
            {

                memCondAssessment.assessment_name = assessment.assessName;
                memCondAssessment.assessed_date = assessment.assessDate;                

                _icmsContext.MemberDiseaseConditionAssessments.Update(memCondAssessment);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnAssessment = new ConditionAssessment();
                    returnAssessment.patientConditionAsessId = memCondAssessment.member_disease_condition_assessment_id;
                    returnAssessment.patientConditionRefId = memCondAssessment.member_disease_condition_reference_id;
                    returnAssessment.assessName = memCondAssessment.assessment_name;
                    returnAssessment.templateId = memCondAssessment.member_disease_condition_assessment_template_id;
                    returnAssessment.assessDate = memCondAssessment.assessed_date;
                    returnAssessment.displayAssessDate = memCondAssessment.assessed_date.Value.ToShortDateString();

                }

            }


            return returnAssessment;

        }





        public List<ConditionAssessmentCarePlan> getConditionAssessmentCarePlans(string condid)
        {
            
            List<ConditionAssessmentCarePlan> returnCarePlans = null;

            int memberDiseaseConditionCareplanId = 0;

            if (int.TryParse(condid, out memberDiseaseConditionCareplanId))
            {

                returnCarePlans = (
                        from memdiscondcarepln in _icmsContext.MemberDiseaseConditionCareplans

                        join memdiscondref in _icmsContext.MemberDiseaseConditionReferences
                        on memdiscondcarepln.member_disease_condition_reference_id equals memdiscondref.member_disease_condition_reference_id

                        where memdiscondcarepln.member_disease_condition_reference_id.Equals(memberDiseaseConditionCareplanId)

                        select new ConditionAssessmentCarePlan
                        {
                            patientConditionAssessCareplanId = memdiscondcarepln.member_disease_condition_careplan_id,
                            patientConditionAsessId = memdiscondcarepln.member_disease_condition_reference_id,
                            hospitalInpatientAdmissionId = memdiscondref.hospital_inpatient_admission_id,
                            careplanName = memdiscondcarepln.careplan_name,
                            startDate = (DateTime)memdiscondcarepln.start_date,
                            displayStartDate = (memdiscondcarepln.start_date != null) ? memdiscondcarepln.start_date.Value.ToShortDateString() : "N/A",
                            displayCompletionDate = (memdiscondcarepln.completion_date != null) ? memdiscondcarepln.completion_date.Value.ToShortDateString() : "N/A",
                            completionDate = (DateTime)memdiscondcarepln.completion_date 
            }
                    )
                    .ToList();

            }

            return returnCarePlans;

        }

        public ConditionAssessmentCarePlan getConditionAssessmentCarePlan(string id, string condid, string assessid, string crplnid)
        {

            ConditionAssessmentCarePlan returnCarePlan = null;

            int memberDiseaseConditionCareplanId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId))
            {

                returnCarePlan = (
                        from memdiscondcarepln in _icmsContext.MemberDiseaseConditionCareplans

                        join memdiscondref in _icmsContext.MemberDiseaseConditionReferences
                        on memdiscondcarepln.member_disease_condition_reference_id equals memdiscondref.member_disease_condition_reference_id

                        where memdiscondcarepln.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)

                        select new ConditionAssessmentCarePlan
                        {
                            patientConditionAssessCareplanId = memdiscondcarepln.member_disease_condition_careplan_id,
                            patientConditionAsessId = memdiscondcarepln.member_disease_condition_reference_id,
                            hospitalInpatientAdmissionId = memdiscondref.hospital_inpatient_admission_id,
                            careplanName = memdiscondcarepln.careplan_name,
                            startDate = (DateTime)memdiscondcarepln.start_date,
                            displayStartDate = (memdiscondcarepln.start_date != null) ? memdiscondcarepln.start_date.Value.ToShortDateString() : "N/A",
                            displayCompletionDate = (memdiscondcarepln.completion_date != null) ? memdiscondcarepln.completion_date.Value.ToShortDateString() : "N/A",
                            completionDate = (DateTime)memdiscondcarepln.completion_date
                        }
                    )
                    .FirstOrDefault();

            }

            return returnCarePlan;

        }


        public ConditionAssessmentCarePlan updateMemberConditionAssessmentCarePlan(ConditionAssessmentCarePlan careplan)
        {

            ConditionAssessmentCarePlan returnCarePlan = null;

            if (careplan.patientConditionAssessCareplanId > 0)
            {
                returnCarePlan = updateExistingMemberConditionAssessmentCarePlan(careplan);
            }
            else
            {
                returnCarePlan = addNewMemberConditionAssessmentCarePlan(careplan);
            }

            return returnCarePlan;

        }

        public ConditionAssessmentCarePlan addNewMemberConditionAssessmentCarePlan(ConditionAssessmentCarePlan careplan)
        {

            ConditionAssessmentCarePlan returnCarePlan = null;

            MemberDiseaseConditionCareplan memdiscondcarepln = new MemberDiseaseConditionCareplan();
            memdiscondcarepln.careplan_name = careplan.careplanName;
            memdiscondcarepln.creation_date = DateTime.Now;
            memdiscondcarepln.member_disease_condition_reference_id = careplan.patientConditionRefId;

            DateTime startDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(careplan.displayStartDate) && DateTime.TryParse(careplan.displayStartDate, out startDate))
            {
                memdiscondcarepln.start_date = startDate;
            }

            if (memdiscondcarepln != null)
            {

                _icmsContext.MemberDiseaseConditionCareplans.Add(memdiscondcarepln);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    returnCarePlan = new ConditionAssessmentCarePlan();
                    returnCarePlan.patientConditionAssessCareplanId = memdiscondcarepln.member_disease_condition_careplan_id;
                    returnCarePlan.patientConditionRefId = memdiscondcarepln.member_disease_condition_reference_id;
                    returnCarePlan.careplanName = memdiscondcarepln.careplan_name;
                    returnCarePlan.displayStartDate = (memdiscondcarepln.start_date != null) ?  memdiscondcarepln.start_date.Value.ToShortDateString() : "";
                    returnCarePlan.startDate = (memdiscondcarepln.start_date != null) ? (DateTime)memdiscondcarepln.start_date : DateTime.MinValue;

                }

            }

            return returnCarePlan;

        }

        public ConditionAssessmentCarePlan updateExistingMemberConditionAssessmentCarePlan(ConditionAssessmentCarePlan careplan)
        {

            ConditionAssessmentCarePlan returnCarePlan = null;

            MemberDiseaseConditionCareplan dbCareplan = (
                    from memdiscondcarepln in _icmsContext.MemberDiseaseConditionCareplans
                    where memdiscondcarepln.member_disease_condition_careplan_id.Equals(careplan.patientConditionAssessCareplanId)
                    select memdiscondcarepln
                )
                .FirstOrDefault();

            if (dbCareplan != null)
            {

                dbCareplan.careplan_name = careplan.careplanName;
                dbCareplan.completion_date = careplan.startDate;

                _icmsContext.MemberDiseaseConditionCareplans.Update(dbCareplan);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnCarePlan = new ConditionAssessmentCarePlan();
                    returnCarePlan.patientConditionAssessCareplanId = dbCareplan.member_disease_condition_careplan_id;
                    returnCarePlan.patientConditionRefId = dbCareplan.member_disease_condition_reference_id;
                    returnCarePlan.careplanName = dbCareplan.careplan_name;
                    returnCarePlan.displayStartDate = (dbCareplan.start_date != null) ? dbCareplan.start_date.Value.ToShortDateString() : "";
                    returnCarePlan.startDate = (dbCareplan.start_date != null) ? (DateTime)dbCareplan.start_date : DateTime.MinValue;                    
                    returnCarePlan.displayCompletionDate = (dbCareplan.completion_date != null) ? dbCareplan.completion_date.Value.ToShortDateString() : "";
                    returnCarePlan.completionDate = (dbCareplan.completion_date != null) ? (DateTime)dbCareplan.completion_date : DateTime.MinValue;

                }

            }


            return returnCarePlan;

        }

        public ConditionAssessmentCarePlan updateAdmissionCareplanName(ConditionAssessmentCarePlan careplan)
        {

            ConditionAssessmentCarePlan returnCarePlan = null;

            MemberDiseaseConditionCareplan dbCareplan = (
                    from memdiscondcarepln in _icmsContext.MemberDiseaseConditionCareplans
                    where memdiscondcarepln.member_disease_condition_careplan_id.Equals(careplan.patientConditionAssessCareplanId)
                    select memdiscondcarepln
                )
                .FirstOrDefault();

            if (dbCareplan != null)
            {

                dbCareplan.careplan_name = careplan.careplanName;

                _icmsContext.MemberDiseaseConditionCareplans.Update(dbCareplan);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {

                    returnCarePlan = new ConditionAssessmentCarePlan();
                    returnCarePlan.patientConditionAssessCareplanId = dbCareplan.member_disease_condition_careplan_id;
                    returnCarePlan.patientConditionRefId = dbCareplan.member_disease_condition_reference_id;
                    returnCarePlan.careplanName = dbCareplan.careplan_name;
                    returnCarePlan.displayStartDate = (dbCareplan.start_date != null) ? dbCareplan.start_date.Value.ToShortDateString() : "";
                    returnCarePlan.startDate = (dbCareplan.start_date != null) ? (DateTime)dbCareplan.start_date : DateTime.MinValue;
                    returnCarePlan.displayCompletionDate = (dbCareplan.completion_date != null) ? dbCareplan.completion_date.Value.ToShortDateString() : "";
                    returnCarePlan.completionDate = (dbCareplan.completion_date != null) ? (DateTime)dbCareplan.completion_date : DateTime.MinValue;

                }

            }


            return returnCarePlan;

        }




        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanAssesses(string inpatadmitid, string crplnid)
        {

            List<ConditionAssessmentCarePlan> returnAssess = null;

            int memberDiseaseConditionCareplanId = 0;
            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId) && int.TryParse(inpatadmitid, out hospitalInpatientAdmissionId))
            {

                returnAssess = (
                        from assesses in _icmsContext.HospitalInpatientAdmissionCareplanAssesses

                        where assesses.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)
                        && assesses.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanAssessId = assesses.hospital_inpatient_admission_careplan_assess_id,
                            careplanAssess = assesses.assess,
                            careplanAssessDate = assesses.creation_date,
                            careplanDisplayAssessDate = (assesses.creation_date.HasValue) ? assesses.creation_date.Value.ToShortDateString() : "N/A"
                        }
                    )
                    .ToList();

            }

            return returnAssess;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanAssess(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnAssesses = null;

            if (careplan.careplanAssessId > 0)
            {
                returnAssesses = updateCareplanAssess(careplan);
            }
            else
            {
                returnAssesses = addCareplanAssess(careplan);
            }

            return returnAssesses;

        }

        private List<ConditionAssessmentCarePlan> addCareplanAssess(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnAssesses = null;

            HospitalInpatientAdmissionCareplanAssess assess = new HospitalInpatientAdmissionCareplanAssess();
            assess.member_disease_condition_careplan_id = careplan.patientConditionAssessCareplanId;
            assess.hospital_inpatient_admission_id = (int)careplan.hospitalInpatientAdmissionId;
            assess.assess = careplan.careplanAssess;
            assess.is_re_assessment = careplan.reassessPatient;
            assess.creation_date = DateTime.Now;

            if (assess != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanAssesses.Add(assess);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnAssesses = getAdmissionCarePlanAssesses(careplan.hospitalInpatientAdmissionId.ToString(), careplan.patientConditionAssessCareplanId.ToString());
                }

            }

            return returnAssesses;

        }

        private List<ConditionAssessmentCarePlan> updateCareplanAssess(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnAssesses = null;

            HospitalInpatientAdmissionCareplanAssess dbAssess = (
                    from hosinptadmitcrplnass in _icmsContext.HospitalInpatientAdmissionCareplanAssesses
                    where hosinptadmitcrplnass.hospital_inpatient_admission_careplan_assess_id.Equals(careplan.careplanAssessId)
                    select hosinptadmitcrplnass
                )
                .FirstOrDefault();

            if (dbAssess != null)
            {

                dbAssess.assess = careplan.careplanAssess;
                dbAssess.last_update_date = DateTime.Now;

                _icmsContext.HospitalInpatientAdmissionCareplanAssesses.Update(dbAssess);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnAssesses = getAdmissionCarePlanAssesses(dbAssess.hospital_inpatient_admission_id.ToString(), dbAssess.member_disease_condition_careplan_id.ToString());
                }
            }


            return returnAssesses;

        }



        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanDiagnoses(string inpatadmitid, string crplnid)
        {

            List<ConditionAssessmentCarePlan> returnDiagnosis = null;

            int memberDiseaseConditionCareplanId = 0;
            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId) && int.TryParse(inpatadmitid, out hospitalInpatientAdmissionId))
            {

                returnDiagnosis = (
                        from diags in _icmsContext.HospitalInpatientAdmissionCareplanDiagnoses

                        join domain in _icmsContext.HospitalNursingDiagnosisDomains
                        on diags.hospital_nursing_diagnosis_domain_id equals domain.hospital_nursing_diagnosis_domain_id into domains
                        from crplndiagdomain in domains.DefaultIfEmpty()

                        join classes in _icmsContext.HospitalNursingDiagnosisClasses
                        on diags.hospital_nursing_diagnosis_class_id equals classes.hospital_nursing_diagnosis_class_id into cls
                        from crplndiagclass in cls.DefaultIfEmpty()

                        join priority in _icmsContext.TaskPriorities
                        on diags.task_priority_id equals priority.task_priority_id into taskpriority
                        from prorities in taskpriority.DefaultIfEmpty()

                        where diags.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)
                        && diags.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanDiagnosisId = diags.hospital_inpatient_admission_careplan_diagnosis_id,
                            careplanDiagnosis = diags.diagnosis,
                            careplanDiagnosisDomainId = diags.hospital_nursing_diagnosis_domain_id,
                            careplanDiagnosisDomainName = crplndiagdomain.domain_name,
                            careplanDiagnosisClassId = diags.hospital_nursing_diagnosis_class_id,
                            careplanDiagnosisClassName = crplndiagclass.class_name,
                            careplanDiagnosisDate = diags.creation_date,
                            careplanDisplayDiagnosisDate = (diags.creation_date.HasValue) ? diags.creation_date.Value.ToShortDateString() : "N/A",
                            careplanDiagnosisTaskPriorityId = (diags.task_priority_id.HasValue) ? (diags.task_priority_id > 0) ? diags.task_priority_id : 1 : 1,
                            careplanDiagnosisTaskPriorityDescription = prorities.task_description,
                            careplanDiagnosisResolved = diags.resolved
                        }
                    )
                    .OrderByDescending(prior => prior.careplanDiagnosisTaskPriorityId)
                    .ToList();

            }

            return returnDiagnosis;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanDiagnosis(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnDiagnosis = null;

            if (careplan.careplanDiagnosisId > 0)
            {
                returnDiagnosis = updateCareplanDiagnosis(careplan);
            }
            else
            {
                returnDiagnosis = addCareplanDiagnosis(careplan);
            }

            return returnDiagnosis;

        }

        private List<ConditionAssessmentCarePlan> addCareplanDiagnosis(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnDiagnosis = null;

            HospitalInpatientAdmissionCareplanDiagnosis diagnosis = new HospitalInpatientAdmissionCareplanDiagnosis();
            diagnosis.member_disease_condition_careplan_id = careplan.patientConditionAssessCareplanId;
            diagnosis.hospital_inpatient_admission_id = (int)careplan.hospitalInpatientAdmissionId;
            diagnosis.diagnosis = careplan.careplanDiagnosis;
            diagnosis.is_re_diagnosis = careplan.rediagnosePatient;
            diagnosis.creation_date = DateTime.Now;

            if (careplan.careplanDiagnosisDomainId > 0)
            {
                diagnosis.hospital_nursing_diagnosis_domain_id = careplan.careplanDiagnosisDomainId;
            }

            if (careplan.careplanDiagnosisClassId > 0)
            {
                diagnosis.hospital_nursing_diagnosis_class_id = careplan.careplanDiagnosisClassId;
            }

            if (careplan.careplanDiagnosisTaskPriorityId > 0)
            { 
                diagnosis.task_priority_id = careplan.careplanDiagnosisTaskPriorityId;
            }

            if (diagnosis != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanDiagnoses.Add(diagnosis);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnDiagnosis = getAdmissionCarePlanDiagnoses(careplan.hospitalInpatientAdmissionId.ToString(), careplan.patientConditionAssessCareplanId.ToString());
                }

            }

            return returnDiagnosis;

        }

        private List<ConditionAssessmentCarePlan> updateCareplanDiagnosis(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnDiagnosis = null;

            HospitalInpatientAdmissionCareplanDiagnosis dbDiag = (
                    from hosinptadmitcrplndiag in _icmsContext.HospitalInpatientAdmissionCareplanDiagnoses
                    where hosinptadmitcrplndiag.hospital_inpatient_admission_careplan_diagnosis_id.Equals(careplan.careplanDiagnosisId)
                    select hosinptadmitcrplndiag
                )
                .FirstOrDefault();

            if (dbDiag != null)
            {

                dbDiag.diagnosis = careplan.careplanDiagnosis;
                dbDiag.last_update_date = DateTime.Now;

                if (!dbDiag.hospital_nursing_diagnosis_domain_id.Equals(careplan.careplanDiagnosisDomainId))
                {
                    dbDiag.hospital_nursing_diagnosis_domain_id = careplan.careplanDiagnosisDomainId;
                }

                if (!dbDiag.hospital_nursing_diagnosis_class_id.Equals(careplan.careplanDiagnosisClassId))
                {
                    dbDiag.hospital_nursing_diagnosis_class_id = careplan.careplanDiagnosisClassId;
                }

                _icmsContext.HospitalInpatientAdmissionCareplanDiagnoses.Update(dbDiag);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnDiagnosis = getAdmissionCarePlanDiagnoses(dbDiag.hospital_inpatient_admission_id.ToString(), dbDiag.member_disease_condition_careplan_id.ToString());
                }
            }


            return returnDiagnosis;

        }




        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanOutcomes(string inpatadmitid, string crplnid)
        {

            List<ConditionAssessmentCarePlan> returnOutcomes = null;

            int memberDiseaseConditionCareplanId = 0;
            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId) && int.TryParse(inpatadmitid, out hospitalInpatientAdmissionId))
            {

                returnOutcomes = (
                        from outcms in _icmsContext.HospitalInpatientAdmissionCareplanOutcomes

                        join goal in _icmsContext.HospitalCareplanGoals
                        on outcms.hospital_careplan_goal_id equals goal.hospital_careplan_goal_id into goals
                        from goalmeasure in goals.DefaultIfEmpty()

                        where outcms.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)
                        && outcms.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanOutcomeId = outcms.hospital_inpatient_admission_careplan_outcome_id,
                            careplanOutcome = outcms.outcome,
                            careplanOutcomeGoalId = (outcms.hospital_careplan_goal_id.HasValue) ? (int)outcms.hospital_careplan_goal_id : 0,
                            careplanOutcomeGoalMeasure = (goalmeasure.goal_measure != null) ? goalmeasure.goal_measure : "N/A",
                            careplanDiagnosisId = (outcms.hospital_inpatient_admission_careplan_diagnosis_id.HasValue) ? (int)outcms.hospital_inpatient_admission_careplan_diagnosis_id : 0,
                            careplanOutcomeDate = outcms.creation_date,
                            careplanDisplayOutcomeDate = (outcms.creation_date.HasValue) ? outcms.creation_date.Value.ToShortDateString() : "N/A",
                        }
                    )
                    .OrderBy(goal => goal.careplanOutcomeGoalId)
                    .ToList();

            }

            return returnOutcomes;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanOutcome(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnOutcomes = null;

            if (careplan.careplanOutcomeId > 0)
            {
                returnOutcomes = updateCareplanOutcome(careplan);
            }
            else
            {
                returnOutcomes = addCareplanOutcome(careplan);
            }

            return returnOutcomes;

        }

        private List<ConditionAssessmentCarePlan> addCareplanOutcome(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnOutcomes = null;
            
            HospitalInpatientAdmissionCareplanOutcome outcome = new HospitalInpatientAdmissionCareplanOutcome();
            outcome.member_disease_condition_careplan_id = careplan.patientConditionAssessCareplanId;
            outcome.hospital_inpatient_admission_id = (int)careplan.hospitalInpatientAdmissionId;
            outcome.outcome = careplan.careplanOutcome;           
            outcome.creation_date = DateTime.Now;

            if (careplan.careplanOutcomeGoalId > 0)
            {
                outcome.hospital_careplan_goal_id = careplan.careplanOutcomeGoalId;
            }

            if (outcome != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanOutcomes.Add(outcome);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnOutcomes = getAdmissionCarePlanOutcomes(careplan.hospitalInpatientAdmissionId.ToString(), careplan.patientConditionAssessCareplanId.ToString());
                }

            }

            return returnOutcomes;

        }

        private List<ConditionAssessmentCarePlan> updateCareplanOutcome(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnOutcomes = null;

            HospitalInpatientAdmissionCareplanOutcome dbOutcome = (
                    from hosinptadmitcrplnoutcm in _icmsContext.HospitalInpatientAdmissionCareplanOutcomes
                    where hosinptadmitcrplnoutcm.hospital_inpatient_admission_careplan_outcome_id.Equals(careplan.careplanOutcomeId)
                    select hosinptadmitcrplnoutcm
                )
                .FirstOrDefault();

            if (dbOutcome != null)
            {
                DateTime dateNow = DateTime.Now;

                dbOutcome.outcome = careplan.careplanOutcome;
                dbOutcome.last_update_date = dateNow;

                if (!dbOutcome.hospital_careplan_goal_id.Equals(careplan.careplanOutcomeGoalId))
                {
                    dbOutcome.hospital_careplan_goal_id = careplan.careplanOutcomeGoalId;
                }

                if (careplan.careplanOutcomeIncreaseGoalMetCount)
                {
                    dbOutcome.goal_met_count = (dbOutcome.goal_met_count.HasValue) ? (dbOutcome.goal_met_count > 0) ? dbOutcome.goal_met_count + 1 : 1 : 1;
                }

                if (careplan.careplanOutcomeGoalMet)
                {
                    dbOutcome.goal_met = true;
                    dbOutcome.goal_met_date = dateNow;
                }


                _icmsContext.HospitalInpatientAdmissionCareplanOutcomes.Update(dbOutcome);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnOutcomes = getAdmissionCarePlanOutcomes(dbOutcome.hospital_inpatient_admission_id.ToString(), dbOutcome.member_disease_condition_careplan_id.ToString());
                }
            }


            return returnOutcomes;

        }






        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanInterventions(string inpatadmitid, string crplnid)
        {

            List<ConditionAssessmentCarePlan> returnInterventions = null;

            int memberDiseaseConditionCareplanId = 0;
            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId) && int.TryParse(inpatadmitid, out hospitalInpatientAdmissionId))
            {

                returnInterventions = (
                        from interv in _icmsContext.HospitalInpatientAdmissionCareplanInterventions

                        join freq in _icmsContext.HospitalMedicationFrequencyAdministrations
                        on interv.hospital_medication_frequency_administration_id equals freq.hospital_medication_frequency_administration_id into freqs
                        from hosmedfreqadmin in freqs.DefaultIfEmpty()

                        join invtype in _icmsContext.HospitalCareplanInterventionTypes
                        on interv.hospital_careplan_intervention_type_id equals invtype.hospital_careplan_intervention_type_id into invtyps
                        from hoscrplnintvtyp in invtyps.DefaultIfEmpty()

                        where interv.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)
                        && interv.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanInterventionId = interv.hospital_inpatient_admission_careplan_intervention_id,
                            careplanIntervention = interv.intervention,
                            careplanInterventionRationale = interv.rationale,
                            careplanInterventionFrequencyAdministrationId = (interv.hospital_medication_frequency_administration_id.HasValue) ? (int)interv.hospital_medication_frequency_administration_id : 0,
                            careplanInterventionFrequencyAdministration = hosmedfreqadmin.administration_frequency,
                            careplanInterventionTypeId = (interv.hospital_careplan_intervention_type_id.HasValue) ? (int)interv.hospital_careplan_intervention_type_id : 0,
                            careplanInterventionType = hoscrplnintvtyp.intervention_type,
                            careplanInterventionDate = interv.creation_date,
                            careplanDisplayInterventionDate = (interv.creation_date.HasValue) ? interv.creation_date.Value.ToShortDateString() : "N/A",
                        }
                    )
                    .OrderBy(interv => interv.careplanInterventionId)
                    .ToList();

            }

            return returnInterventions;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanIntervention(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnInterventions = null;

            if (careplan.careplanInterventionId > 0)
            {
                returnInterventions = updateCareplanIntervention(careplan);
            }
            else
            {
                returnInterventions = addCareplanIntervention(careplan);
            }

            return returnInterventions;

        }

        private List<ConditionAssessmentCarePlan> addCareplanIntervention(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnInterventions = null;

            HospitalInpatientAdmissionCareplanIntervention intervention = new HospitalInpatientAdmissionCareplanIntervention();
            intervention.member_disease_condition_careplan_id = careplan.patientConditionAssessCareplanId;
            intervention.hospital_inpatient_admission_id = (int)careplan.hospitalInpatientAdmissionId;
            intervention.intervention = careplan.careplanIntervention;
            intervention.creation_date = DateTime.Now;

            if (!string.IsNullOrEmpty(careplan.careplanInterventionRationale))
            {
                intervention.rationale = careplan.careplanInterventionRationale;
            }

            if (careplan.careplanInterventionFrequencyAdministrationId > 0)
            {
                intervention.hospital_medication_frequency_administration_id = careplan.careplanInterventionFrequencyAdministrationId;
            }

            if (careplan.careplanInterventionTypeId > 0)
            { 
                intervention.hospital_careplan_intervention_type_id = careplan.careplanInterventionTypeId;
            }

            if (intervention != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanInterventions.Add(intervention);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnInterventions = getAdmissionCarePlanInterventions(careplan.hospitalInpatientAdmissionId.ToString(), careplan.patientConditionAssessCareplanId.ToString());
                }

            }

            return returnInterventions;

        }

        private List<ConditionAssessmentCarePlan> updateCareplanIntervention(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnInterventions = null;

            HospitalInpatientAdmissionCareplanIntervention dbIntervention = (
                    from hosinptadmitcrplninterven in _icmsContext.HospitalInpatientAdmissionCareplanInterventions
                    where hosinptadmitcrplninterven.hospital_inpatient_admission_careplan_intervention_id.Equals(careplan.careplanInterventionId)
                    select hosinptadmitcrplninterven
                )
                .FirstOrDefault();

            if (dbIntervention != null)
            {
                DateTime dateNow = DateTime.Now;

                dbIntervention.intervention = careplan.careplanIntervention;
                dbIntervention.last_update_date = dateNow;

                if (!dbIntervention.hospital_medication_frequency_administration_id.Equals(careplan.careplanInterventionFrequencyAdministrationId))
                {
                    dbIntervention.hospital_medication_frequency_administration_id = careplan.careplanInterventionFrequencyAdministrationId;
                }

                if (!dbIntervention.hospital_careplan_intervention_type_id.Equals(careplan.careplanInterventionTypeId))
                {
                    dbIntervention.hospital_careplan_intervention_type_id = careplan.careplanInterventionTypeId;
                }


                _icmsContext.HospitalInpatientAdmissionCareplanInterventions.Update(dbIntervention);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnInterventions = getAdmissionCarePlanInterventions(dbIntervention.hospital_inpatient_admission_id.ToString(), dbIntervention.member_disease_condition_careplan_id.ToString());
                }
            }


            return returnInterventions;

        }




        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanInterventionAdministeredHistory(string inpatadmitid, string crplnid, string interventionId)
        {

            List<ConditionAssessmentCarePlan> returnInterventionHistory = null;

            int hospitalInpatientAdmissionCareplanInterventionId = 0;

            if (int.TryParse(interventionId, out hospitalInpatientAdmissionCareplanInterventionId))
            {

                returnInterventionHistory = (
                        from administered in _icmsContext.HospitalInpatientAdmissionCareplanInterventionAdministereds

                        join interv in _icmsContext.HospitalInpatientAdmissionCareplanInterventions
                        on administered.hospital_inpatient_admission_careplan_intervention_id equals interv.hospital_inpatient_admission_careplan_intervention_id

                        where administered.hospital_inpatient_admission_careplan_intervention_id.Equals(hospitalInpatientAdmissionCareplanInterventionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanInterventionAdministeredId = administered.hospital_inpatient_admission_careplan_intervention_administered_id,
                            careplanInterventionId = interv.hospital_inpatient_admission_careplan_intervention_id,
                            careplanInterventionAdministeredDate = administered.creation_date,
                            careplanDisplayInterventionAdministeredDate = administered.creation_date.ToShortDateString()
                        }
                    )
                    .OrderByDescending(admin => admin.careplanInterventionAdministeredDate)
                    .ToList();

            }

            return returnInterventionHistory;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanInterventionAdministeredHistory(ConditionAssessmentCarePlan careplan)
        {
            List<ConditionAssessmentCarePlan> returnInterventionHistory = null;

            if (careplan.careplanInterventionId > 0)
            {
                returnInterventionHistory = addCareplanInterventionAdministered(careplan);
            }

            return returnInterventionHistory;
        }

        private List<ConditionAssessmentCarePlan> addCareplanInterventionAdministered(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnInterventionHistory = null;
            
            HospitalInpatientAdmissionCareplanInterventionAdministered administered = new HospitalInpatientAdmissionCareplanInterventionAdministered();
            administered.hospital_inpatient_admission_careplan_intervention_id = careplan.careplanInterventionId;
            administered.creation_date = DateTime.Now;

            if (administered != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanInterventionAdministereds.Add(administered);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnInterventionHistory = getAdmissionCarePlanInterventionAdministeredHistory(careplan.hospitalInpatientAdmissionId.ToString(), 
                                                                                                    careplan.patientConditionAssessCareplanId.ToString(), 
                                                                                                    careplan.careplanInterventionId.ToString());
                }

            }

            return returnInterventionHistory;

        }






        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanEvaluations(string inpatadmitid, string crplnid)
        {

            List<ConditionAssessmentCarePlan> returnEvaluations = null;

            int memberDiseaseConditionCareplanId = 0;
            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(crplnid, out memberDiseaseConditionCareplanId) && int.TryParse(inpatadmitid, out hospitalInpatientAdmissionId))
            {

                returnEvaluations = (
                        from evals in _icmsContext.HospitalInpatientAdmissionCareplanEvaluations

                        where evals.member_disease_condition_careplan_id.Equals(memberDiseaseConditionCareplanId)
                        && evals.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new ConditionAssessmentCarePlan
                        {
                            careplanEvaluationId = evals.hospital_inpatient_admission_careplan_evaluation_id,
                            careplanEvaluation = evals.evaluation,
                            careplanEvaluationDate = evals.creation_date,
                            careplanDisplayEvaluationDate = (evals.creation_date.HasValue) ? evals.creation_date.Value.ToShortDateString() : "N/A",
                        }
                    )
                    .OrderByDescending(evals => evals.careplanEvaluationDate)
                    .ToList();

            }

            return returnEvaluations;

        }


        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanEvaluation(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnEvaluations = null;

            if (careplan.careplanInterventionId > 0)
            {
                returnEvaluations = updateCareplanEvaluation(careplan);
            }
            else
            {
                returnEvaluations = addCareplanEvaluation(careplan);
            }

            return returnEvaluations;

        }

        private List<ConditionAssessmentCarePlan> addCareplanEvaluation(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnInterventions = null;

            HospitalInpatientAdmissionCareplanEvaluation evaluation = new HospitalInpatientAdmissionCareplanEvaluation();
            evaluation.member_disease_condition_careplan_id = careplan.patientConditionAssessCareplanId;
            evaluation.hospital_inpatient_admission_id = (int)careplan.hospitalInpatientAdmissionId;
            evaluation.evaluation = careplan.careplanEvaluation;
            evaluation.creation_date = DateTime.Now;

            if (evaluation != null)
            {

                _icmsContext.HospitalInpatientAdmissionCareplanEvaluations.Add(evaluation);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnInterventions = getAdmissionCarePlanEvaluations(careplan.hospitalInpatientAdmissionId.ToString(), careplan.patientConditionAssessCareplanId.ToString());
                }

            }

            return returnInterventions;

        }

        private List<ConditionAssessmentCarePlan> updateCareplanEvaluation(ConditionAssessmentCarePlan careplan)
        {

            List<ConditionAssessmentCarePlan> returnEvaluations = null;

            HospitalInpatientAdmissionCareplanEvaluation dbEvalulation = (
                    from hosinptadmitcrplneval in _icmsContext.HospitalInpatientAdmissionCareplanEvaluations
                    where hosinptadmitcrplneval.hospital_inpatient_admission_careplan_evaluation_id.Equals(careplan.careplanEvaluationId)
                    select hosinptadmitcrplneval
                )
                .FirstOrDefault();

            if (dbEvalulation != null)
            {
                DateTime dateNow = DateTime.Now;

                dbEvalulation.evaluation = careplan.careplanEvaluation;
                dbEvalulation.last_update_date = dateNow;

                _icmsContext.HospitalInpatientAdmissionCareplanEvaluations.Update(dbEvalulation);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    returnEvaluations = getAdmissionCarePlanEvaluations(dbEvalulation.hospital_inpatient_admission_id.ToString(), dbEvalulation.member_disease_condition_careplan_id.ToString());
                }
            }


            return returnEvaluations;

        }

    }
}

using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IPatientCondition
    {
        public List<Condition> getConditions();

        public List<ConditionAssessment> getAssessmentTemplates(string conditionId);

        public ConditionAssessment getAssessmentTemplate(string assessid);

        public List<Condition> getPatientMemberConditionReferences(string id);

        public Condition getPatientMemberConditionReference(string id, string condid);        

        public List<ConditionAssessment> getPatientMemberConditionAssessments(string id, string condid);

        public ConditionAssessment getPatientMemberConditionAssessment(string id, string condid, string assessid);

        public Condition getConditionDifferentialIcds(string condid);

        public List<ConditionAssessmentCarePlan> getConditionAssessmentCarePlans(string condid);

        public ConditionAssessmentCarePlan getConditionAssessmentCarePlan(string id, string condid, string assessid, string crplnid);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanAssesses(string inpatadmitid, string crplnid);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanDiagnoses(string inpatadmitid, string crplnid);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanOutcomes(string inpatadmitid, string crplnid);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanInterventions(string inpatadmitid, string crplnid);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanInterventionAdministeredHistory(string inpatadmitid, string crplnid, string interventionId);

        public List<ConditionAssessmentCarePlan> getAdmissionCarePlanEvaluations(string inpatadmitid, string crplnid);



        public Condition updateMemberConditionReference(Condition condition);

        public Condition updateMemberConditionReferenceIcd(Condition condition);

        public Condition updateMemberConditionDifferentialIcd(Condition condition);

        public Condition removeMemberConditionDifferentialIcd(Condition condition);

        public ConditionAssessment updateMemberConditionAssessment(ConditionAssessment assessment);

        public ConditionAssessmentCarePlan updateMemberConditionAssessmentCarePlan(ConditionAssessmentCarePlan careplan);

        public ConditionAssessmentCarePlan updateAdmissionCareplanName(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanAssess(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanDiagnosis(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanOutcome(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanIntervention(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanInterventionAdministeredHistory(ConditionAssessmentCarePlan careplan);

        public List<ConditionAssessmentCarePlan> saveAdmissionCareplanEvaluation(ConditionAssessmentCarePlan careplan);

    }
}

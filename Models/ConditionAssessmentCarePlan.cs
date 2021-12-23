using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ConditionAssessmentCarePlan
    {
        public int patientConditionAssessCareplanId { get; set; }

        public int patientConditionAsessId { get; set; }

        public int patientConditionRefId { get; set; }

        public int? hospitalInpatientAdmissionId { get; set; }

        public string careplanName { get; set; }

        public DateTime? startDate { get; set; }
        public string displayStartDate { get; set; }

        public DateTime? completionDate { get; set; }
        public string displayCompletionDate { get; set; }


        public bool? reassessPatient { get; set; }
        public int careplanAssessId { get; set; }
        public string careplanAssess { get; set; }
        public DateTime? careplanAssessDate { get; set; }
        public string? careplanDisplayAssessDate { get; set; }



        public bool? rediagnosePatient { get; set; }
        public int careplanDiagnosisId { get; set; }
        public string careplanDiagnosis { get; set; }
        public int? careplanDiagnosisDomainId { get; set; }
        public string? careplanDiagnosisDomainName { get; set; }
        public int? careplanDiagnosisClassId { get; set; }
        public string? careplanDiagnosisClassName { get; set; }
        public DateTime? careplanDiagnosisDate { get; set; }
        public string? careplanDisplayDiagnosisDate { get; set; }
        public int? careplanDiagnosisTaskPriorityId { get; set; }
        public string? careplanDiagnosisTaskPriorityDescription { get; set; }
        public bool? careplanDiagnosisResolved { get; set; }


        public int careplanOutcomeId { get; set; }
        public string careplanOutcome { get; set; }
        public int? careplanOutcomeGoalId { get; set; }
        public string careplanOutcomeGoalMeasure { get; set; }
        public DateTime? careplanOutcomeDate { get; set; }
        public string? careplanDisplayOutcomeDate { get; set; }
        public bool careplanOutcomeIncreaseGoalMetCount { get; set; }
        public bool careplanOutcomeGoalMet { get; set; }


        public int careplanInterventionId { get;set;}
        public string careplanIntervention { get;set;}
        public string careplanInterventionRationale { get; set; }
        public int? careplanInterventionFrequencyAdministrationId { get; set; }
        public string careplanInterventionFrequencyAdministration { get; set; }
        public int? careplanInterventionTypeId { get; set; }
        public string careplanInterventionType { get; set; }
        public DateTime? careplanInterventionDate { get; set; }
        public string careplanDisplayInterventionDate { get; set; }
        public bool careplanInterventionIncreaseMetCount { get; set; }
        public bool careplanInterventionMet { get; set; }

        public int careplanInterventionAdministeredId { get; set; }
        public DateTime? careplanInterventionAdministeredDate { get; set; }
        public string careplanDisplayInterventionAdministeredDate { get; set; }


        public int careplanEvaluationId { get; set; }
        public string careplanEvaluation { get; set; }
        public DateTime? careplanEvaluationDate { get; set; }
        public string careplanDisplayEvaluationDate { get; set; }



    }
}

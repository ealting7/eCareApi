using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientConditionController : ControllerBase
    {

        private readonly IPatientCondition _patientConditionInterface;

        public PatientConditionController(IPatientCondition patientConditionInterface)
        {
            _patientConditionInterface = patientConditionInterface ?? throw new ArgumentNullException(nameof(patientConditionInterface));
        }


        [HttpGet("dbms/get/diseaseconditions")]
        public IActionResult getConditions()
        {
            var conditions = _patientConditionInterface.getConditions();

            if (conditions == null || conditions.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(conditions);
        }

        [HttpGet("dbms/get/assessmenttemplates/{conditionId}")]
        public IActionResult getAssessmentTemplates(string conditionId)
        {
            var templates = _patientConditionInterface.getAssessmentTemplates(conditionId);

            if (templates == null || templates.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(templates);
        }

        [HttpGet("dbms/get/assessmenttemplates/assessment/{assessid}")]
        public IActionResult getAssessmentTemplate(string assessid)
        {
            var templates = _patientConditionInterface.getAssessmentTemplate(assessid);

            if (templates == null)
            {
                return NoContent();
            }

            return Ok(templates);
        }



        [HttpGet("dbms/get/patient/{id}/conditions")]
        public IActionResult getPatientMemberConditionsReferences(string id)
        {
            var conditions = _patientConditionInterface.getPatientMemberConditionReferences(id);

            if (conditions == null || conditions.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(conditions);
        }

        [HttpGet("dbms/get/patient/{id}/conditions/{condid}")]
        public IActionResult getPatientMemberConditionsReference(string id, string condid)
        {
            var condition = _patientConditionInterface.getPatientMemberConditionReference(id, condid);

            if (condition == null)
            {
                return NoContent();
            }

            return Ok(condition);
        }


        [HttpGet("dbms/get/patient/{id}/conditions/{condid}/assessments")]
        public IActionResult getPatientMemberConditionAssessments(string id, string condid)
        {
            var assessments = _patientConditionInterface.getPatientMemberConditionAssessments(id, condid);

            if (assessments == null || assessments.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(assessments);
        }


        [HttpGet("dbms/get/patient/{id}/conditions/{condid}/assessments/{assessid}")]
        public IActionResult getPatientMemberConditionAssessments(string id, string condid, string assessid)
        {
            var assessments = _patientConditionInterface.getPatientMemberConditionAssessment(id, condid, assessid);

            if (assessments == null)
            {
                return NoContent();
            }

            return Ok(assessments);
        }


        [HttpGet("dbms/get/patient/{id}/conditions/{condid}/differentialicds")]
        public IActionResult getPatientMemberConditionDifferentialIcds(string id, string condid)
        {
            var icds = _patientConditionInterface.getConditionDifferentialIcds(condid);

            if (icds.differentialIcds == null || icds.differentialIcds.Count.Equals(0))
            {
                return NoContent();
            }

            return Ok(icds);
        }


        [HttpGet("dbms/get/patient/conditions/{condid}/assessments/careplans")]
        public IActionResult getConditionAssessmentCarePlans(string condid)
        {
            var carePlans = _patientConditionInterface.getConditionAssessmentCarePlans(condid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/patient/{id}/conditions/{condid}/assessments/{assessid}/careplans/{crplnid}")]
        public IActionResult getConditionAssessmentCarePlan(string id, string condid, string assessid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getConditionAssessmentCarePlan(id, condid, assessid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }


        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/assesses")]
        public IActionResult getAdmissionCarePlanAssesses(string inpatadmitid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanAssesses(inpatadmitid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/diagnoses")]
        public IActionResult getAdmissionCarePlanDiagnoses(string inpatadmitid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanDiagnoses(inpatadmitid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/outcomes")]
        public IActionResult getAdmissionCarePlanOutcomes(string inpatadmitid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanOutcomes(inpatadmitid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/interventions")]
        public IActionResult getAdmissionCarePlanInterventions(string inpatadmitid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanInterventions(inpatadmitid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/interventions/{intervid}/administered")]
        public IActionResult getAdmissionCarePlanInterventionAdministeredHistory(string inpatadmitid, string crplnid, string intervid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanInterventionAdministeredHistory(inpatadmitid, crplnid, intervid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }

        [HttpGet("dbms/get/inpatient/admissions/{inpatadmitid}/careplans/{crplnid}/evaluations")]
        public IActionResult getAdmissionCarePlanEvaluations(string inpatadmitid, string crplnid)
        {
            var carePlans = _patientConditionInterface.getAdmissionCarePlanEvaluations(inpatadmitid, crplnid);

            if (carePlans == null)
            {
                return NoContent();
            }

            return Ok(carePlans);
        }





        [HttpPost("dbms/condition/update/")]
        public IActionResult updateMemberConditionReference(Condition condition)
        {
            var returnCondition = _patientConditionInterface.updateMemberConditionReference(condition);

            if (returnCondition == null)
            {
                return NoContent();
            }

            return Ok(returnCondition);
        }

        [HttpPost("dbms/condition/icd/update/")]
        public IActionResult updateMemberConditionReferenceIcd(Condition condition)
        {
            var returnCondition = _patientConditionInterface.updateMemberConditionReferenceIcd(condition);

            if (returnCondition == null)
            {
                return NoContent();
            }

            return Ok(returnCondition);
        }

        [HttpPost("dbms/condition/differentialicd/update/")]
        public IActionResult updateMemberConditionDifferentialIcd(Condition condition)
        {
            var returnCondition = _patientConditionInterface.updateMemberConditionDifferentialIcd(condition);

            if (returnCondition == null)
            {
                return NoContent();
            }

            return Ok(returnCondition);
        }

        [HttpPost("dbms/condition/differentialicd/remove/")]
        public IActionResult removeMemberConditionDifferentialIcd(Condition condition)
        {
            var returnCondition = _patientConditionInterface.removeMemberConditionDifferentialIcd(condition);

            if (returnCondition == null)
            {
                return NoContent();
            }

            return Ok(returnCondition);
        }



        [HttpPost("dbms/condition/careplan/name/update/")]
        public IActionResult updateCareplanName(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.updateAdmissionCareplanName(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }


        [HttpPost("dbms/condition/careplan/assess/update/")]
        public IActionResult updateCareplanAssess(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanAssess(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }

        [HttpPost("dbms/condition/careplan/diagnosis/update/")]
        public IActionResult updateCareplanDiagnosis(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanDiagnosis(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }

        [HttpPost("dbms/condition/careplan/outcome/update/")]
        public IActionResult updateCareplanOutcome(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanOutcome(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }

        [HttpPost("dbms/condition/careplan/intervention/update/")]
        public IActionResult updateCareplanIntervention(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanIntervention(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }

        [HttpPost("dbms/condition/careplan/intervention/administered/update/")]
        public IActionResult updateCareplanInterventionAdministeredHistory(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanInterventionAdministeredHistory(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }

        [HttpPost("dbms/condition/careplan/evaluation/update/")]
        public IActionResult updateCareplanEvaluation(ConditionAssessmentCarePlan careplan)
        {
            var returnCareplan = _patientConditionInterface.saveAdmissionCareplanEvaluation(careplan);

            if (returnCareplan == null)
            {
                return NoContent();
            }

            return Ok(returnCareplan);
        }



        [HttpPost("dbms/condition/assessment/update/")]
        public IActionResult updateMemberConditionAssessment(ConditionAssessment assessment)
        {
            var returnAssessment = _patientConditionInterface.updateMemberConditionAssessment(assessment);

            if (returnAssessment == null)
            {
                return NoContent();
            }

            return Ok(returnAssessment);
        }


        [HttpPost("dbms/condition/assessment/careplan/update/")]
        public IActionResult updateMemberConditionAssessmentCarePlan(ConditionAssessmentCarePlan careplan)
        {
            var returnAssessment = _patientConditionInterface.updateMemberConditionAssessmentCarePlan(careplan);

            if (returnAssessment == null)
            {
                return NoContent();
            }

            return Ok(returnAssessment);
        }




    }
}

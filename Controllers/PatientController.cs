using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatient _patientInterface;

        public PatientController(IPatient patientInterface)
        {
            _patientInterface = patientInterface ?? throw new ArgumentNullException(nameof(patientInterface));
        }


        [HttpGet("labqsearch/{first}/{last}/{dob}")]
        public IActionResult GetPatientSearchFirstLastDob(string first, string last, string dob)
        {
            var patients = _patientInterface.GetPatientsUsingFirstLastDob(first, last, dob);

            if (patients == null)
            {
                return NotFound();
            }

            var returnPatients = new List<Patient>();

            foreach (var pat in patients)
            {
                returnPatients.Add(new Patient
                {
                    PatientId = pat.member_id,
                    FirstName = pat.member_first_name,
                    LastName = pat.member_last_name,
                    DateOfBirth = pat.member_birth,
                    DateOfBirthDisplay = pat.member_birth.ToString(),
                    Ssn = pat.member_ssn
                });
            }


            return Ok(returnPatients);
        }

        [HttpGet("labqsearch/{id}")]
        public IActionResult GetPatientSearchId(string id)
        {
            var patient = _patientInterface.GetPatientsUsingId(id);

            if (patient == null)
            {
                return NotFound();
            }

            Patient returnPatient = new Patient();

            returnPatient.PatientId = patient.PatientId;
            returnPatient.FirstName = patient.FirstName;
            returnPatient.LastName = patient.LastName;
            returnPatient.MiddleName = patient.MiddleName;
            returnPatient.DateOfBirth = (!patient.DateOfBirth.Equals(DateTime.MinValue)) ? Convert.ToDateTime(patient.DateOfBirth?.ToString("d")) : DateTime.MinValue;
            returnPatient.DateOfBirthDisplay = (!patient.DateOfBirth.Equals(DateTime.MinValue)) ? patient.DateOfBirth?.ToString("d") : "";
            returnPatient.Ssn = patient.Ssn;
            returnPatient.Gender = (!string.IsNullOrEmpty(patient.Gender)) ? (patient.Gender.ToLower().Equals("m")) ? "male" : "female" : "na";
            returnPatient.Email = patient.Email;
            returnPatient.Ethnicity = patient.Ethnicity;

            returnPatient.Ancestry = _patientInterface.GetPatientAncestryUsingId(id);
            returnPatient.Addresses = _patientInterface.GetPatientAddress(id, true);
            returnPatient.HomePhoneNumber = _patientInterface.GetPatientHomePhoneNumber(id, true);

            return Ok(returnPatient);
        }


        [HttpGet("labqsearch/{id}/insurance")]
        public IActionResult GetPatientInsurance(string id)
        {
            var insurance = _patientInterface.GetPatientInsuranceUsingId(id);

            if (insurance == null)
            {
                return NoContent();
            }

            Patient returnPatient = new Patient();

            returnPatient.PatientId = insurance.PatientId;

            if (!returnPatient.PatientId.Equals(Guid.Empty))
            {
                returnPatient.InsuranceName = insurance.InsuranceName;
                returnPatient.SelfPay = (insurance.SelfPay.HasValue) ? insurance.SelfPay : false;
                returnPatient.IsMedicaid = (insurance.IsMedicaid.HasValue) ? insurance.IsMedicaid : false;
                returnPatient.IsMedicare = (insurance.IsMedicare.HasValue) ? insurance.IsMedicare : false;
                returnPatient.InsuranceMemberId = insurance.InsuranceMemberId;
                returnPatient.InsuranceSubscriberFirstName = insurance.InsuranceSubscriberFirstName;
                returnPatient.InsuranceSubscriberLastName = insurance.InsuranceSubscriberLastName;
                returnPatient.InsuranceRelationshipId = insurance.InsuranceRelationshipId;
                returnPatient.InsuranceRelationshipToPatient = insurance.InsuranceRelationshipToPatient;
            }

            return Ok(returnPatient);
        }

    }
}

using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatient _patientInterface;

        public PatientController(IPatient patientInterface)
        {
            _patientInterface = patientInterface ?? throw new ArgumentNullException(nameof(patientInterface));
        }

        [HttpPost("dbms/search")]
        public IActionResult getPatients(Patient search)
        {
            var returnPatients = _patientInterface.searchPatients(search);

            if (returnPatients == null)
            {
                return NotFound();
            }

            return Ok(returnPatients);
        }

        [HttpGet("dbms/search/{patientId}")]
        public IActionResult getPatient(string patientId)
        {
            var returnPatient = _patientInterface.searchPatient(patientId);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpGet("dbms/search/{patientId}/age")]
        public IActionResult getPatientAge(string patientId)
        {
            var returnPatient = _patientInterface.searchPatientAge(patientId);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
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
                    firstName = pat.member_first_name,
                    lastName = pat.member_last_name,
                    dateOfBirth = pat.member_birth,
                    dateOfBirthDisplay = pat.member_birth.ToString(),
                    ssn = pat.member_ssn
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
            returnPatient.firstName = patient.firstName;
            returnPatient.lastName = patient.lastName;
            returnPatient.middleName = patient.middleName;
            returnPatient.dateOfBirth = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? Convert.ToDateTime(patient.dateOfBirth?.ToString("d")) : DateTime.MinValue;
            returnPatient.dateOfBirthDisplay = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? patient.dateOfBirth?.ToString("d") : "";
            returnPatient.ssn = patient.ssn;
            returnPatient.gender = (!string.IsNullOrEmpty(patient.gender)) ? (patient.gender.ToLower().Equals("m")) ? "male" : "female" : "na";
            returnPatient.emailAddress = patient.emailAddress;
            returnPatient.ethnicity = patient.ethnicity;

            returnPatient.ancestry = _patientInterface.GetPatientAncestryUsingId(id);
            returnPatient.addresses = _patientInterface.GetPatientAddress(id, true);
            returnPatient.homePhoneNumber = _patientInterface.GetPatientHomePhoneNumber(id, true);

            return Ok(returnPatient);
        }

        [HttpGet("utilizationmanagement/patients/{id}")]
        public IActionResult GetUtilizationPatient(string id)
        {
            var patient = _patientInterface.GetPatientsUsingId(id);

            if (patient == null)
            {
                return NotFound();
            }

            Patient returnPatient = new Patient();

            returnPatient.PatientId = patient.PatientId;
            returnPatient.firstName = patient.firstName;
            returnPatient.lastName = patient.lastName;
            returnPatient.middleName = patient.middleName;
            returnPatient.FullName = patient.firstName + ((!string.IsNullOrEmpty(patient.middleName)) ? " " + patient.middleName : "") + " " + patient.lastName;
            returnPatient.dateOfBirth = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? Convert.ToDateTime(patient.dateOfBirth?.ToString("d")) : DateTime.MinValue;
            returnPatient.dateOfBirthDisplay = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? patient.dateOfBirth?.ToString("d") : "";
            returnPatient.ssn = patient.ssn;
            returnPatient.gender = (!string.IsNullOrEmpty(patient.gender)) ? (patient.gender.ToLower().Equals("m")) ? "male" : "female" : "na";
            returnPatient.emailAddress = patient.emailAddress;
            returnPatient.ethnicity = patient.ethnicity;

            returnPatient.ancestry = _patientInterface.GetPatientAncestryUsingId(id);
            returnPatient.addresses = _patientInterface.GetPatientAddress(id, true);
            returnPatient.homePhoneNumber = _patientInterface.GetPatientHomePhoneNumber(id, true);

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
                returnPatient.selfPay = (insurance.selfPay.HasValue) ? insurance.selfPay : false;
                returnPatient.isMedicaid = (insurance.isMedicaid.HasValue) ? insurance.isMedicaid : false;
                returnPatient.isMedicare = (insurance.isMedicare.HasValue) ? insurance.isMedicare : false;
                returnPatient.insuranceMemberId = insurance.insuranceMemberId;
                returnPatient.insuranceSubscriberFirstName = insurance.insuranceSubscriberFirstName;
                returnPatient.insuranceSubscriberLastName = insurance.insuranceSubscriberLastName;
                returnPatient.insuranceRelationshipId = insurance.insuranceRelationshipId;
                returnPatient.insuranceRelationshipToPatient = insurance.insuranceRelationshipToPatient;
            }

            return Ok(returnPatient);
        }



        [HttpGet("notes/{id}/{date}")]
        public IActionResult GetPatientMemberNote(string id, string date)
        {
            var note = _patientInterface.GetPatientMemberNotes(id, date);

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }
    }
}

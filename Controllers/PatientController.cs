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

        [HttpGet("dbms/get/patients/{patientId}")]
        public IActionResult getPatientDemographics(string patientId)
        {
            var returnPatient = _patientInterface.getPatientDemographics(patientId);

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

            //Patient returnPatient = new Patient();

            //returnPatient.PatientId = patient.PatientId;
            //returnPatient.firstName = patient.firstName;
            //returnPatient.lastName = patient.lastName;
            //returnPatient.middleName = patient.middleName;
            //returnPatient.FullName = patient.firstName + ((!string.IsNullOrEmpty(patient.middleName)) ? " " + patient.middleName : "") + " " + patient.lastName;
            //returnPatient.dateOfBirth = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? Convert.ToDateTime(patient.dateOfBirth?.ToString("d")) : DateTime.MinValue;
            //returnPatient.dateOfBirthDisplay = (!patient.dateOfBirth.Equals(DateTime.MinValue)) ? patient.dateOfBirth?.ToString("d") : "";
            //returnPatient.ssn = patient.ssn;
            //returnPatient.gender = (!string.IsNullOrEmpty(patient.gender)) ? (patient.gender.ToLower().Equals("m")) ? "male" : "female" : "na";
            //returnPatient.emailAddress = patient.emailAddress;
            //returnPatient.ethnicity = patient.ethnicity;

            //returnPatient.addresses = patient.addresses;
            //returnPatient.homePhoneNumber = patient.homePhoneNumber;
            //returnPatient.eveningPhoneNumber = patient.eveningPhoneNumber;
            //returnPatient.egpMemberId = patient.egpMemberId;
            //returnPatient.medicaidNumber = patient.medicaidNumber;
            //returnPatient.medicareNumber = patient.medicareNumber;

            //returnPatient.employerId = patient.employerId;
            //returnPatient.employerName = patient.employerName;

            //returnPatient.ancestry = _patientInterface.GetPatientAncestryUsingId(id);
            //returnPatient.addresses = _patientInterface.GetPatientAddress(id, true);
            //returnPatient.homePhoneNumber = _patientInterface.GetPatientHomePhoneNumber(id, true);

            return Ok(patient);
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
                returnPatient.insuranceId = insurance.insuranceId;
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

        [HttpGet("dbms/dashboard/get/assets")]
        public IActionResult getPatientDashboardAssets()
        {
            var note = _patientInterface.getPatientDashboardAssets();

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }


        
        [HttpGet("dbms/get/patients/address/{addrId}")]
        public IActionResult getPatientAddressByAddressId(int addrId)
        {
            var note = _patientInterface.getPatientAddressByAddressId(addrId);

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }




        [HttpPost("dbms/inpatient/patient/create")]
        public IActionResult createInpatientPatient(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientForInpatient(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }


        [HttpPost("dbms/verify/basic")]
        public IActionResult patientBasicExists(Patient patient)
        {
            var returnPatient = _patientInterface.patientBasicExists(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/add/basic")]
        public IActionResult createPatient(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientBasic(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/update/info")]
        public IActionResult updatePatientInfo(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientInfo(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/update/email")]
        public IActionResult updatePatientEmail(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientEmail(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/add/address")]
        public IActionResult addPatientAddress(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientAddress(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/remove/address")]
        public IActionResult removePatientAddress(Patient patient)
        {
            var returnPatient = _patientInterface.removePatientAddress(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }
        
        [HttpPost("dbms/update/address")]
        public IActionResult updatePatientAddress(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientAddress(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/add/phone")]
        public IActionResult addPatientPhone(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientPhone(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/remove/phone")]
        public IActionResult removePatientPhone(Patient patient)
        {
            var returnPatient = _patientInterface.removePatientPhone(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/update/employer")]
        public IActionResult updatePatientEmployer(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientEmployer(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/add/hospital")]
        public IActionResult addPatientHospital(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientHospital(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/remove/hospital")]
        public IActionResult removePatientHospital(Patient patient)
        {
            var returnPatient = _patientInterface.removePatientHospital(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/update/cm/identification")]
        public IActionResult updatePatientCmIdentification(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientCmIdentification(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/add/caseowner")]
        public IActionResult addPatientCaseOwner(Patient patient)
        {
            var returnPatient = _patientInterface.addPatientCaseOwner(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/remove/caseowner")]
        public IActionResult removePatientCaseOwner(Patient patient)
        {
            var returnPatient = _patientInterface.removePatientCaseOwner(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

        [HttpPost("dbms/update/insurance")]
        public IActionResult updatePatientInsurance(Patient patient)
        {
            var returnPatient = _patientInterface.updatePatientInsurance(patient);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }
        


    }
}

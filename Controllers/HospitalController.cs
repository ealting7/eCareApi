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
    public class HospitalController : ControllerBase
    {
        private readonly IHospital _hospitalInterface;

        public HospitalController(IHospital hospitalInterface)
        {
            _hospitalInterface = hospitalInterface ?? throw new ArgumentNullException(nameof(hospitalInterface));
        }


        [HttpGet("dbms/search/departments/{hospitalId}")]
        public IActionResult getHospitalDepartments(string hospitalId)
        {
            var depts = _hospitalInterface.getHospitalDepartments(hospitalId);

            if (depts == null)
            {
                return NoContent();
            }

            return Ok(depts);
        }


        [HttpPost("dbms/admissions/")]
        public IActionResult getHospitalAdmissions(Admission search)
        {
            var admissions = _hospitalInterface.getHospitalInpatientAdmissions(search);

            if (admissions == null)
            {
                return NotFound();
            }

            return Ok(admissions);
        }


        [HttpGet("dbms/get/patients/{patientId}/inpatient/admissions/")]
        public IActionResult getCareplanAssessBasicGeneral(string patientId)
        {
            var admits = _hospitalInterface.getPatientInpatientAdmissions(patientId);

            if (admits == null)
            {
                return NoContent();
            }

            return Ok(admits);
        }


        [HttpGet("dbms/get/inpatient/admissions/{admitId}")]
        public IActionResult getAdmission(string admitId)
        {
            var admit = _hospitalInterface.getInpatientAdmission(admitId);

            if (admit == null)
            {
                return NoContent();
            }

            return Ok(admit);
        }


        [HttpGet("dbms/get/patients/{patientId}/admissions/current/")]
        public IActionResult getPatientCurrentAdmission(string patientId)
        {
            var admits = _hospitalInterface.getPatientCurrentAdmission(patientId);

            if (admits == null)
            {
                return NoContent();
            }

            return Ok(admits);
        }



        [HttpGet("labqsearch/hospitals/collectionfacilities")]
        public IActionResult GetCollectionFacilities()
        {
            var facilities = _hospitalInterface.GetCollectionFacilities();

            if (facilities == null)
            {
                return NoContent();
            }

            var returnDrs = new List<HospitalFacility>();

            foreach (var hospFac in facilities)
            {
                returnDrs.Add(new HospitalFacility
                {
                    hospitalId = hospFac.hospital_id,
                    hospitalName = hospFac.name,
                    address1 = hospFac.address1,
                    address2 = hospFac.address2,
                    city = hospFac.city,
                    stateAbbrev = hospFac.state_abbrev,
                    zip = hospFac.zip
                });
            }


            return Ok(returnDrs);
        }

        [HttpGet("labqsearch/hospitals/labfacilities")]
        public IActionResult GetLaboratoryFacilities()
        {
            var facilities = _hospitalInterface.GetLaboratoryFacilities();

            if (facilities == null)
            {
                return NoContent();
            }

            var returnDrs = new List<HospitalFacility>();

            foreach (var hospFac in facilities)
            {
                returnDrs.Add(new HospitalFacility
                {
                    hospitalId = hospFac.hospitalId,
                    hospitalName = hospFac.hospitalName,
                    address1 = hospFac.address1,
                    address2 = hospFac.address2,
                    city = hospFac.city,
                    stateAbbrev = hospFac.stateAbbrev,
                    zip = hospFac.zip,
                    hospitalSpecialty = hospFac.hospitalSpecialty
                });
            }


            return Ok(returnDrs);
        }


        [HttpGet("dbms/get/careplan/assess/form/data")]
        public IActionResult getHospitalCareplanAssessFormItems()
        {
            var items = _hospitalInterface.getCareplanAssessFormItems();

            if (items == null)
            {
                return NoContent();
            }

            return Ok(items);
        }

        [HttpGet("dbms/get/careplan/diagnosis/domains/{domainId}/classes")]
        public IActionResult getHospitalCareplanDiagnosisDomainClasses(string domainId)
        {
            var classses = _hospitalInterface.getCareplanDiagnosisDomainClasses(domainId);

            if (classses == null)
            {
                return NoContent();
            }

            return Ok(classses);
        }


        [HttpGet("dbms/get/careplan/assess/basic/general/{inpatientAdmissionId}")]
        public IActionResult getCareplanAssessBasicGeneral(int inpatientAdmissionId)
        {
            var items = _hospitalInterface.getCareplanAssessBasicGenerals(inpatientAdmissionId);

            if (items == null)
            {
                return NoContent();
            }

            return Ok(items);
        }

    }
}

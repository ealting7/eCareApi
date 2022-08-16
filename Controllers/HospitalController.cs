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

        [HttpGet("dbms/search/departments/{departmentId}/rooms")]
        public IActionResult getHospitalDepartmentRooms(string departmentId)
        {
            var rooms = _hospitalInterface.getHospitalDepartmentRooms(departmentId);

            if (rooms == null)
            {
                return NoContent();
            }

            return Ok(rooms);
        }


        [HttpGet("dbms/get/hospital/{hospitalId}/er/visits/")]
        public IActionResult getHospitalErVisits(string hospitalId)
        {
            var visits = _hospitalInterface.getHospitalErVisits(hospitalId);

            if (visits == null)
            {
                return NoContent();
            }

            return Ok(visits);
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


        [HttpPost("dbms/get/inpatient/admissions/")]
        public IActionResult getAdmission(Admission admit)
        {
            var admission = _hospitalInterface.getInpatientAdmission(admit);

            if (admission == null)
            {
                return NoContent();
            }

            if (admit.usr != null)
            {
                admission.loggedInUser = _hospitalInterface.getLoggedInUser(admit.usr);
            }

            return Ok(admission);
        }

        [HttpPost("dbms/get/inpatient/admissions/user/defaults/")]
        public IActionResult getUserAdmissionDashboardDefaults(Admission admit)
        {
            var admission = _hospitalInterface.getInpatientAdmissionUserDashboardDefaults(admit);

            if (admission == null)
            {
                return NoContent();
            }

            return Ok(admission);
        }

        [HttpPost("dbms/get/inpatient/admissions/charts")]
        public IActionResult getAdmissionChart(InpatientChart admit)
        {
            var admission = _hospitalInterface.getInpatientChart(admit);

            if (admission == null)
            {
                return NoContent();
            }

            return Ok(admission);
        }

        [HttpPost("dbms/get/inpatient/admissions/charts/source/")]
        public IActionResult getAdmissionChartSourceItem(InpatientChart chart)
        {
            var admission = _hospitalInterface.getAdmissionChartSourceItem(chart);

            if (admission == null)
            {
                return NoContent();
            }

            return Ok(admission);
        }

        [HttpPost("dbms/get/inpatient/admissions/charts/sources/")]
        public IActionResult getAdmissionChartSources(InpatientChart chart)
        {
            var sources = _hospitalInterface.getAdmissionChartSources(chart);

            if (sources == null)
            {
                return NoContent();
            }

            return Ok(sources);
        }

                   
        [HttpPost("dbms/get/inpatient/admissions/chart/source/highlightnote")]
        public IActionResult getAdmissionChartSourceHighlightNote(InpatientChart source)
        {
            var sources = _hospitalInterface.getAdmissionChartSourceHighlightNote(source);

            if (sources == null)
            {
                return NoContent();
            }

            return Ok(sources);
        }


        [HttpPost("dbms/update/inpatient/admissions/charts/sources/")]
        public IActionResult updateAllChartSources(InpatientChart chart)
        {
            var admission = _hospitalInterface.updateAllChartSources(chart);

            if (admission == null)
            {
                return NoContent();
            }

            return Ok(admission);
        }


        
        [HttpPost("dbms/update/inpatient/admissions/charts/source/")]
        public IActionResult updateChartSources(InpatientChart chart)
        {
            var admission = _hospitalInterface.updateChartSource(chart);

            if (admission == null)
            {
                return NoContent();
            }

            return Ok(admission);
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

        [HttpGet("labqsearch/hospitals/specimentypes")]
        public IActionResult getSpecimenTypes()
        {
            var specimenTypes = _hospitalInterface.getSpecimenTypes();

            if (specimenTypes == null)
            {
                return NoContent();
            }            

            return Ok(specimenTypes);
        }

        [HttpGet("labqsearch/hospitals/specimenvolumes")]
        public IActionResult getSpecimenVolumes()
        {
            var specimenVolumes = _hospitalInterface.getSpecimenVolumes();

            if (specimenVolumes == null)
            {
                return NoContent();
            }

            return Ok(specimenVolumes);
        }



        [HttpGet("dbms/get/mdt/notetypes")]
        public IActionResult getHospitalNoteTypes()
        {
            var noteTypes = _hospitalInterface.getHospitalNoteTypes();

            if (noteTypes == null)
            {
                return NoContent();
            }

            return Ok(noteTypes);
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







        [HttpGet("dbms/get/inpatient/admissions/{admitId}/user/{userId}/dashboard/defaults")]
        public IActionResult getUserAdmissionDashboardDefaults(string admitId, string userId)
        {
            var admit = _hospitalInterface.getUserInpatientAdmissionDashboardDefaults(admitId, userId);

            if (admit == null)
            {
                return NoContent();
            }

            return Ok(admit);
        }

        [HttpPost("dbms/update/inpatient/admissions/user/defaults")]
        public IActionResult updateUserAdmissionDashboardDefaults(Admission defaults)
        {
            var admissions = _hospitalInterface.updateUserInpatientAdmissionDashboardDefaults(defaults);

            if (admissions == null)
            {
                return NotFound();
            }

            return Ok(admissions);
        }




        [HttpGet("dbms/get/inpatient/admissions/{admitId}/medications")]
        public IActionResult getAdmissionMedications(string admitId)
        {
            var meds = _hospitalInterface.getAdmissionMedications(admitId);

            if (meds == null)
            {
                return NoContent();
            }

            return Ok(meds);
        }



        [HttpGet("dbms/get/inpatient/admissions/{admitId}/medications/removed")]
        public IActionResult getRemovedAdmissionMedications(string admitId)
        {
            var meds = _hospitalInterface.getRemovedAdmissionMedications(admitId);

            if (meds == null)
            {
                return NoContent();
            }

            return Ok(meds);
        }


        [HttpPost("dbms/update/inpatient/admissions/medications/allergy")]
        public IActionResult updateAdmissionMedicationAllergies(Admission medication)
        {
            var allergies = _hospitalInterface.updateAdmissionMedicationAllergies(medication);

            if (allergies == null)
            {
                return NotFound();
            }

            return Ok(allergies);
        }



        [HttpGet("dbms/get/inpatient/admissions/{admitId}/labs")]
        public IActionResult getAdmissionLabs(string admitId)
        {
            var labs = _hospitalInterface.getAdmissionLabs(admitId);

            if (labs == null)
            {
                return NoContent();
            }

            return Ok(labs);
        }



        [HttpPost("dbms/get/inpatient/admissions/notes/mdt")]
        public IActionResult getAdmissionMdtNotes(Note note)
        {
            var notes = _hospitalInterface.getAdmissionNotesMdt(note);

            if (notes == null)
            {
                return NotFound();
            }

            return Ok(notes);
        }



        [HttpGet("dbms/get/inpatient/admissions/{admitId}/documents")]
        public IActionResult getAdmissionDocuments(string admitId)
        {
            var docs = _hospitalInterface.getAdmissionDocuments(admitId);

            if (docs == null)
            {
                return NoContent();
            }

            return Ok(docs);
        }

        [HttpGet("dbms/get/inpatient/admissions/documents/{docId}")]
        public IActionResult getAdmissionDocument(string docId)
        {
            var doc = _hospitalInterface.getAdmissionDocument(docId);

            if (doc == null)
            {
                return NoContent();
            }

            return Ok(doc);
        }

        [HttpPost("dbms/update/inpatient/admissions/documents")]
        public IActionResult uploadAdmissionDocument(DocumentForm doc)
        {
            var docs = _hospitalInterface.uploadAdmissionDocument(doc);

            if (docs == null)
            {
                return NotFound();
            }

            return Ok(docs);
        }








        [HttpPost("dbms/inpatient/admissions/vitalsigns")]
        public IActionResult addAdmissionVitalSign(Admission admit)
        {
            var admissions = _hospitalInterface.insertAdmissionVitalSign(admit);

            if (admissions == null)
            {
                return NotFound();
            }

            return Ok(admissions);
        }



        [HttpPost("dbms/inpatient/admission/create")]
        public IActionResult addPatientAdmission(Patient patient)
        {
            var returnPatientWithAdmission = _hospitalInterface.insertHospitalInpatientAdmission(patient);

            if (returnPatientWithAdmission == null)
            {
                return NotFound();
            }

            return Ok(returnPatientWithAdmission);
        }



        [HttpPost("dbms/update/inpatient/admissions/notes/mdt")]
        public IActionResult updateAdmissionNotesMdt(Note mdtNote)
        {
            var notes = _hospitalInterface.updateAdmissionNotesMdt(mdtNote, true);

            if (notes == null)
            {
                return NotFound();
            }

            return Ok(notes);
        }




        [HttpPost("dbms/facilities/remove")]
        public IActionResult removeFacility(HospitalFacility facility)
        {
            var removed = _hospitalInterface.disableFacility(facility);

            return Ok(removed);
        }

    }
}

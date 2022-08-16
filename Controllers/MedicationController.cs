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
    public class MedicationController : ControllerBase
    {

        private readonly IMedication _medicationInterface;

        public MedicationController(IMedication medicationInterface)
        {
            _medicationInterface = medicationInterface ?? throw new ArgumentNullException(nameof(medicationInterface));
        }

        [HttpPost("dbms/search/medications/")]
        public IActionResult medicationSearch(Medication medSearch)
        {
            var returnMedications = _medicationInterface.medicationSearch(medSearch);

            if (returnMedications == null)
            {
                return NotFound();
            }

            return Ok(returnMedications);
        }


        [HttpPost("dbms/update/inpatient/admissions/medications")]
        public IActionResult updateAdmissionMedications(Medication med)
        {
            var medications = _medicationInterface.updateAdmissionMedications(med);

            if (medications == null)
            {
                return NotFound();
            }

            Admission admit = new Admission();
            admit.medication = medications;

            return Ok(admit);
        }
        
        [HttpPost("dbms/update/inpatient/admissions/medications/reorder")]
        public IActionResult reOrderAdmissionMedications(Medication med)
        {
            var medications = _medicationInterface.reOrderAdmissionMedications(med);

            if (medications == null)
            {
                return NotFound();
            }

            Admission admit = new Admission();
            admit.medication = medications;

            return Ok(admit);
        }



        [HttpPost("dbms/remove/inpatient/admissions/medications/")]
        public IActionResult removeAdmissionMedications(Medication med)
        {
            var medications = _medicationInterface.removeAdmissionMedications(med);

            if (medications == null)
            {
                return NotFound();
            }

            Admission admit = new Admission();
            admit.medication = medications;

            return Ok(admit);
        }
    }
}

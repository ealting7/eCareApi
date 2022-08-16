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
    public class EmrController : ControllerBase
    {
        private readonly IEmr _emrInterface;

        public EmrController(IEmr emrInterface)
        {
            _emrInterface = emrInterface ?? throw new ArgumentNullException(nameof(emrInterface));
        }


        [HttpGet("dbms/dbms/get/allergies/{patientId}")]
        public IActionResult getPatientAllergies(string patientId)
        {
            var returnPatient = _emrInterface.getPatientAllergies(patientId);

            if (returnPatient == null)
            {
                return NotFound();
            }

            return Ok(returnPatient);
        }

    }
}

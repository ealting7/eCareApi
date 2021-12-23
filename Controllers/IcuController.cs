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
    public class IcuController : ControllerBase
    {
        private readonly IIcu _icuInterface;

        public IcuController(IIcu icuInterface)
        {
            _icuInterface = icuInterface ?? throw new ArgumentNullException(nameof(icuInterface));
        }


        [HttpGet("dbms/get/form/data")]
        public IActionResult getHospitalIcuFormItems()
        {
            var assets = _icuInterface.getIcuFormItems();

            if (assets == null)
            {
                return NoContent();
            }

            return Ok(assets);
        }

        [HttpPost("dbms/get/hospitals/schedules/")]
        public IActionResult getHospitalIcuSchedules([FromBody] Appointment icu)
        {
            var icuSchedules = _icuInterface.getHospitalIcuWorkDay(icu);

            if (icuSchedules == null)
            {
                return NoContent();
            }

            return Ok(icuSchedules);
        }
        
        [HttpPost("dbms/search/patients/preop")]
        public IActionResult searchPatientsPreop([FromBody] Appointment preop)
        {
            var patient = _icuInterface.searchPreopPatients(preop);

            if (patient == null)
            {
                return NoContent();
            }

            return Ok(patient);
        }


        [HttpPost("dbms/inpatient/admissions/rooms/")]
        public IActionResult loadIcuRoomAdmissions([FromBody]Admission admit)
        {
            var icuAdmissions = _icuInterface.loadIcuRoomAdmissions(admit);

            if (icuAdmissions == null)
            {
                return NoContent();
            }

            return Ok(icuAdmissions);
        }


    }
}

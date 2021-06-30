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
    public class HospitalController : ControllerBase
    {
        private readonly IHospital _hospitalInterface;

        public HospitalController(IHospital hospitalInterface)
        {
            _hospitalInterface = hospitalInterface ?? throw new ArgumentNullException(nameof(hospitalInterface));
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
    }
}

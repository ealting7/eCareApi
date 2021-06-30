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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctor _doctorInterface;

        public DoctorController(IDoctor doctorInterface)
        {
            _doctorInterface = doctorInterface ?? throw new ArgumentNullException(nameof(doctorInterface));
        }

        [HttpGet("labqsearch/{first}/{last}/{state}")]
        public IActionResult GetProviderSearchFirstLastState(string first, string last, string state)
        {
            var doctors = _doctorInterface.GetDoctors(first, last, state);

            if (doctors == null)
            {
                return NoContent();
            }

            var returnDrs = new List<Doctor>();

            foreach (var dr in doctors)
            {
                returnDrs.Add(new Doctor
                {
                    providerAddressId = dr.providerAddressId,
                    pcpId = dr.pcpId,
                    firstName = dr.firstName,
                    lastName = dr.lastName,
                    practiceName = dr.practiceName,
                    npi = dr.npi,
                    specialtyDesc = dr.specialtyDesc,
                    emailAddress = dr.emailAddress,
                    phoneNumber = dr.phoneNumber,
                    address1 = dr.address1,
                    address2 = dr.address2,
                    city = dr.city,
                    stateAbbrev = dr.stateAbbrev,
                    zip = dr.zip
                });
            }


            return Ok(returnDrs);
        }

        [HttpGet("labqsearch/{id}/{phoneNumber}")]
        public IActionResult GetProviderUsingProvAddrId(string id, string phoneNumber)
        {
            var doctor = _doctorInterface.GetDoctorUsingProvAddrId(id, phoneNumber);

            if (doctor == null)
            {
                return NoContent();

            }

            var returnDr = new Doctor();

            if (!doctor.pcpId.Equals(Guid.Empty))
            { 
                returnDr.pcpId = doctor.pcpId;
                returnDr.firstName = doctor.firstName;
                returnDr.lastName = doctor.lastName;
                returnDr.practiceName = doctor.practiceName;
                returnDr.npi = doctor.npi;
                returnDr.specialtyDesc = doctor.specialtyDesc;
                returnDr.emailAddress = doctor.emailAddress;
                returnDr.phoneNumber = doctor.phoneNumber;
                returnDr.address1 = doctor.address1;
                returnDr.address2 = doctor.address2;
                returnDr.city = doctor.city;
                returnDr.stateAbbrev = doctor.stateAbbrev;
                returnDr.zip = doctor.zip; 
            }

            return Ok(returnDr);
        }
    }
}

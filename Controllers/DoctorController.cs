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

        
        [HttpGet("get/providers/pcp/{id}")]
        public IActionResult GetProviderUsingPcpId(string id)
        {
            var doctor = _doctorInterface.GetProviderUsingPcpId(id);

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
            }

            return Ok(returnDr);
        }


        [HttpGet("get/provider/addresses/{id}")]
        public IActionResult GetProviderAddresses(string id)
        {
            var addrs = _doctorInterface.GetAllProviderAddresses(id);

            if (addrs == null)
            {
                return NoContent();

            }

            var returnAddrs = new List<Doctor>();

            foreach (var address in addrs)
            {
                if (!address.pcpId.Equals(Guid.Empty))
                {
                    returnAddrs.Add(new Doctor
                    {
                        pcpId = address.pcpId,
                        firstName = address.firstName,
                        lastName = address.lastName,
                        practiceName = address.practiceName,
                        npi = address.npi,
                        specialtyDesc = address.specialtyDesc,
                        emailAddress = address.emailAddress,
                        address1 = address.address1,
                        address2 = address.address2,
                        street = address.address1 + address.address2,
                        city = address.city,
                        stateAbbrev = address.stateAbbrev,
                        zip = address.zip
                    });

                }
            }



            return Ok(returnAddrs);
        }

        [HttpGet("get/provider/{id}")]
        public IActionResult getProvider(string id)
        {
            var provider = _doctorInterface.getProvider(id);

            if (provider == null)
            {
                return NoContent();

            }
           
            return Ok(provider);
        }



        [HttpGet("dbms/get/medicalreviews/{provId}")]
        public IActionResult getProviderMedicalReviews(string provId)
        {
            var provider = _doctorInterface.getProviderMedicalReviews(provId);

            if (provider == null)
            {
                return NoContent();

            }

            return Ok(provider);
        }

        [HttpGet("dbms/get/medicalreviews/{provId}/{medRevReqId}")]
        public IActionResult getProviderMedicalReviews(string provId, int medRevReqId)
        {
            var provider = _doctorInterface.getProviderMedicalReview(provId, medRevReqId);

            if (provider == null)
            {
                return NoContent();

            }

            return Ok(provider);
        }



        [HttpPost("dbms/provider/add")]
        public IActionResult addProviderNew(Doctor mdDoctor)
        {
            var provider = _doctorInterface.addProviderNew(mdDoctor);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }

        [HttpPost("dbms/provider/update/npi")]
        public IActionResult updateProviderNpi(Doctor mdDoctor)
        {
            var provider = _doctorInterface.updateProviderNpi(mdDoctor);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }

        [HttpPost("dbms/provider/update/address")]
        public IActionResult updateProviderAddress(Doctor mdDoctor)
        {
            var provider = _doctorInterface.updateProviderAddress(mdDoctor);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }

        [HttpPost("dbms/provider/update/phone")]
        public IActionResult updateProviderPhone(Doctor mdDoctor)
        {
            var provider = _doctorInterface.updateProviderPhone(mdDoctor);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }

        [HttpPost("dbms/provider/update/specialty")]
        public IActionResult updateProviderSpecialty(Doctor mdDoctor)
        {
            var provider = _doctorInterface.updateProviderSpecialty(mdDoctor);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }


        [HttpPost("dbms/medicalreviews/questions/add")]
        public IActionResult addMedicalReviewQuestion(MedicalReview medReview)
        {
            var provider = _doctorInterface.addMedicalReviewQuestion(medReview);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }

        [HttpPost("dbms/medicalreviews/determination/add")]
        public IActionResult addMedicalReviewDetermination(MedicalReview medReview)
        {
            var provider = _doctorInterface.addMedicalReviewDetermination(medReview);

            if (provider == null)
            {
                return NoContent();
            }

            return Ok(provider);
        }




        [HttpPost("dbms/provider/remove")]
        public IActionResult removeProvider(Doctor mdDoctor)
        {
            var removed = _doctorInterface.disableProvider(mdDoctor);

            return Ok(removed);
        }


    }
}

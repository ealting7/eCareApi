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
    public class UtilizationManagementController : Controller
    {

        private readonly IUtilizationManagement _umInterface;

        public UtilizationManagementController(IUtilizationManagement umInterface)
        {
            _umInterface = umInterface ?? throw new ArgumentNullException(nameof(umInterface));
        }


        [HttpGet("dbms/get/reftypes")]
        public IActionResult GetReferralTypes()
        {
            var apptTypes = _umInterface.GetReferralTypes();

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }


        [HttpGet("dbms/get/contexts")]
        public IActionResult GetContexts()
        {
            var apptTypes = _umInterface.GetContexts();

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/categories")]
        public IActionResult GetCategories()
        {
            var categories = _umInterface.GetCategories();

            if (categories == null)
            {
                return NoContent();
            }

            return Ok(categories);
        }

        [HttpGet("dbms/get/refreasons")]
        public IActionResult GetReferralReasons()
        {
            var reasons = _umInterface.GetReferralReasons();

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
        }
        
        [HttpGet("dbms/get/refcurrstat")]
        public IActionResult GetReferralCurrentStatus()
        {
            var currStatus = _umInterface.GetReferralCurrentStatus();

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }
        
        [HttpGet("dbms/get/refcurrstat/{actionId}/actionreasons")]
        public IActionResult GetReferralActionReasons(string actionId)
        {
            var currStatus = _umInterface.GetReferralActionReasons(actionId);

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }


        [HttpPost("dbms/get/providers")]
        public IActionResult GetProviderSearch(SearchParams srchParams)
        {
            var reasons = _umInterface.GetProviderSearch(srchParams);

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
        }

        [HttpGet("dbms/get/providers/{id}/{phoneNumber}")]
        public IActionResult GetProvider(string id, string phoneNumber)
        {
            var doctor = _umInterface.GetProvider(id, phoneNumber);


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


        [HttpGet("dbms/get/providers/address/{id}")]
        public IActionResult GetProviderUsingProviderAddressId(string id)
        {
            var doctor = _umInterface.GetProviderUsingProviderAddressId(id);


            if (doctor == null)
            {
                return NoContent();

            }

            var returnDr = new Doctor();

            if (!doctor.pcpId.Equals(Guid.Empty))
            {
                returnDr.pcpId = doctor.pcpId;
                returnDr.fullName = doctor.fullName;
                returnDr.firstName = doctor.firstName;
                returnDr.lastName = doctor.lastName;
                returnDr.practiceName = doctor.practiceName;
                returnDr.npi = doctor.npi;
                returnDr.specialtyDesc = doctor.specialtyDesc;
                returnDr.emailAddress = doctor.emailAddress;
                returnDr.address1 = doctor.address1;
                returnDr.address2 = doctor.address2;
                returnDr.city = doctor.city;
                returnDr.stateAbbrev = doctor.stateAbbrev;
                returnDr.zip = doctor.zip;
            }

            return Ok(returnDr);
        }




        [HttpGet("dbms/get/patients/{id}/referrals")]
        public IActionResult GetPatientReferrals(string id)
        {
            var referrals = _umInterface.GetPatientReferrals(id);


            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}")]
        public IActionResult GetPatientReferral(string id, string refId)
        {
            var referrals = _umInterface.GetPatientReferral(id, refId);


            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }




        
        [HttpPost("dbms/update/general")]
        public IActionResult updateGeneralUm(Utilization util)
        {
            var reasons = _umInterface.updateGeneralUm(util);

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
        }

        
        [HttpPost("dbms/update/refprovider")]
        public IActionResult updateProviderUm(Utilization util)
        {
            var providers = _umInterface.updateProviderUm(util);

            if (providers == null)
            {
                return NoContent();
            }

            return Ok(providers);
        }

        
        [HttpPost("dbms/update/refcodes")]
        public IActionResult updateCodesUm(Utilization util)
        {
            var codes = _umInterface.updateCodesUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }
    }
}

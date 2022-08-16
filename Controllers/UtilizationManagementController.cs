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

        [HttpGet("dbms/get/mdspecialty")]
        public IActionResult getMdSpecialty()
        {
            var specialties = _umInterface.getMdSpecialties();

            if (specialties == null)
            {
                return NoContent();
            }

            return Ok(specialties);
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

        [HttpGet("dbms/get/refcurrstat/refactionto/{userId}")]
        public IActionResult getAssignToUsers(string userId)
        {
            var currStatus = _umInterface.getAssignToUsers(userId);

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }
        
        [HttpGet("dbms/get/utilbedtypes")]
        public IActionResult GetReferralUtilizationBedTypes()
        {
            var currStatus = _umInterface.getReferralUtilizationBedTypes();

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }

        [HttpGet("dbms/get/utildecisions")]
        public IActionResult GetReferralUtilizationDecisions()
        {
            var currStatus = _umInterface.getReferralUtilizationDecisions();

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }

        [HttpGet("dbms/get/utildecisionbys")]
        public IActionResult GetReferralUtilizationDecisionBys()
        {
            var bys = _umInterface.getReferralUtilizationDecisionBys();

            if (bys == null)
            {
                return NoContent();
            }

            return Ok(bys);
        }
        
        [HttpGet("dbms/get/utildenialreaons")]
        public IActionResult GetReferralUtilizationDenialReasons()
        {
            var bys = _umInterface.getReferralUtilizationDenialReasons();

            if (bys == null)
            {
                return NoContent();
            }

            return Ok(bys);
        }

        [HttpGet("dbms/get/utilperiods")]
        public IActionResult GetReferralUtilizationPeriods()
        {
            var bys = _umInterface.getReferralUtilizationPeriods();

            if (bys == null)
            {
                return NoContent();
            }

            return Ok(bys);
        }

        [HttpGet("dbms/get/utilnotebillcodes")]
        public IActionResult getReferralUtilizationNoteBillCodes()
        {
            var codes = _umInterface.getReferralUtilizationNoteBillCodes();

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpGet("dbms/get/utilsavingunits")]
        public IActionResult getReferralUtilizationSavingUnits()
        {
            var currStatus = _umInterface.getReferralUtilizationSavingUnits();

            if (currStatus == null)
            {
                return NoContent();
            }

            return Ok(currStatus);
        }

        [HttpGet("dbms/get/utilrequestsendto")]
        public IActionResult getReferralRequestSendToUsers()
        {
            var users = _umInterface.getReferralRequestSendToUsers();

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpGet("dbms/get/utilrequestquestanswer/{questId}")]
        public IActionResult getReferralRequestQuestionAnswer(string questId)
        {
            var users = _umInterface.getReferralRequestQuestionAnswer(questId);

            if (users == null)
            {
                return NoContent();
            }

            return Ok(users);
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


        [HttpPost("dbms/get/facilities")]
        public IActionResult getFacilitiesSearch(SearchParams srchParams)
        {
            var reasons = _umInterface.getFacilitiesSearch(srchParams);

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
        }

        [HttpGet("dbms/get/facilities/{id}")]
        public IActionResult getFacility(string id)
        {
            var facility = _umInterface.getFacility(id);

            if (facility == null)
            {
                return NoContent();

            }
            
            return Ok(facility);
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
        
        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/utilzations/{utilid}")]
        public IActionResult GetReferralUtilizationItem(string memId, string refId, string utilid)
        {
            var referrals = _umInterface.getReferralUtilizationItem(memId, refId, utilid);


            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }

        [HttpPost("dbms/get/patients/referrals/utilizations/pendreviewstatus")]
        public IActionResult priorUtilizationIsPendedReadyForReview(Utilization util)
        {
            var referrals = _umInterface.priorUtilizationIsPendedReadyForReview(util);

            return Ok(referrals);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/action/{actid}")]
        public IActionResult GetReferralActionItem(string memId, string refId, string actid)
        {
            var referrals = _umInterface.getReferralActionItem(memId, refId, actid);


            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/letters/")]
        public IActionResult getReferralLetters(string id, string refId)
        {
            Guid memberId = Guid.Empty;

            if (Guid.TryParse(id, out memberId))
            {

                var letters = _umInterface.getReferralLetters(refId, memberId);

                if (letters == null)
                {
                    return NoContent();

                }

                Utilization referral = new Utilization();
                referral.letters = new List<Letter>();
                referral.letters = letters;

                return Ok(referral);

            } else
            {
                return NoContent();
            }
        }        

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/letters/{letterId}")]
        public IActionResult getReferralLetterItem(string memId, string refId, string letterId)
        {
            var referrals = _umInterface.getReferralLetterItem(letterId);


            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/fax/{faxId}")]
        public IActionResult getReferralFaxItem(string memId, string refId, string faxId)
        {
            var referrals = _umInterface.getReferralFaxItem(faxId);

            if (referrals == null)
            {
                return NoContent();

            }

            return Ok(referrals);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/suspendnotes/")]
        public IActionResult getReferralUtilizationSuspendNotes(string memId, string refId)
        {

            var notes = _umInterface.getReferralUtilizationSuspendNotes(refId);

            if (notes == null)
            {
                return NoContent();

            }

            return Ok(notes);
        }

        [HttpGet("dbms/get/patients/{id}/referrals/{refId}/suspendnotes/{noteId}")]
        public IActionResult getReferralUtilizationSuspendNote(string memId, string refId, string noteId)
        {

            var notes = _umInterface.getReferralUtilizationSuspendNote(noteId);

            if (notes == null)
            {
                return NoContent();

            }

            return Ok(notes);
        }





        [HttpGet("dbms/get/form/data/claimdatarpt")]
        public IActionResult getClaimDataReportAssets()
        {
            var assets = _umInterface.getClaimDataReportAssets();

            if (assets == null)
            {
                return NoContent();
            }

            return Ok(assets);
        }


        [HttpPost("dbms/get/datamined/claims/")]
        public IActionResult getClaimDataMinedReportMatches(ClaimDataMine reportParams)
        {
            var reportId = _umInterface.loadDataMinedClaims(reportParams);

            if (reportId > 0)
            {
                return NoContent();
            }

            return Ok(reportId);
        }




        
        [HttpPost("dbms/get/referralnumber")]
        public IActionResult createUmReferralNumber(Utilization referral)
        {
            var refNumber = _umInterface.createUmReferralNumber(referral);

            if (refNumber == null)
            {
                return NoContent();
            }

            return Ok(refNumber);
        }


        [HttpGet("dbms/update/refauthnumber/{refNum}")]
        public IActionResult updateUmAuthNumber(string refNum)
        {
            var reasons = _umInterface.updateUmAuthNumber(refNum);

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
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

        [HttpPost("dbms/update/reffacility")]
        public IActionResult updateFacilityUm(Utilization util)
        {
            var providers = _umInterface.updateFacilityUm(util);

            if (providers == null)
            {
                return NoContent();
            }

            return Ok(providers);
        }

        [HttpPost("dbms/update/reffacility/address")]
        public IActionResult updateFacilityAddress(Utilization util)
        {
            var providers = _umInterface.updateFacilityAddress(util);

            if (providers == null)
            {
                return NoContent();
            }

            return Ok(providers);
        }

        [HttpPost("dbms/update/reffacility/npi")]
        public IActionResult updateFacilityNpi(HospitalFacility facility)
        {
            var providers = _umInterface.updateFacilityNpi(facility);

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

        [HttpPost("dbms/add/refactions")]
        public IActionResult addActionUm(Utilization util)
        {
            var codes = _umInterface.addActionUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/add/refutilizations")]
        public IActionResult addUtilizationsUm(Utilization util)
        {
            var codes = _umInterface.addUtilizationsUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/update/refutilizations")]
        public IActionResult updateUtilizationsUm(Utilization util)
        {
            var codes = _umInterface.updateUtilizationsUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/add/refutilizationnote")]
        public IActionResult addUtilizationsUmNote(Note utilNote)
        {
            var codes = _umInterface.addUtilizationsUmNote(utilNote);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/suspend/refutilizationnote")]
        public IActionResult suspendUtilizationsUmNote(Note utilNote)
        {
            var codes = _umInterface.suspendUtilizationsUmNote(utilNote);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/suspend/refutilizationnote/remove")]
        public IActionResult removeSuspendNote(Note utilNote)
        {
            bool removed = _umInterface.removeSuspendNote(utilNote);

            return Ok(removed);
        }

        [HttpPost("dbms/add/refutilizationsaving")]
        public IActionResult addUtilizationsUmSaving(Saving utilSaving)
        {
            var codes = _umInterface.addUtilizationsUmSaving(utilSaving);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/add/refutilizationmedrevreq")]
        public IActionResult addUtilizationsUmMedicalReviewRequest(Utilization util)
        {
            var codes = _umInterface.addUtilizationsUmMedicalReviewRequest(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/add/refrequest")]
        public IActionResult addUtilizationsUmRequest(UtilizationRequest utilReq)
        {
            var codes = _umInterface.addUtilizationsUmRequest(utilReq);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }        

        [HttpPost("dbms/remove/refutilizations")]
        public IActionResult removeUtilizationsUm(Utilization util)
        {
            var codes = _umInterface.removeUtilizationsUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }

        [HttpPost("dbms/remove/refletters")]
        public IActionResult removeLetterUm(Utilization util)
        {
            var codes = _umInterface.removeLetterUm(util);

            if (codes == null)
            {
                return NoContent();
            }

            return Ok(codes);
        }
        

    }
}

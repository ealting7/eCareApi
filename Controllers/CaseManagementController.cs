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
    public class CaseManagementController : Controller
    {
        private readonly ICaseManagement _cmInterface;

        public CaseManagementController(ICaseManagement cmInterface)
        {
            _cmInterface = cmInterface ?? throw new ArgumentNullException(nameof(cmInterface));
        }

        
        [HttpGet("dbms/get/caseowners")]
        public IActionResult GetReferralTypes()
        {
            var apptTypes = _cmInterface.getCmCaseOwners();

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/caseowners/{caseOwnerId}/cases")]
        public IActionResult getCaseOwnerCases(string caseOwnerId)
        {
            var apptTypes = _cmInterface.getCaseOwnerCases(caseOwnerId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/cm/patients/{patId}")]
        public IActionResult getCmPatient(string patId)
        {
            var apptTypes = _cmInterface.getCmPatient(patId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpPost("dbms/get/cm/patients/search")]
        public IActionResult getCmPatientSearch(Patient srchPatients)
        {
            var reasons = _cmInterface.getCmPatientSearch(srchPatients);

            if (reasons == null)
            {
                return NoContent();
            }

            return Ok(reasons);
        }

        [HttpGet("dbms/get/cm/patients/{patId}/reports")]
        public IActionResult getCmPatientReports(string patId)
        {
            var rpts = _cmInterface.getCmPatientReports(patId);

            if (rpts == null)
            {
                return NoContent();
            }

            return Ok(rpts);
        }

        [HttpGet("dbms/get/cm/patients/{patId}/reports/comprehensive/{compId}")]
        public IActionResult getCmPatientReports(string patId, int compId)
        {
            var rpts = _cmInterface.getCmPatientComphrehensiveReport(patId, compId);

            if (rpts == null)
            {
                return NoContent();
            }

            return Ok(rpts);
        }

        [HttpGet("dbms/get/cm/patients/{patId}/bills")]
        public IActionResult getCmPatientBills(string patId)
        {
            var bills = _cmInterface.getCmPatientBills(patId);

            if (bills == null)
            {
                return NoContent();
            }

            return Ok(bills);
        }

        [HttpGet("dbms/get/cm/patients/{patId}/bills/{billId}")]
        public IActionResult getCmPatientBills(string patId, string billId)
        {
            var bill = _cmInterface.getCmPatientBill(patId, billId);

            if (bill == null)
            {
                return NoContent();
            }

            return Ok(bill);
        }

        [HttpGet("dbms/get/cm/patients/{patId}/bills/historical")]
        public IActionResult getCmPatientHistoricalBills(string patId)
        {
            var bills = _cmInterface.getCmPatientHistoricalBills(patId);

            if (bills == null)
            {
                return NoContent();
            }

            return Ok(bills);
        }


        [HttpGet("dbms/get/cm/patients/reports/comprehensive/notes/{noteType}/{followupId}")]
        public IActionResult getComprehensiveReportNote(string noteType, string followupId)
        {
            var note = _cmInterface.getComprehensiveReportNote(noteType, followupId);

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }



        [HttpPost("dbms/get/cm/patients/reports/comprehensive/notes/historical")]
        public IActionResult getComprehensiveReportHistoricalNote(Note lcmNote)
        {
            var rpts = _cmInterface.getComprehensiveReportHistoricalNote(lcmNote);

            if (rpts == null)
            {
                return NoContent();
            }

            return Ok(rpts);
        }



        [HttpPost("dbms/cm/reports/flipcases")]
        public IActionResult flipCmCaseReports(Case report)
        {
            var rpts = _cmInterface.flipCmCaseReports(report);

            if (rpts == null)
            {
                return NoContent();
            }

            return Ok(rpts);
        }

        [HttpPost("dbms/cm/reports/remove/referral")]
        public IActionResult removeCmCaseReportReferral(Case report)
        {
            var rpt = _cmInterface.removeCmCaseReportReferral(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/add/referral")]
        public IActionResult addCmCaseReportReferral(Case report)
        {
            var rpt = _cmInterface.addCmCaseReportReferral(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/remove/facility")]
        public IActionResult removeCmCaseReportFacility(LcmReports report)
        {
            var rpt = _cmInterface.removeCmCaseReportFacility(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/remove/code")]
        public IActionResult removeCmCaseReportCode(LcmReports report)
        {
            var rpt = _cmInterface.removeCmCaseReportCode(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/add/facility")]
        public IActionResult addCmCaseReportFacility(LcmReports report)
        {
            var facility = _cmInterface.addCmCaseReportFacility(report);

            if (facility == null)
            {
                return NoContent();
            }

            return Ok(facility);
        }

        [HttpPost("dbms/cm/reports/add/code")]
        public IActionResult addCmCaseReportCode(LcmReports report)
        {
            var facility = _cmInterface.addCmCaseReportCode(report);

            if (facility == null)
            {
                return NoContent();
            }

            return Ok(facility);
        }

        [HttpPost("dbms/cm/reports/update/original")]
        public IActionResult updateCmCaseReportOriginal(LcmReports report)
        {
            var rpt = _cmInterface.updateCmCaseReportOriginal(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/update/updated")]
        public IActionResult updateCmCaseReportUpdated(LcmReports report)
        {
            var rpt = _cmInterface.updateCmCaseReportUpdated(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/update/additional")]
        public IActionResult updateCmCaseReportAdditional(LcmReports report)
        {
            var rpt = _cmInterface.updateCmCaseReportAdditional(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

        [HttpPost("dbms/cm/reports/update/notes")]
        public IActionResult updateCmCaseReportNotes(LcmReports report)
        {
            var rpt = _cmInterface.updateCmCaseReportNotes(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }



        [HttpPost("dbms/cm/reports/add/comprehensive")]
        public IActionResult createCmCaseReportComprehensive(LcmReports report)
        {
            var rpt = _cmInterface.createCmCaseReportComprehensive(report);

            if (rpt == null)
            {
                return NoContent();
            }

            return Ok(rpt);
        }

    }
}

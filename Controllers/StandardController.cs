using eCareApi.Entities;
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
    public class StandardController : ControllerBase
    {
        private readonly IStandard _standardInterface;

        public StandardController(IStandard standardInterface)
        {
            _standardInterface = standardInterface ?? throw new ArgumentNullException(nameof(standardInterface));
        }


        [HttpGet("states")]
        public IActionResult GetStates()
        {
            var states = _standardInterface.GetStates();

            if (states == null)
            {
                return NoContent();
            }

            return Ok(states);
        }


        [HttpGet("tpa")]
        public IActionResult GetTpas()
        {
            var tpa = _standardInterface.GetTpas();

            if (tpa == null)
            {
                return NoContent();
            }

            return Ok(tpa);
        }

        [HttpGet("tpa/{tpaId}/emailoptions/billing")]
        public IActionResult GetTpas(int tpaId)
        {
            var emailOptions = _standardInterface.GetTpaEmailBillingOptions(tpaId);

            if (emailOptions == null)
            {
                return NoContent();
            }

            return Ok(emailOptions);
        }




        [HttpGet("get/maritalstatus")]
        public IActionResult getMaritalStatuses()
        {
            var note = _standardInterface.getMaritalStatuses();

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }


        [HttpGet("get/phonetype")]
        public IActionResult getPhoneTypes()
        {
            var note = _standardInterface.getPhoneTypes();

            if (note == null)
            {
                return NoContent();
            }

            return Ok(note);
        }




        [HttpPost("dbms/email/billing/invoice")]
        public IActionResult emailBillingInvoice(Email invoice)
        {
            var emailInvoice = _standardInterface.emailBillingInvoice(invoice);

            if (emailInvoice == null)
            {
                return NoContent();
            }

            return Ok(emailInvoice);
        }



        [HttpPost("dbms/add/facility")]
        public IActionResult addFacility(HospitalFacility facility)
        {
            var emailInvoice = _standardInterface.addFacility(facility);

            if (emailInvoice == null)
            {
                return NoContent();
            }

            return Ok(emailInvoice);
        }


    }
}

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
    }
}

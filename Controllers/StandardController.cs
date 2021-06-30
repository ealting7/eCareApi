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
    [Route("api/[controller]")]
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
    }
}

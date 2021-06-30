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
    public class IcmsUserController : ControllerBase
    {
        private readonly IIcmsUser _icmsUserInterace;

        public IcmsUserController(IIcmsUser icmsInterface)
        {
            _icmsUserInterace = icmsInterface ?? throw new ArgumentNullException(nameof(icmsInterface));
        }

        [HttpGet]
        public IActionResult GetIcmsUsers()
        {
            var users = _icmsUserInterace.GetIcmsUsers();

            if (users == null)
            {
                return NotFound();
            }

            var returnUsers = new List<IcmsUser>();

            foreach(var usr in users)
            {
                returnUsers.Add(new IcmsUser
                {
                    UserId = usr.system_user_id,
                    FirstName = usr.system_user_first_name,
                    LastName = usr.system_user_last_name,
                    FullName = usr.system_user_first_name + " " + usr.system_user_last_name
                });
            }

            return Ok(returnUsers);
        }
    }
}

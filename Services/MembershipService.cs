using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class MembershipService
    {
        private readonly AspNetContext _aspNetContext;


        public MembershipService(AspNetContext aspNetContext)
        {
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
        }



        public bool validateUser(IcmsUser user)
        {
            bool userValidated = false;


            if (!user.UserId.Equals(Guid.Empty))
            {
                AspNetMembership aspUser = new AspNetMembership();


                aspUser = (from mems in _aspNetContext.AspNetMemberships
                           where mems.UserId.Equals(user.UserId)
                           select mems).FirstOrDefault();


                if (aspUser.UserId.Equals(user.UserId))
                {
                    userValidated = true;
                }
            }


            return userValidated;
        }

    }
}

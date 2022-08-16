using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class IcmsUserService : IIcmsUser
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspnetContext;

        public IcmsUserService(IcmsContext icmsContext, AspNetContext aspnetContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspnetContext = aspnetContext ?? throw new ArgumentNullException(nameof(aspnetContext));
        }

        public IEnumerable<SystemUser> GetIcmsUsers()
        {
            IEnumerable<SystemUser> users = Enumerable.Empty<SystemUser>();

            var items = _icmsContext.IcmsUsers
                            .Where(usr => !usr.user_inactive_flag &&
                                          usr.data_admin_flag &&
                                          usr.security_admin_flag)
                            .OrderBy(usr => usr.system_user_last_name)
                            .ThenBy(usr => usr.system_user_first_name);

            users = items.ToList();

            return users;
        }

        public IcmsUser getIcmsUser(string usrId)
        {

            IcmsUser usr = null;

            Guid sysUsrId = Guid.Empty;

            if (Guid.TryParse(usrId, out sysUsrId))
            {
                usr = getIcmsUserUsingSysId(sysUsrId);
            }

            if (usr == null)
            {
                usr = getIcmsUserUsingAspNetUser(sysUsrId);

                if (usr == null)
                {
                    usr = getAspNetUser(sysUsrId);
                }
            }

            return usr;
        }

        private IcmsUser getIcmsUserUsingSysId(Guid sysUsrId)
        {

            IcmsUser icmsUsr = null;

            icmsUsr = (

                    from sysUsr in _icmsContext.SystemUsers
                    where sysUsr.system_user_id.Equals(sysUsrId)
                    select new IcmsUser
                    {
                        UserId = sysUsr.system_user_id,
                        FirstName = sysUsr.system_user_first_name,
                        LastName = sysUsr.system_user_last_name,
                        FullName = sysUsr.system_user_first_name + " " + sysUsr.system_user_last_name,
                        emailAddress = sysUsr.email_address,
                    }
                )
                .FirstOrDefault();

            return icmsUsr;
        }
        private IcmsUser getIcmsUserUsingAspNetUser(Guid sysUsrId)
        {

            IcmsUser icmsUsr = null;

            Guid icmsSysUsrId = Guid.Empty;

            icmsSysUsrId = (

                    from icmsRef in _aspnetContext.IcmsUserReferences
                    where icmsRef.UserId.Equals(sysUsrId)
                    select icmsRef.icms_system_user_id.Value
                )
                .FirstOrDefault();

            if (!icmsSysUsrId.Equals(Guid.Empty))
            {

                icmsUsr = (

                        from sysUsr in _icmsContext.SystemUsers
                        where sysUsr.system_user_id.Equals(icmsSysUsrId)
                        select new IcmsUser
                        {
                            UserId = sysUsr.system_user_id,
                            FirstName = sysUsr.system_user_first_name,
                            LastName = sysUsr.system_user_last_name,
                            FullName = sysUsr.system_user_first_name + " " + sysUsr.system_user_last_name,
                            emailAddress = sysUsr.email_address,
                        }
                    )
                    .FirstOrDefault();
            }

            return icmsUsr;
        }
        private IcmsUser getAspNetUser(Guid sysUsrId)
        {

            IcmsUser userAspNet = null;

            userAspNet = (

                    from aspMem in _aspnetContext.AspNetMemberships

                    join aspusr in _aspnetContext.AspNetUsers
                    on aspMem.UserId equals aspusr.UserId

                    where aspMem.UserId.Equals(sysUsrId)
                    select new IcmsUser
                    {
                        UserId = aspMem.UserId,
                        FirstName = aspusr.UserName,
                        LastName = aspusr.UserName,
                        FullName = aspusr.UserName,
                        emailAddress = aspMem.Email
                    }
                )
                .FirstOrDefault();

            return userAspNet;
        }
    }
}

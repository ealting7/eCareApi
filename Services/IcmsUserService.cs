using eCareApi.Context;
using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class IcmsUserService : IIcmsUser
    {
        private readonly IcmsContext _databaseContext;

        public IcmsUserService(IcmsContext icmsContext)
        {
            _databaseContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }
        public IEnumerable<SystemUser> GetIcmsUsers()
        {
            IEnumerable<SystemUser> users = Enumerable.Empty<SystemUser>();

            var items = _databaseContext.IcmsUsers
                            .Where(usr => !usr.user_inactive_flag &&
                                          usr.data_admin_flag &&
                                          usr.security_admin_flag)
                            .OrderBy(usr => usr.system_user_last_name)
                            .ThenBy(usr => usr.system_user_first_name);

            users = items.ToList();

            return users;
        }
    }
}

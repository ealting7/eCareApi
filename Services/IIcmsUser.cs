using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IIcmsUser
    {
        IEnumerable<SystemUser> GetIcmsUsers();

        public IcmsUser getIcmsUser(string usrId);
    }
}

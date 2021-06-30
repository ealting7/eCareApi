using eCareApi.Entities;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IIcmsUser
    {
        IEnumerable<SystemUser> GetIcmsUsers();
    }
}

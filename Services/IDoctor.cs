using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IDoctor
    {
        IEnumerable<Doctor> GetDoctors(string first, string last, string state_abbrev);

        Doctor GetDoctorUsingProvAddrId(string id, string GetDoctorUsingProvAddrId);
    }
}

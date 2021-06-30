using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IHospital
    {
        IEnumerable<Hospital> GetCollectionFacilities();

        IEnumerable<HospitalFacility> GetLaboratoryFacilities();
    }
}

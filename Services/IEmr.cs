using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IEmr
    {

        public Patient getPatientAllergies(string patientId);
    }
}

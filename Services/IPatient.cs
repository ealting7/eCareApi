using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IPatient
    {
        IEnumerable<Member> GetPatientsUsingFirstLastDob(string firstName, string lastName, string dob);

        Patient GetPatientsUsingId(string id);

        IEnumerable<HospitalRace> GetPatientAncestryUsingId(string id);

        IEnumerable<MemberAddress> GetPatientAddress(string id, bool returnOneAddress);

        PhoneNumber GetPatientHomePhoneNumber(string id, bool returnOneNumber);

        Patient GetPatientInsuranceUsingId(string id);
    }
}

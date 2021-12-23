using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IPatient
    {
        IEnumerable<Member> GetPatientsUsingFirstLastDob(string firstName, string lastName, string dob);

        public List<Patient> searchPatients(Patient search);

        public Patient searchPatient(string patientId);

        public Patient searchPatientAge(string patientId);

        Patient GetPatientsUsingId(string id);

        IEnumerable<HospitalRace> GetPatientAncestryUsingId(string id);

        IEnumerable<MemberAddress> GetPatientAddress(string id, bool returnOneAddress);

        PhoneNumber GetPatientHomePhoneNumber(string id, bool returnOneNumber);

        Patient GetPatientInsuranceUsingId(string id);

        Note GetPatientMemberNotes(string id, string date);
    }
}

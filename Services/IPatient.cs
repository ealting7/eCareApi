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

        public Patient getPatientDemographics(string patientId);

        public Patient searchPatientAge(string patientId);

        Patient GetPatientsUsingId(string id);

        List<HospitalRace> GetPatientAncestryUsingId(string id);

        List<MemberAddress> GetPatientAddress(string id, bool returnOneAddress);

        public Patient getPatientAddressByAddressId(int addressId);

        PhoneNumber GetPatientHomePhoneNumber(string id, bool returnOneNumber);

        Patient GetPatientInsuranceUsingId(string id);

        Note GetPatientMemberNotes(string id, string date);

        public PatientAsset getPatientDashboardAssets();

        public List<Patient> patientBasicExists(Patient patient);

        public Patient addPatientForInpatient(Patient patient);

        public Patient addPatientBasic(Patient addPatient);

        public Patient updatePatientInfo(Patient addPatient);

        public Patient updatePatientEmail(Patient updatePatient);

        public Patient addPatientAddress(Patient addPatient);

        public Patient removePatientAddress(Patient addPatient);

        public Patient updatePatientAddress(Patient addPatient);

        public Patient addPatientPhone(Patient addPatient);

        public Patient removePatientPhone(Patient addPatient);

        public Patient updatePatientEmployer(Patient updatePatient);

        public Patient updatePatientInsurance(Patient updatePatient);

        public Patient addPatientHospital(Patient addPatient);

        public Patient removePatientHospital(Patient addPatient);

        public Patient updatePatientCmIdentification(Patient updatePatient);

        public Patient addPatientCaseOwner(Patient addPatient); 

        public Patient removePatientCaseOwner(Patient addPatient); 
    }
}

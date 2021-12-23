using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IHospital
    {

        public List<Admission> getHospitalDepartments(string hospitalId);
        public List<Admission> getHospitalInpatientAdmissions(Admission search);

        public Admission getInpatientAdmission(string admitId);

        public List<Admission> getPatientInpatientAdmissions(string patientId);

        public Admission getPatientCurrentAdmission(string patientId);

        public List<Hospital> GetHospitals();

        IEnumerable<Hospital> GetCollectionFacilities();

        IEnumerable<HospitalFacility> GetLaboratoryFacilities();

        public CareplanAssessItem getCareplanAssessFormItems();

        public CareplanAssessItem getCareplanDiagnosisDomainClasses(string domainId);

        public List<AssessItem> getCareplanAssessBasicGenerals(int inpatientAdmissionId);
    }
}

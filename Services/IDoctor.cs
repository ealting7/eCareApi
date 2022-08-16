using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IDoctor
    {
        IEnumerable<Doctor> GetDoctors(string first, string last, string state_abbrev);

        Doctor GetDoctorUsingProvAddrId(string id, string GetDoctorUsingProvAddrId);

        Doctor GetProviderUsingPcpId(string id);

        List<Doctor> GetAllProviderAddresses(string id);

        public Doctor getProvider(string id);


        public List<Utilization> getProviderMedicalReviews(string provId);

        public Utilization getProviderMedicalReview(string provId, int medRevReqId);


        public Doctor addProviderNew(Doctor mdDoctor);

        public Doctor updateProviderNpi(Doctor mdDoctor);
        public Doctor updateProviderAddress(Doctor mdDoctor);

        public Doctor updateProviderPhone(Doctor mdDoctor); 

        public Doctor updateProviderSpecialty(Doctor mdDoctor);

        public Utilization addMedicalReviewQuestion(MedicalReview medReview);
        public Utilization addMedicalReviewDetermination(MedicalReview medReview);



        public bool disableProvider(Doctor mdoctor);
    }
}

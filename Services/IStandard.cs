using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IStandard
    {
        IEnumerable<State> GetStates();

        IEnumerable<Tpas> GetTpas();

        Tpas GetTpa(int id);

        public List<HospitalMaritalStatusTypes > getMaritalStatuses();

        public List<PhoneType> getPhoneTypes();

        public Email GetTpaEmailBillingOptions(int tpaId);



        public Email emailBillingInvoice(Email invoice);

        public HospitalFacility addFacility(HospitalFacility facility);
    }
}

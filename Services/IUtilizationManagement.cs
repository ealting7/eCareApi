using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IUtilizationManagement
    {
        public List<rReferralType> GetReferralTypes();

        public List<rReferralContext> GetContexts();

        public List<rReferralCategory> GetCategories();

        public List<rReferralReason> GetReferralReasons();

        public List<rCurrentStatus> GetReferralCurrentStatus();

        public List<rReferralPendReason> GetReferralActionReasons(string actionId);

        public List<Doctor> GetProviderSearch(SearchParams srchParams);

        public Doctor GetProvider(string id, string phoneNumber);

        public Doctor GetProviderUsingProviderAddressId(string id);

        public List<Utilization> GetPatientReferrals(string id);

        public Utilization GetPatientReferral(string id, string refId);

        public Utilization updateGeneralUm(Utilization util);

        public Utilization updateProviderUm(Utilization util);

        public Utilization updateCodesUm(Utilization util);
    }
}

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

        public List<Doctor> getMdSpecialties();

        public List<rCurrentStatus> GetReferralCurrentStatus();

        public List<rReferralPendReason> GetReferralActionReasons(string actionId);

        public List<IcmsUser> getAssignToUsers(string userId);

        public List<rBedDayType> getReferralUtilizationBedTypes();

        List<rReferralDecision> getReferralUtilizationDecisions();

        List<IcmsUser> getReferralUtilizationDecisionBys();

        public List<rDenialReason> getReferralUtilizationDenialReasons();

        public List<rUtilizationVisitPeriod> getReferralUtilizationPeriods();

        public List<BillingCodes> getReferralUtilizationNoteBillCodes();

        public List<rSavingsUnit> getReferralUtilizationSavingUnits();

        public List<IcmsUser> getReferralRequestSendToUsers();

        public MedicalReview getReferralRequestQuestionAnswer(string questId);

        public List<Doctor> GetProviderSearch(SearchParams srchParams);

        public Doctor GetProvider(string id, string phoneNumber);

        public Doctor GetProviderUsingProviderAddressId(string id);

        public List<HospitalFacility> getFacilitiesSearch(SearchParams srchParams);

        public HospitalFacility getFacility(string id);

        public List<Utilization> GetPatientReferrals(string id);

        public Utilization GetPatientReferral(string id, string refId);

        public ClaimDataMine getClaimDataReportAssets();

        public UtilizationItem getReferralUtilizationItem(string memId, string refId, string utilid);

        public bool priorUtilizationIsPendedReadyForReview(Utilization util);

        public UtilizationWorkflow getReferralActionItem(string memId, string refId, string actid);

        public List<Letter> getReferralLetters(string refId, Guid memId);

        public Letter getReferralLetterItem(string letterId);

        public Fax getReferralFaxItem(string faxId);

        public List<Note> getReferralUtilizationSuspendNotes(string refId);

        public Note getReferralUtilizationSuspendNote(string noteId);

        public int loadDataMinedClaims(ClaimDataMine reportParams);


        public Utilization createUmReferralNumber(Utilization referral);

        public Utilization updateUmAuthNumber(string refNum);

        public Utilization updateGeneralUm(Utilization util);

        public Utilization updateProviderUm(Utilization util);

        public Utilization updateFacilityUm(Utilization util);

        public Utilization updateFacilityAddress(Utilization util);

        public Utilization updateFacilityNpi(HospitalFacility facility);

        public Utilization updateCodesUm(Utilization util);

        public Utilization addActionUm(Utilization util);

        public Utilization addUtilizationsUm(Utilization util);

        public Utilization updateUtilizationsUm(Utilization util);

        public Utilization addUtilizationsUmNote(Note utilNote);

        public List<Note> suspendUtilizationsUmNote(Note utilNote); 

        public Utilization addUtilizationsUmSaving(Saving utilSaving);

        public Utilization addUtilizationsUmMedicalReviewRequest(Utilization util);

        public Utilization addUtilizationsUmRequest(UtilizationRequest utilReq);

        public Utilization removeUtilizationsUm(Utilization util);

        public Utilization removeLetterUm(Utilization util);

        public bool removeSuspendNote(Note utilNote);
    }
}

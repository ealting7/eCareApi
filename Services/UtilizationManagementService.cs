using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Services
{
    public class UtilizationManagementService : IUtilizationManagement
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _dataStagingContext;


        public UtilizationManagementService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext dataStagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _dataStagingContext = dataStagingContext ?? throw new ArgumentNullException(nameof(dataStagingContext));
        }



        public List<rReferralType> GetReferralTypes()
        {
            List<rReferralType> appointmentTypes = new List<rReferralType>();
            byte disabledByte = 0;

            appointmentTypes = (
                from refTypes in _icmsContext.ReferralTypes
                where refTypes.disabled.Equals(disabledByte)
                select refTypes
                )
                .Distinct()
                .OrderBy(refType => refType.label)
                .ToList();


            return appointmentTypes;
        }

        public List<rReferralContext> GetContexts()
        {
            List<rReferralContext> contexts = new List<rReferralContext>();

            contexts = (
                from refContx in _icmsContext.ReferralContexts
                select refContx
                )
                .Distinct()
                .OrderBy(refType => refType.listorder)
                .ToList();


            return contexts;
        }

        public List<rReferralCategory> GetCategories()
        {
            List<rReferralCategory> categories = new List<rReferralCategory>();

            categories = (
                from refCatgor in _icmsContext.ReferralCategories
                select refCatgor
                )
                .Distinct()
                .OrderBy(refType => refType.referral_category)
                .ToList();


            return categories;
        }

        public List<rReferralReason> GetReferralReasons()
        {
            List<rReferralReason> reasons = new List<rReferralReason>();

            reasons = (
                from refReas in _icmsContext.ReferralReasons
                select refReas
                )
                .Distinct()
                .OrderBy(refType => refType.label.Trim())
                .ToList();


            return reasons;
        }

        public List<Doctor> getMdSpecialties()
        {
            List<Doctor> specialties = new List<Doctor>();

            specialties = (
                    from mdSpec in _icmsContext.Specialtys
                    orderby mdSpec.specialty_desc
                    select new Doctor
                    {
                        specialtyId = mdSpec.specialty_id,
                        specialtyDesc = mdSpec.specialty_desc
                    }
                )
                .ToList();


            return specialties;
        }

        public List<rCurrentStatus> GetReferralCurrentStatus()
        {
            List<rCurrentStatus> reasons = new List<rCurrentStatus>();

            reasons = (
                        from refcurrstat in _icmsContext.ReferralCurrentStatus
                        select refcurrstat
                      )
                      .Distinct()
                      .OrderBy(refType => refType.listorder)
                      .ToList();


            return reasons;
        }

        public List<rReferralPendReason> GetReferralActionReasons(string actionId)
        {
            List<rReferralPendReason> reasons = new List<rReferralPendReason>();

            int currStatId = int.Parse(actionId);

            reasons = (
                        from actreason in _icmsContext.ReferralActionReasons
                        where actreason.currentstatus_id.Equals(currStatId)
                        select actreason
                      )
                      .Distinct()
                      .OrderBy(refType => refType.listorder)
                      .ToList();


            return reasons;
        }

        public List<IcmsUser> getAssignToUsers(string userId)
        {
            List<IcmsUser> assignToUsers = null;

            Guid currentUserId = Guid.Empty;

            if (Guid.TryParse(userId, out currentUserId))
            {
                StandardService standSer = new StandardService(_icmsContext, _aspNetContext);
                IcmsUser addNurse = standSer.getAspUser(currentUserId);

                if (addNurse != null)
                { 
                    assignToUsers = new List<IcmsUser>();
                    
                    addNurse.FullName = addNurse.FullName + " (Me)";
                    assignToUsers.Add(addNurse);

                    getAssignToDoctors(ref assignToUsers);
                }

            }


            return assignToUsers;
        }

        private void getAssignToDoctors(ref List<IcmsUser> assignToUsers)
        {
            try
            {
                List<SystemUser> dbDoctors = (
                                                from sysusr in _icmsContext.SystemUsers
                                                where (sysusr.review_md > 0 ||
                                                sysusr.appeal_md > 0 ||
                                                sysusr.md_review_company > 0)// ||
                                                // sysusr.is_wishard_dr > 0 ||
                                                // sysusr.is_columbia_doctor > 0)
                                                && (!sysusr.user_inactive_flag || sysusr.user_inactive_flag.Equals(null))
                                                select sysusr
                                             )
                                             .ToList();

                if (dbDoctors != null)
                {

                    foreach (SystemUser doctor in dbDoctors)
                    {
                        IcmsUser addDoctor = new IcmsUser();
                        addDoctor.UserId = doctor.system_user_id;
                        addDoctor.FullName = doctor.system_user_first_name + ' ' + doctor.system_user_last_name;

                        assignToUsers.Add(addDoctor);
                    }
                }
            }
            catch(Exception ex)
            {

            }

        }

        public List<rBedDayType> getReferralUtilizationBedTypes()
        {
            List<rBedDayType> bedTypes = new List<rBedDayType>();

            bedTypes = (
                        from utilBedTypes in _icmsContext.rBedDayTypes
                        select utilBedTypes
                      )
                      .Distinct()
                      .OrderBy(bed => bed.label)
                      .ToList();


            return bedTypes;
        }

        public List<rReferralDecision> getReferralUtilizationDecisions()
        {
            List<rReferralDecision> decisions = new List<rReferralDecision>();

            decisions = (
                        from utilDecisions in _icmsContext.rReferralDecisions
                        select utilDecisions
                      )
                      .Distinct()
                      .OrderBy(bed => bed.listorder)
                      .ToList();


            return decisions;
        }

        public List<IcmsUser> getReferralUtilizationDecisionBys()
        {

            List<IcmsUser> users = new List<IcmsUser>();

            users = (from revItms in _icmsContext.ReviewTypeItemses
                     orderby revItms.is_default descending, revItms.name
                     select new IcmsUser
                     {
                         reviewTypeItemsId = revItms.review_type_items_id,
                         FullName = revItms.name,
                         isDefault = (revItms.is_default.HasValue) ? (revItms.is_default.Value > 0) ? true : false : false,
                         isDr = (revItms.is_dr.HasValue) ? (revItms.is_dr.Value > 0) ? true : false : false,
                         isThirdParty = (revItms.is_third_party.HasValue) ? (revItms.is_third_party.Value > 0) ? true : false : false
                     })
                      .ToList();

            return users;
        }

        public List<rDenialReason> getReferralUtilizationDenialReasons()
        {
            List<rDenialReason> denials = new List<rDenialReason>();

            denials = (
                        from utilDenyReas in _icmsContext.rDenialReasons
                        select utilDenyReas
                      )
                      .Distinct()
                      .OrderBy(bed => bed.listorder)
                      .ToList();


            return denials;
        }

        public List<rUtilizationVisitPeriod> getReferralUtilizationPeriods()
        {
            List<rUtilizationVisitPeriod> periods = new List<rUtilizationVisitPeriod>();

            periods = (
                        from utilVisits in _icmsContext.rUtilizationVisitPeriods
                        select utilVisits
                      )
                      .Distinct()
                      .OrderBy(visit => visit.label)
                      .ToList();


            return periods;
        }

        public List<BillingCodes> getReferralUtilizationNoteBillCodes()
        {
            List<BillingCodes> billCodes = null;

            BillingService billServ = new BillingService(_icmsContext, _dataStagingContext, _aspNetContext, null);
            billCodes = billServ.GetDbmsBillingCodes();

            return billCodes;
        }

        public List<rSavingsUnit> getReferralUtilizationSavingUnits()
        {

            List<rSavingsUnit> savingUnits = new List<rSavingsUnit>();

            savingUnits = (
                        from utilSavUnits in _icmsContext.rSavingsUnits
                        select utilSavUnits
                      )
                      .Distinct()
                      .OrderBy(unit => unit.units_label)
                      .ToList();


            return savingUnits;
        }


        public List<IcmsUser> getReferralRequestSendToUsers()
        {
            List<IcmsUser> users = null;

            users = (
                        from sysusr in _icmsContext.SystemUsers
                        where sysusr.review_md == 1
                        && (!sysusr.user_inactive_flag || sysusr.user_inactive_flag.Equals(null))
                        orderby sysusr.system_user_last_name, sysusr.system_user_first_name
                        select new IcmsUser
                        {
                            UserId = sysusr.system_user_id,
                            FirstName = sysusr.system_user_first_name,
                            LastName = sysusr.system_user_last_name,
                            FullName = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                            reviewMd = (sysusr.review_md.HasValue) ? (sysusr.review_md.Value.Equals(1)) ? true : false : false,
                            emailAddress = sysusr.email_address
                        }
                    )
                    .ToList();

            return users;
        }


        public List<ClinicalReviewBillDescriptions> getReferralCrBillDescriptions()
        {

            List<ClinicalReviewBillDescriptions> crDescripts = new List<ClinicalReviewBillDescriptions>();

            crDescripts = (
                        from descripts in _icmsContext.ClinicalReviewBillDescriptionses
                        select descripts
                      )
                      .Distinct()
                      .OrderBy(unit => unit.description)
                      .ToList();


            return crDescripts;
        }

        public string getCrBillDescription(int descriptionId)
        {

            string crDescripts = "";

            crDescripts = (
                        from descripts in _icmsContext.ClinicalReviewBillDescriptionses
                        where descripts.description_id.Equals(descriptionId)
                        select descripts.description
                      )
                      .FirstOrDefault();


            return crDescripts;
        }


        public MedicalReview getReferralRequestQuestionAnswer(string questId)
        {
            
            MedicalReview reviewQuestion = null;

            int questionId = 0;

            if (int.TryParse(questId, out questionId) && questionId > 0)
            {

                reviewQuestion = (

                        from mdQuest in _icmsContext.MdReviewQuestions
                        where mdQuest.md_review_question_id.Equals(questionId)
                        select new MedicalReview
                        {
                            mdReviewQuestionId = mdQuest.md_review_question_id,
                            questionNote = mdQuest.md_question_note,
                            answerNote = mdQuest.md_answer_note,
                        }
                    )
                    .FirstOrDefault();
            }

            return reviewQuestion;
        }



        public List<Doctor> GetProviderSearch(SearchParams srchParams)
        {
            List<Doctor> drs = new List<Doctor>();

            DoctorService docService = new DoctorService(_icmsContext, _aspNetContext, _dataStagingContext);

            var returnDrs = docService.GetDoctors(srchParams.firstName, srchParams.lastName, srchParams.stateAbbrev);


            if (returnDrs != null)
            {
                foreach(Doctor dr in returnDrs)
                {
                    drs.Add(dr);
                }
            }

            

            return drs;
        }

        public Doctor GetProvider(string id, string phoneNumber)
        {
            Doctor returnDoctor = new Doctor();
            int providerAddressId = 0;


            if (int.TryParse(id, out providerAddressId) &&
                providerAddressId > 0)
            {
                var doctor = (from drAddr in _icmsContext.PcpAddresses
                              join dr in _icmsContext.Pcps
                              on drAddr.pcp_id equals dr.pcp_id
                              join drSpecial in _icmsContext.PcpSpecialtys
                              on dr.pcp_id equals drSpecial.pcp_id
                              join Spec in _icmsContext.Specialtys
                              on drSpecial.specialty_id equals Spec.specialty_id
                              join drAddrConts in _icmsContext.PcpAddressContacts
                              on drAddr.provider_address_id equals drAddrConts.provider_address_id
                              join drConts in _icmsContext.PcpContacts
                              on drAddrConts.provider_contact_id equals drConts.provider_contact_id
                              join drContPhones in _icmsContext.PcpContactPhones
                              on drConts.provider_contact_id equals drContPhones.provider_contact_id
                              join drPhone in _icmsContext.PcpPhoneNumbers
                              on drContPhones.provider_phone_id equals drPhone.provider_phone_id
                              where drAddr.provider_address_id.Equals(providerAddressId)
                              select new Doctor
                              {
                                  pcpId = dr.pcp_id,
                                  firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                                  lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                                  practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                                  npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                                  specialtyDesc = (!string.IsNullOrWhiteSpace(Spec.specialty_desc)) ? Spec.specialty_desc : "",
                                  emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                                  phoneNumber = (!string.IsNullOrWhiteSpace(drPhone.phone_number)) ? drPhone.phone_number : "",
                                  address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                                  address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                                  city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                                  stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                                  zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                              }).ToList();

                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    returnDoctor = doctor.Where(dr => dr.phoneNumber.Equals(phoneNumber)).FirstOrDefault();
                }
                else
                {
                    returnDoctor = doctor.Where(dr => dr.phoneNumber.Equals(DBNull.Value) || dr.phoneNumber.Equals("")).FirstOrDefault();
                }
            }

            return returnDoctor;
        }

        public Doctor GetProviderUsingProviderAddressId(string id)
        {
            Doctor returnDoctor = new Doctor();
            int providerAddressId = 0;


            if (int.TryParse(id, out providerAddressId) &&
                providerAddressId > 0)
            {
                var doctor = (from drAddr in _icmsContext.PcpAddresses

                              join dr in _icmsContext.Pcps
                              on drAddr.pcp_id equals dr.pcp_id

                              join drSpecial in _icmsContext.PcpSpecialtys
                              on dr.pcp_id equals drSpecial.pcp_id

                              join Spec in _icmsContext.Specialtys
                              on drSpecial.specialty_id equals Spec.specialty_id

                              join drAddrConts in _icmsContext.PcpAddressContacts
                              on drAddr.provider_address_id equals drAddrConts.provider_address_id

                              join drConts in _icmsContext.PcpContacts
                              on drAddrConts.provider_contact_id equals drConts.provider_contact_id

                              join drContPhones in _icmsContext.PcpContactPhones
                              on drConts.provider_contact_id equals drContPhones.provider_contact_id

                              join drPhone in _icmsContext.PcpPhoneNumbers
                              on drContPhones.provider_phone_id equals drPhone.provider_phone_id

                              where drAddr.provider_address_id.Equals(providerAddressId)
                              select new Doctor
                              {
                                  providerContatctId = drContPhones.provider_contact_id,
                                  pcpId = dr.pcp_id,
                                  fullName = ((!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "") + " " + ((!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : ""),
                                  firstName = (!string.IsNullOrWhiteSpace(dr.provider_first_name)) ? dr.provider_first_name : "",
                                  lastName = (!string.IsNullOrWhiteSpace(dr.provider_last_name)) ? dr.provider_last_name : "",
                                  practiceName = (!string.IsNullOrWhiteSpace(dr.provider_group_name)) ? dr.provider_group_name : "",
                                  npi = (!string.IsNullOrEmpty(dr.npi)) ? dr.npi : (!string.IsNullOrWhiteSpace(dr.billing_npi)) ? dr.billing_npi : "",
                                  specialtyDesc = (!string.IsNullOrWhiteSpace(Spec.specialty_desc)) ? Spec.specialty_desc : "",
                                  emailAddress = (!string.IsNullOrWhiteSpace(dr.email_address)) ? dr.email_address : "",
                                  phoneNumber = (!string.IsNullOrWhiteSpace(drPhone.phone_number)) ? drPhone.phone_number : "",
                                  address1 = (!string.IsNullOrWhiteSpace(drAddr.address_line1)) ? drAddr.address_line1 : "",
                                  address2 = (!string.IsNullOrWhiteSpace(drAddr.address_line2)) ? drAddr.address_line2 : "",
                                  city = (!string.IsNullOrWhiteSpace(drAddr.city)) ? drAddr.city : "",
                                  stateAbbrev = (!string.IsNullOrWhiteSpace(drAddr.state_abbrev)) ? drAddr.state_abbrev : "",
                                  zip = (!string.IsNullOrWhiteSpace(drAddr.zip_code)) ? drAddr.zip_code : ""
                              })
                              .OrderByDescending(prov => prov.providerContatctId)
                              .Take(1)
                              .ToList();

            }

            return returnDoctor;
        }




        public List<HospitalFacility> getFacilitiesSearch(SearchParams srchParams)
        {

            List<HospitalFacility> facilities = null;

            if (!string.IsNullOrEmpty(srchParams.stateAbbrev))
            {

                facilities = (

                        from depts in _icmsContext.rDepartments

                        join deptAddr in _icmsContext.FacilityAddresses
                        on depts.id equals deptAddr.id into deptAddrs
                        from facAddress in deptAddrs.DefaultIfEmpty()

                        where depts.label.Contains(srchParams.facilityName)
                        && (depts.disable_flag.Value.Equals(false) || !depts.disable_flag.HasValue)
                        && facAddress.state_abbrev.Equals(srchParams.stateAbbrev)
                        && (depts.disable_flag.Equals(false) || !depts.disable_flag.HasValue)
                        orderby depts.label
                        select new HospitalFacility
                        {
                            hospitalId = depts.id,
                            hospitalName = depts.label,
                            address1 = facAddress.address_line_one,
                            address2 = facAddress.address_line_two,
                            city = facAddress.city,
                            stateAbbrev = facAddress.state_abbrev,
                            zip = facAddress.zip_code,
                            npi = depts.npi
                        }
                    )
                    .ToList();
            }
            else
            {

                facilities = (

                        from depts in _icmsContext.rDepartments

                        join deptAddr in _icmsContext.FacilityAddresses
                        on depts.id equals deptAddr.id into deptAddrs
                        from facAddress in deptAddrs.DefaultIfEmpty()

                        where depts.label.Contains(srchParams.facilityName)
                        && (depts.disable_flag.Value.Equals(false) || !depts.disable_flag.HasValue)
                        orderby depts.label
                        select new HospitalFacility
                        {
                            hospitalId = depts.id,
                            hospitalName = depts.label,
                            address1 = facAddress.address_line_one,
                            address2 = facAddress.address_line_two,
                            city = facAddress.city,
                            stateAbbrev = facAddress.state_abbrev,
                            zip = facAddress.zip_code,
                            npi = depts.npi
                        }
                    )
                    .ToList();
            }
                

            return facilities;
        }

        public HospitalFacility getFacility(string id)
        {

            HospitalFacility facility = null;

            int facilityId = 0;

            if (int.TryParse(id, out facilityId))
            {

                facility = (

                        from depts in _icmsContext.rDepartments

                        join deptAddr in _icmsContext.FacilityAddresses
                        on depts.id equals deptAddr.id into deptAddrs
                        from facAddress in deptAddrs.DefaultIfEmpty()

                        where depts.id.Equals(facilityId)
                        && (depts.disable_flag.Value.Equals(false) || !depts.disable_flag.HasValue)
                        orderby depts.label
                        select new HospitalFacility
                        {
                            hospitalId = depts.id,
                            hospitalName = depts.label,
                            address1 = facAddress.address_line_one,
                            address2 = facAddress.address_line_two,
                            city = facAddress.city,
                            stateAbbrev = facAddress.state_abbrev,
                            zip = facAddress.zip_code,
                            npi = depts.npi
                        }
                    )
                    .FirstOrDefault();
                    
            }

            return facility;
        }





        public List<Utilization> GetPatientReferrals(string id)
        {
            List<Utilization> refs = new List<Utilization>();

            Guid memberId = Guid.Empty;


            if (Guid.TryParse(id, out memberId))
            {
                List<Utilization> patRefs = (from memrefs in _icmsContext.MemberReferrals

                        join refTypes in _icmsContext.ReferralTypes
                        on memrefs.type_id equals refTypes.id into rftypes
                        from referralTypes in rftypes.DefaultIfEmpty()

                        where memrefs.member_id.Equals(memberId)
                        select new Utilization
                        {
                            memberId = memrefs.member_id,
                            referralNumber = memrefs.referral_number,
                            authNumber = memrefs.auth_number,
                            startDate = memrefs.auth_start_date,
                            endDate = memrefs.auth_end_date,
                            referralType = referralTypes.label
                        }
                       )
                       .OrderByDescending(auth => auth.startDate)
                       .ThenByDescending(auth => auth.endDate)
                       .ToList();


                foreach (Utilization auth in patRefs)
                {
                    Utilization addAuth = new Utilization();
                    addAuth.memberId = auth.memberId;
                    addAuth.referralNumber = auth.referralNumber;
                    addAuth.authNumber = auth.authNumber;
                    addAuth.startDate = (auth.startDate != null && !auth.startDate.Equals(DateTime.MinValue)) ? auth.startDate : DateTime.MinValue;
                    addAuth.displayStartDate = (auth.startDate != null && !auth.startDate.Equals(DateTime.MinValue)) ? auth.startDate.Value.ToShortDateString() : "";
                    addAuth.endDate = (auth.endDate != null && !auth.endDate.Equals(DateTime.MinValue)) ? auth.endDate : DateTime.MinValue;
                    addAuth.displayEndDate = (auth.endDate != null && !auth.endDate.Equals(DateTime.MinValue)) ? auth.endDate.Value.ToShortDateString() : "";
                    addAuth.referralType = auth.referralType;

                    refs.Add(addAuth);
                }

            }
            


            return refs;
        }

        public Utilization GetPatientReferral(string id, string refId)
        {
            Utilization referral = new Utilization();

            Guid memberId = Guid.Empty;


            if (Guid.TryParse(id, out memberId))
            {

                Utilization patRefs = (from memrefs in _icmsContext.MemberReferrals

                                       join refTypes in _icmsContext.ReferralTypes
                                       on memrefs.type_id equals refTypes.id into rftypes
                                       from referralTypes in rftypes.DefaultIfEmpty()

                                       join byProv in _icmsContext.Pcps
                                       on memrefs.referring_pcp_id equals byProv.pcp_id into byProvs
                                       from byProviders in byProvs.DefaultIfEmpty()

                                       join toProv in _icmsContext.Pcps
                                       on memrefs.referred_to_pcp_id equals toProv.pcp_id into toProvs
                                       from toProviders in toProvs.DefaultIfEmpty()

                                       join toFac in _icmsContext.rDepartments
                                       on memrefs.referred_to_department_id equals toFac.id into toFacs
                                       from toFacility in toFacs.DefaultIfEmpty()

                                       where memrefs.member_id.Equals(memberId)
                                       && memrefs.referral_number.Equals(refId)
                                       select new Utilization
                                       {
                                           memberId = memrefs.member_id,
                                           referralNumber = memrefs.referral_number,
                                           authNumber = (!string.IsNullOrEmpty(memrefs.auth_number)) ? memrefs.auth_number : "N/A",
                                           startDate = memrefs.auth_start_date,
                                           endDate = memrefs.auth_end_date,
                                           referralTypeId = memrefs.type_id,
                                           referralType = referralTypes.label,
                                           utilizationType = referralTypes.inpatient_outpatient_type,
                                           referralContextId = memrefs.context_id,
                                           referralCategoryId = memrefs.referral_category,
                                           referralReasonId = memrefs.reason_id,
                                           referredByPcpId = memrefs.referring_pcp_id,
                                           referredByPcpName = byProviders.provider_first_name + " " + byProviders.provider_last_name,
                                           referredByPcpNpi = (!string.IsNullOrEmpty(byProviders.npi)) ? byProviders.npi : byProviders.billing_npi,
                                           referredToPcpId = memrefs.referred_to_pcp_id,
                                           referredToPcpName = toProviders.provider_first_name + " " + toProviders.provider_last_name,
                                           referredToPcpNpi = (!string.IsNullOrEmpty(toProviders.npi)) ? toProviders.npi : toProviders.billing_npi,
                                           referredToFacilityId = memrefs.referred_to_department_id,
                                           referredToFacilityName = toFacility.label,
                                           referredToFacilityNpi = toFacility.npi
                                       }
                       )
                       .OrderByDescending(auth => auth.startDate)
                       .ThenByDescending(auth => auth.endDate)
                       .FirstOrDefault();


                if (patRefs != null)
                {

                    referral.memberId = patRefs.memberId;
                    referral.referralNumber = patRefs.referralNumber;
                    referral.authNumber = patRefs.authNumber;
                    referral.utilizationType = patRefs.utilizationType;
                    referral.startDate = (patRefs.startDate != null && !patRefs.startDate.Equals(DateTime.MinValue)) ? patRefs.startDate : DateTime.MinValue;
                    referral.displayStartDate = (patRefs.startDate != null && !patRefs.startDate.Equals(DateTime.MinValue)) ? patRefs.startDate.Value.ToShortDateString() : "";
                    referral.endDate = (patRefs.endDate != null && !patRefs.endDate.Equals(DateTime.MinValue)) ? patRefs.endDate : DateTime.MinValue;
                    referral.displayEndDate = (patRefs.endDate != null && !patRefs.endDate.Equals(DateTime.MinValue)) ? patRefs.endDate.Value.ToShortDateString() : "";
                    referral.referralTypeId = patRefs.referralTypeId;
                    referral.referralType = patRefs.referralType;
                    referral.referralContextId = patRefs.referralContextId;
                    referral.referralCategoryId = patRefs.referralCategoryId;
                    referral.referralReasonId = patRefs.referralReasonId;

                    //REFERRED BY PROVIDER
                    referral.referredByPcpId = patRefs.referredByPcpId;
                    referral.referredByPcpName = patRefs.referredByPcpName;
                    referral.referredByPcpNpi = patRefs.referredByPcpNpi;

                    //REFERRED TO PROVIDER
                    referral.referredToPcpId = patRefs.referredToPcpId;
                    referral.referredToPcpName = patRefs.referredToPcpName;
                    referral.referredToPcpNpi = patRefs.referredToPcpNpi;

                    //REFERRED TO FACILITY
                    referral.referredToFacilityId = patRefs.referredToFacilityId;
                    referral.referredToFacilityName = patRefs.referredToFacilityName;
                    referral.referredToFacilityNpi = patRefs.referredToFacilityNpi;

                    //CODES
                    referral.diagnosisCodes = GetReferralCodes("ICD10", refId, memberId);
                    referral.cptCodes = GetReferralCodes("CPT", refId, memberId);
                    referral.hcpcsCodes = GetReferralCodes("HCPCS", refId, memberId);

                    //ACTIONS
                    referral.actions = getReferralActions(refId, memberId);
                    //UTILIZATIONS
                    referral.utilizations = getReferralUtilizations(refId, memberId, patRefs.utilizationType);

                    if (referral.utilizations != null && referral.utilizations.Count > 0)
                    {
                        setReferralStartEndDates(ref referral);
                    }

                    //LETTERS
                    referral.letters = getReferralLetters(refId, memberId);
                    //NOTES
                    referral.notes = getReferralNotes(refId, memberId);
                    //SAVINGS
                    referral.savings = getReferralSavings(refId, memberId);
                    //FAXES
                    referral.faxes = getReferralFaxes(refId, memberId);
                    //REQUESTS
                    referral.requests = getReferralRequests(refId, memberId);
                    //CR BILLS
                    referral.crBills = getReferralCrBills(refId, memberId);
                }

            }



            return referral;
        }

        public void setReferralStartEndDates(ref Utilization referral)
        {

            DateTime refStart = DateTime.MinValue;
            DateTime refEnd = DateTime.MinValue;

            bool updateStart = (referral.startDate == null || referral.startDate == DateTime.MinValue) ? true : false;
            bool updateEnd = (referral.endDate == null || referral.endDate == DateTime.MinValue) ? true : false;

            List<UtilizationItem> orderedUtils = referral.utilizations.OrderBy(util => util.startDate)
                                                                      .ThenBy(util => util.endDate)
                                                                      .ToList();


            foreach (UtilizationItem util in orderedUtils)
            {

                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;

                if (util.referralType.Equals("O"))
                {

                    startDate = (util.visitsAuthorizedStartDate.HasValue) ? (DateTime)util.visitsAuthorizedStartDate.Value : DateTime.MinValue;
                    endDate = (util.visitsAuthorizedEndDate.HasValue) ? (DateTime)util.visitsAuthorizedEndDate.Value : DateTime.MinValue;
                } 
                else
                {

                }



                if (refStart.Equals(DateTime.MinValue) && startDate != DateTime.MinValue)
                {
                    refStart = startDate;
                }
                else if (startDate != null && startDate < refStart)
                {
                    refStart = startDate;
                }

                if (refEnd.Equals(DateTime.MinValue) && endDate != DateTime.MinValue)
                {
                    refEnd = endDate;
                }
                else if (endDate != null && endDate > refEnd)
                {
                    refEnd = endDate;
                }
            }

            if (refStart != DateTime.MinValue)
            {
                referral.startDate = refStart;
            }

            if (refEnd != DateTime.MinValue)
            {
                referral.endDate = refEnd;
            }

            if (updateStart || updateEnd &&
                !string.IsNullOrEmpty(referral.referralNumber))
            {
                updateReferralStartEndDates(referral.referralNumber, (DateTime)referral.startDate, (DateTime)referral.endDate);
            }
        }

        public List<MedicalCode> GetReferralCodes(string codeType, string referralNumber, Guid memberId)
        {

            List<MedicalCode> returnCodes = new List<MedicalCode>();

            switch (codeType)
            {
                case "ICD10":
                    returnCodes = GetReferralDiagnosisCodes(referralNumber, memberId);
                    break;

                case "CPT":
                    returnCodes = GetReferralCptCodes(referralNumber, memberId);
                    break;

                case "HCPCS":
                    returnCodes = GetReferralHcpcsCodes(referralNumber, memberId);
                    break;
            }


            return returnCodes;

        }

        public List<MedicalCode> GetReferralDiagnosisCodes(string referralNumber, Guid memberId)
        {
            
            List<MedicalCode> returnCodes = new List<MedicalCode>();

            returnCodes = (
                            from refdiags in _icmsContext.MemberReferralDiags

                            join diag10 in _icmsContext.DiagnosisCodes10
                            on refdiags.diagnosis_or_procedure_code equals diag10.diagnosis_code into icd10codes
                            from icd10 in icd10codes.DefaultIfEmpty()

                            where refdiags.referral_number.Equals(referralNumber)
                            && refdiags.member_id.Equals(memberId)

                            select new MedicalCode
                            {
                                CodeType = "ICD10",
                                CodeId = refdiags.id,
                                Code = refdiags.diagnosis_or_procedure_code,
                                DisplayDescription = icd10.medium_description,
                                LongDescription = icd10.long_description,
                                ShortDescription = icd10.short_description
                            }
                           )
                           .ToList();  
            
            foreach (MedicalCode code in returnCodes)
            {

                if (string.IsNullOrEmpty(code.DisplayDescription))
                {
                    string displayDescription = getIcd09Descriptions(code);
                    code.DisplayDescription = (!string.IsNullOrEmpty(displayDescription)) ? displayDescription : "N/A";
                }

            }

            return returnCodes;

        }

        public List<MedicalCode> GetReferralCptCodes(string referralNumber, Guid memberId)
        {

            List<MedicalCode> returnCodes = new List<MedicalCode>();

            returnCodes = (
                            from refcpts in _icmsContext.MemberReferralCpts

                            join cpt in _icmsContext.CptCodes
                            on refcpts.cpt_code equals cpt.cpt_code into cptcodes
                            from cpts in cptcodes.DefaultIfEmpty()

                            where refcpts.referral_number.Equals(referralNumber)
                            && refcpts.member_id.Equals(memberId)

                            select new MedicalCode
                            {
                                CodeType = "CPT",
                                CodeId = refcpts.id,
                                Code = refcpts.cpt_code,
                                DisplayDescription = (!string.IsNullOrEmpty(cpts.medium_descr)) ? cpts.medium_descr : cpts.cpt_descr,
                                LongDescription = cpts.cpt_descr,
                                ShortDescription = cpts.short_descr
                            }
                           )
                           .ToList();

            return returnCodes;

        }

        public List<MedicalCode> GetReferralHcpcsCodes(string referralNumber, Guid memberId)
        {

            List<MedicalCode> returnCodes = new List<MedicalCode>();

            returnCodes = (
                            from refhcpcs in _icmsContext.MemberReferralHcpcss

                            join hcp in _icmsContext.HcpcsCodes
                            on refhcpcs.hcpcs_code equals hcp.hcp_code into hcpcscodes
                            from hcpcs in hcpcscodes.DefaultIfEmpty()

                            where refhcpcs.referral_number.Equals(referralNumber)
                            && refhcpcs.member_id.Equals(memberId)

                            select new MedicalCode
                            {
                                CodeType = "HCPCS",
                                CodeId = refhcpcs.id,
                                Code = refhcpcs.hcpcs_code,
                                DisplayDescription = (!string.IsNullOrEmpty(hcpcs.hcpcs_short)) ? hcpcs.hcpcs_short: hcpcs.hcpcs_full,
                                LongDescription = hcpcs.hcpcs_full,
                                ShortDescription = hcpcs.hcpcs_short
                            }
                           )
                           .ToList();

            return returnCodes;

        }


        public rMemberReferral GetReferral(string refNumber, Guid memId)
        {
            
            rMemberReferral returnReferral = null;

            if (memId.Equals(Guid.Empty))
            {

                returnReferral = (
                                    from rmemref in _icmsContext.MemberReferrals
                                    where rmemref.referral_number.Equals(refNumber)
                                    select rmemref
                                 )
                                 .FirstOrDefault();
            } else
            {

                returnReferral = (
                                    from rmemref in _icmsContext.MemberReferrals
                                    where rmemref.referral_number.Equals(refNumber)
                                    && rmemref.member_id.Equals(memId)
                                    select rmemref
                                 )
                                 .FirstOrDefault();
            }

            return returnReferral;
        }

        public List<rMemberReferralDiags> GetReferralDiags(string refNumber, Guid memId)
        {

            List<rMemberReferralDiags> returnDiags = null;

            returnDiags = (
                            from memrefdiags in _icmsContext.MemberReferralDiags
                            where memrefdiags.referral_number.Equals(refNumber)
                            && memrefdiags.member_id.Equals(memId)
                            select memrefdiags
                          )
                          .ToList();

            return returnDiags;

        }

        public List<rMemberReferralCpts> GetReferralCpts(string refNumber, Guid memId)
        {

            List<rMemberReferralCpts> returnCpts = null;

            returnCpts = (
                            from memrefcpts in _icmsContext.MemberReferralCpts
                            where memrefcpts.referral_number.Equals(refNumber)
                            && memrefcpts.member_id.Equals(memId)
                            select memrefcpts
                          )
                          .ToList();

            return returnCpts;

        }

        public List<rMemberReferralHcpcs> GetReferralHcpcs(string refNumber, Guid memId)
        {

            List<rMemberReferralHcpcs> returnHcpcs = null;

            returnHcpcs = (
                            from memrefhcpcs in _icmsContext.MemberReferralHcpcss
                            where memrefhcpcs.referral_number.Equals(refNumber)
                            && memrefhcpcs.member_id.Equals(memId)
                            select memrefhcpcs
                          )
                          .ToList();

            return returnHcpcs;

        }


        public string getIcd09Descriptions(MedicalCode code)
        {
            string returnDescription = "";

            DiagnosisCodes diagOld = (
                                        from diags in _icmsContext.DiagnosisCodes
                                        where diags.Diagnosis_Code.Equals(code.Code)
                                        select diags
                                     )
                                     .FirstOrDefault();

            if (diagOld != null)
            {
                if (diagOld.Diagnosis_Descr != null)
                {
                    returnDescription = diagOld.Diagnosis_Descr;
                }
            }

            return returnDescription;
        }



        public List<UtilizationWorkflow> getReferralActions(string refNumber, Guid memId)
        {

            List<UtilizationWorkflow> actions = null;

            actions = (
                            from refwrkflws in _icmsContext.rMemberReferralWorkflows

                            join wrkflwxref in _icmsContext.rWorkflowXrefs
                            on refwrkflws.r_workflow_xref_id equals wrkflwxref.id 
                            //into xrefs
                            //from wrkflwrefs in xrefs.DefaultIfEmpty()

                            join pendreason in _icmsContext.ReferralActionReasons
                            on wrkflwxref.pendreason_id equals pendreason.id 
                            //into reasons
                            //from pendreasons in reasons.DefaultIfEmpty()

                            join currstat in _icmsContext.ReferralCurrentStatus
                            on wrkflwxref.currentstatus_id equals currstat.id 
                            //into stats
                            //from currstats in stats.DefaultIfEmpty()

                            where refwrkflws.referral_number.Equals(refNumber)
                            && refwrkflws.member_id.Equals(memId)

                            orderby refwrkflws.created_date descending

                            select new UtilizationWorkflow
                            {
                                workflowId = refwrkflws.workflow_id,
                                workflowDescription = wrkflwxref.evaluation_text,
                                reasonId = wrkflwxref.pendreason_id,
                                reasonDescription = pendreason.label,
                                statusId = wrkflwxref.currentstatus_id,
                                statusDescription = currstat.label,
                                assignToUserId = refwrkflws.assigned_to_user_id,
                                creationDate = refwrkflws.created_date,
                                displayCreationDate = (refwrkflws.created_date != null) ? 
                                    refwrkflws.created_date.ToShortDateString() + " " + refwrkflws.created_date.ToShortTimeString() : ""
                            }
                      )
                      .ToList();

            if (actions != null && actions.Count > 0)
            {
                
                StandardService standServ = new StandardService(_icmsContext, _aspNetContext);

                foreach(UtilizationWorkflow refAction in actions)
                {
                    if (!refAction.assignToUserId.Equals(Guid.Empty))
                    {

                        IcmsUser aspUsr = standServ.getAspUser((Guid)refAction.assignToUserId);

                        if (aspUsr != null)
                        {
                            refAction.assignToUserName = aspUsr.FullName;
                        } 
                        else
                        {
                            
                            IcmsUser icmsUsr = standServ.getIcmsUser((Guid)refAction.assignToUserId);

                            refAction.assignToUserName = icmsUsr.FullName;
                        }

                    }
                }
            }

            return actions;

        }

        public List<UtilizationItem> getReferralUtilizations(string refNumber, Guid memId, string utilizationType)
        {

            List<UtilizationItem> utilizations = null;

            utilizations = (

                    from utilDays in _icmsContext.rUtilizationDayses

                    join refDec in _icmsContext.rReferralDecisions
                    on utilDays.util_decision_id equals refDec.id into refDecs
                    from referralDecisions in refDecs.DefaultIfEmpty()

                    join beds in _icmsContext.rBedDayTypes
                    on utilDays.type_id equals beds.id into bedtyps
                    from bedTypes in bedtyps.DefaultIfEmpty()

                    join denials in _icmsContext.rDenialReasons
                    on utilDays.denial_reason_id equals denials.id into denialreas
                    from denialReasons in denialreas.DefaultIfEmpty()

                    join utilrev in _icmsContext.rUtilizationReviewses
                    on new { p1 = utilDays.referral_number, p2 = utilDays.member_id, p3 = utilDays.line_number }
                    equals new { p1 = (string)utilrev.referral_number, p2 = (Guid)utilrev.member_id, p3 = (int)utilrev.line_number } into utilrevs
                    from utilReview in utilrevs.DefaultIfEmpty()

                    join revItms in _icmsContext.ReviewTypeItemses
                    on utilReview.review_type_items_id equals revItms.review_type_items_id into reviewItems
                    from utilReviewBy in reviewItems.DefaultIfEmpty()

                    join perdreq in _icmsContext.rUtilizationVisitPeriods
                    on new { periodsrequested = utilDays.visits_period_requested } 
                    equals new { periodsrequested = perdreq.visit_period_abbrev } into periodreqs 
                    from periodRequests in periodreqs.DefaultIfEmpty()

                    join perdauth in _icmsContext.rUtilizationVisitPeriods
                    on new {periodsauthed = utilDays.visits_period_authorized }
                    equals new { periodsauthed = perdauth.visit_period_abbrev } into periodauths
                    from periodAuths in periodauths.DefaultIfEmpty()

                    where utilDays.referral_number.Equals(refNumber)
                    && utilDays.member_id.Equals(memId)
                    && (utilDays.removed.Equals(false) || utilDays.removed == null)
                    orderby utilDays.line_number descending
                    select new UtilizationItem
                    {
                        utilizationItemId = utilDays.PatCaseActID,
                        memberId = utilDays.member_id,
                        referralNumber = utilDays.referral_number,
                        referralType = utilDays.referral_type,
                        lineNumber = utilDays.line_number,
                        typeId = utilDays.type_id,
                        bedType = bedTypes.label,
                        surgeryFlag = utilDays.surgery_flag,  
                        surgeryOnFirstDayFlag = utilDays.surgery_on_first_day_flag,  
                        startDate = utilDays.start_date,  
                        endDate = utilDays.end_date,  
                        decisionId = utilDays.util_decision_id,  
                        decision = (referralDecisions.label != null) ? referralDecisions.label : "",
                        decisionBy = (utilReviewBy.name != null) ? utilReviewBy.name : "",
                        nextReviewDate = utilDays.next_review_date,  
                        numberOfDays = utilDays.number_of_days,  
                        visitsRecurring = utilDays.visits_recurring_flag,  
                        visitsNumPerPeriodRequested = utilDays.visits_num_periods_requested,  
                        visitsNumPerPeriodAuthorized = utilDays.visits_num_per_period_authorized,  
                        visitsRequested = (periodRequests.label != null) ? periodRequests.label : "",
                        visitsPeriodRequested = utilDays.visits_num_periods_requested,  
                        visitsPeriodAuthorized = utilDays.visits_num_periods_requested,  
                        visitsNumPeriodsRequested = utilDays.visits_num_periods_requested,  
                        visitsNumPeriodsAuthorized = utilDays.visits_num_periods_authorized,  
                        visitsAuthorized = (periodAuths.label != null) ? periodAuths.label : "",
                        visitsAuthorizedEndDate = utilDays.visits_authorized_end_date,  
                        visitsAuthorizedStartDate = utilDays.visits_authorized_start_date,
                        denialReasonId = utilDays.denial_reason_id,
                        denialReason = (denialReasons.label != null) ? denialReasons.label : "N/A",
                        dateUpdated = utilDays.DateUpdated
                    }
                )
                .ToList();

            return utilizations;
        }

        private string getReferralUtilizationType(string refNumber, Guid memId)
        {

            string utilizationType = "";

            utilizationType = (

                    from referral in _icmsContext.MemberReferrals

                    join refType in _icmsContext.ReferralTypes
                    on referral.type_id equals refType.id 

                    where referral.referral_number.Equals(refNumber)
                    && referral.member_id.Equals(memId)
                    select refType.inpatient_outpatient_type
                )
                .FirstOrDefault();

            return utilizationType;
        }

        private string getReferralTypeName(string refNumber, Guid memId)
        {

            string utilizationType = "";

            utilizationType = (

                    from referral in _icmsContext.MemberReferrals

                    join refType in _icmsContext.ReferralTypes
                    on referral.type_id equals refType.id

                    where referral.referral_number.Equals(refNumber)
                    && referral.member_id.Equals(memId)
                    select refType.inpatient_outpatient_type
                )
                .FirstOrDefault();

            return utilizationType;
        }

        public List<Letter> getReferralLetters(string refNumber, Guid memId)
        {

            List<Letter> letters = null;

            letters = (
                            from refLetters in _icmsContext.rMemberReferralLetterses

                            where refLetters.referral_number.Equals(refNumber)
                            && refLetters.member_id.Equals(memId)
                            && (refLetters.removed.Equals(false) || refLetters.removed == null)

                            orderby refLetters.letter_created descending

                            select new Letter
                            {
                                letterId = refLetters.id,
                                letterFileName = refLetters.file_identifier,
                                creationDate = (refLetters.letter_created.HasValue) ? refLetters.letter_created.Value : null,
                                displayCreationDate = (refLetters.letter_created.HasValue) ?
                                    refLetters.letter_created.Value.ToShortDateString() + " " + refLetters.letter_created.Value.ToShortTimeString() : ""
                            }
                      )
                      .ToList();

            return letters;

        }

        public List<Note> getReferralNotes(string refNumber, Guid memId)
        {

            List<Note> notes = null;

            List<Note> noteDates = (

                    from refNotes in _icmsContext.rUtilizationDaysNoteses
                    where refNotes.referral_number.Equals(refNumber)
                    && refNotes.member_id.Equals(memId)
                    select new Note
                    {
                        recordDate = refNotes.record_date
                    }
                )
                .Distinct()
                .OrderByDescending(d => d.recordDate)
                .ToList();

            if (noteDates != null && noteDates.Count > 0)
            {

                notes = new List<Note>();

                foreach (Note noteDate in noteDates)
                {

                    List<Note> dbNotes = (
                                    from refNotes in _icmsContext.rUtilizationDaysNoteses

                                    where refNotes.referral_number.Equals(refNumber)
                                    && refNotes.member_id.Equals(memId)
                                    && refNotes.record_date.Equals(noteDate.recordDate)

                                    orderby refNotes.record_date descending, refNotes.record_seq_num

                                    select new Note
                                    {
                                        noteId = refNotes.r_utilization_days_notes_id,
                                        recordDate = refNotes.record_date,
                                        noteSequenceNumber = refNotes.record_seq_num,
                                        noteText = refNotes.evaluation_text
                                    }
                                )
                                .ToList();

                    if (dbNotes != null && dbNotes.Count > 0)
                    {

                        Note addNote = new Note();

                        addNote.memberId = memId;
                        addNote.referralNumber = refNumber;
                        addNote.recordDate = noteDate.recordDate;
                        addNote.displayRecordDate = (noteDate.recordDate != null) ? 
                            noteDate.recordDate.ToShortDateString() + " " + noteDate.recordDate.ToShortTimeString() : 
                            "";

                        string noteToAdd = "";

                        foreach (Note note in dbNotes)
                        {
                            noteToAdd += note.noteText;
                        }

                        addNote.noteText = noteToAdd;

                        notes.Add(addNote);
                    }
                }

                if (notes.Count > 0)
                {
                    notes.OrderByDescending(nte => nte.recordDate);
                }
            }

            return notes;
        }

        public List<Note> getReferralSuspendNotes(string refNumber)
        {

            List<Note> suspendNotes = null;

            NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);

            suspendNotes = noteServ.getReferralSuspendedNotes(refNumber);

            return suspendNotes;
        }

        public List<Saving> getReferralSavings(string refNumber, Guid memId)
        {

            List<Saving> savings = null;

            savings = (

                    from refSav in _icmsContext.rUtilizationSavingses

                    join savUnits in _icmsContext.rSavingsUnits
                    on (int)refSav.saving_units_id equals savUnits.saving_units_id into savingUnit
                    from savingUnits in savingUnit.DefaultIfEmpty()

                    where refSav.referral_number.Equals(refNumber)
                    && refSav.member_id.Equals(memId)
                    orderby refSav.date_updated descending
                    select new Saving
                    {
                        savingsId = refSav.r_utilization_savings_id,
                        guidSavingsId = refSav.utilization_savings_id,
                        createdDate = refSav.date_updated,
                        displayCreatedDate = (refSav.date_updated.HasValue) ? 
                            refSav.date_updated.Value.ToShortDateString() + " " + refSav.date_updated.Value.ToShortTimeString() :
                            "",
                        itemDescription = refSav.item_description,
                        savingUnitsId = refSav.saving_units_id,
                        savingUnits = savingUnits.units_label,
                        quantity = refSav.quantity,
                        displayQuantity = refSav.quantity.Value.ToString("0.00"),
                        cost = refSav.cost,
                        displayCost = refSav.cost.Value.ToString("0.00"),
                        negotiated = refSav.negotiated,
                        displayNegotiated = refSav.negotiated.Value.ToString("0.00"),
                        savings = (refSav.cost > 0 && refSav.negotiated > 0) ? Convert.ToDecimal(refSav.cost - refSav.negotiated) : 0,
                        displaySavings = (refSav.cost > 0 && refSav.negotiated > 0) ? (refSav.cost - refSav.negotiated).Value.ToString("0.00") : ""
                    }
                )
                .ToList();
            

            return savings;
        }

        public List<Fax> getReferralFaxes(string refNumber, Guid memId)
        {

            List<Fax> faxes = null;

            faxes = (

                    from refFaxes in _icmsContext.rInboundFaxes

                    join refs in _icmsContext.MemberReferrals
                    on refFaxes.referral_number equals refs.referral_number 

                    where refFaxes.referral_number.Equals(refNumber)                       
                    && refFaxes.deleted_flag.Equals(false) 

                    orderby refFaxes.fax_creationtime descending
                    select new Fax
                    {
                        FaxId = refFaxes.id,
                        CreateDate = refFaxes.fax_creationtime,
                        displayCreatedDate = (refFaxes.fax_creationtime.HasValue) ? 
                                                refFaxes.fax_creationtime.Value.ToShortDateString() + " " + refFaxes.fax_creationtime.Value.ToShortTimeString() :
                                                "",                                
                        AssignedToUserId = refFaxes.assigned_to_user_id,
                        FaxQueueId = refFaxes.faxqueue_id,
                        emailFileName = refFaxes.email_filename
                    }
                )
                .ToList();


            return faxes;
        }

        public List<UtilizationRequest> getReferralRequests(string refNumber, Guid memId)
        {

            List<UtilizationRequest> requestsQuestionsDeterminations = null;
            List<UtilizationRequest> requests = null;
            List<UtilizationRequest> questions = null;
            List<UtilizationRequest> answers = null;
            List<UtilizationRequest> determiniations = null;

            questions = (

                    from quests in _icmsContext.MdReviewQuestions

                    join sysusr in _icmsContext.SystemUsers
                    on quests.assigned_to_system_user_id equals sysusr.system_user_id

                    where quests.referral_number.Equals(refNumber)
                    && !string.IsNullOrEmpty(quests.md_question_note)

                    orderby quests.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Question",
                        reviewQuestionId = quests.md_review_question_id,
                        requestNote = quests.md_question_note,
                        taskId = quests.task_id,
                        assignedToUserId = quests.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = quests.creation_date,
                        displayCreateDate = (quests.creation_date.HasValue) ? 
                            quests.creation_date.Value.ToShortDateString() + " " + quests.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            answers = (

                                from answrs in _icmsContext.MdReviewQuestions

                                join sysusr in _icmsContext.SystemUsers
                                on answrs.assigned_to_system_user_id equals sysusr.system_user_id

                                where answrs.referral_number.Equals(refNumber)
                                && !string.IsNullOrEmpty(answrs.md_answer_note)

                                orderby answrs.creation_date descending
                                select new UtilizationRequest
                                {
                                    requestType = "Answer",
                                    reviewQuestionId = answrs.md_review_question_id,
                                    requestNote = answrs.md_answer_note,
                                    taskId = answrs.task_id,
                                    assignedToUserId = answrs.assigned_to_system_user_id,
                                    assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                                    createDate = answrs.creation_date,
                                    displayCreateDate = (answrs.creation_date.HasValue) ?
                                        answrs.creation_date.Value.ToShortDateString() + " " + answrs.creation_date.Value.ToShortTimeString() :
                                        ""
                                }
                            )
                            .ToList();


            requests = (

                    from requts in _icmsContext.MdReviewRequests

                    join sysusr in _icmsContext.SystemUsers
                    on requts.assigned_to_system_user_id equals sysusr.system_user_id

                    where requts.referral_number.Equals(refNumber)

                    orderby requts.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Request",
                        reviewRequestId = requts.md_review_request_id,
                        requestNote = requts.md_review_appeal_note,
                        taskId = requts.task_id,
                        assignedToUserId = requts.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = requts.creation_date,
                        displayCreateDate = (requts.creation_date.HasValue) ?
                            requts.creation_date.Value.ToShortDateString() + " " + requts.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            determiniations = (

                    from requts in _icmsContext.MdReviewDeterminations

                    join sysusr in _icmsContext.SystemUsers
                    on requts.assigned_to_system_user_id equals sysusr.system_user_id

                    where requts.referral_number.Equals(refNumber)

                    orderby requts.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Determination",
                        reviewRequestId = requts.md_review_determination_id,
                        requestNote = requts.md_review_determination_note,
                        taskId = requts.task_id,
                        assignedToUserId = requts.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = requts.creation_date,
                        displayCreateDate = (requts.creation_date.HasValue) ?
                            requts.creation_date.Value.ToShortDateString() + " " + requts.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            if (answers != null || questions != null || requests != null || determiniations != null)
            {

                List<UtilizationRequest> reqQuestDeterm = new List<UtilizationRequest>();

                if (answers != null)
                {
                    foreach (UtilizationRequest answr in answers)
                    {
                        reqQuestDeterm.Add(answr);
                    }
                }

                if (questions != null)
                {
                    foreach(UtilizationRequest quest in questions)
                    {
                        reqQuestDeterm.Add(quest);
                    }
                }

                if (requests != null)
                {
                    foreach (UtilizationRequest req in requests)
                    {
                        reqQuestDeterm.Add(req);
                    }
                }

                if (determiniations != null)
                {
                    foreach (UtilizationRequest deter in determiniations)
                    {
                        reqQuestDeterm.Add(deter);
                    }
                }

                requestsQuestionsDeterminations = reqQuestDeterm.OrderByDescending(req => req.createDate).ToList();
            }


            return requestsQuestionsDeterminations;
        }

        public List<UtilizationRequest> getMedicalReviewRequests(int medicalReviewId, string refNumber)
        {

            List<UtilizationRequest> requestsQuestionsDeterminations = null;
            List<UtilizationRequest> requests = null;
            List<UtilizationRequest> questions = null;
            List<UtilizationRequest> answers = null;
            List<UtilizationRequest> determiniations = null;

            questions = (

                    from quests in _icmsContext.MdReviewQuestions

                    join sysusr in _icmsContext.SystemUsers
                    on quests.assigned_to_system_user_id equals sysusr.system_user_id

                    where quests.md_review_request_id.Equals(medicalReviewId)
                    && quests.md_question_note != null

                    orderby quests.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Question",
                        reviewRequestId = quests.md_review_question_id,
                        requestNote = quests.md_question_note,
                        taskId = quests.task_id,
                        assignedToUserId = quests.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = quests.creation_date,
                        displayCreateDate = (quests.creation_date.HasValue) ?
                            quests.creation_date.Value.ToShortDateString() + " " + quests.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            answers = (

                    from answr in _icmsContext.MdReviewQuestions

                    join sysusr in _icmsContext.SystemUsers
                    on answr.assigned_to_system_user_id equals sysusr.system_user_id

                    where answr.md_review_request_id.Equals(medicalReviewId)
                    && answr.md_answer_note != null

                    orderby answr.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Answer",
                        reviewRequestId = answr.md_review_question_id,
                        requestNote = answr.md_answer_note,
                        taskId = answr.task_id,
                        assignedToUserId = answr.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = answr.creation_date,
                        displayCreateDate = (answr.creation_date.HasValue) ?
                            answr.creation_date.Value.ToShortDateString() + " " + answr.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();


            requests = (

                    from requts in _icmsContext.MdReviewRequests

                    join sysusr in _icmsContext.SystemUsers
                    on requts.assigned_to_system_user_id equals sysusr.system_user_id

                    where requts.md_review_request_id.Equals(medicalReviewId)
                    && requts.md_review_appeal_note != null

                    orderby requts.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Request",
                        reviewRequestId = requts.md_review_request_id,
                        requestNote = requts.md_review_appeal_note,
                        taskId = requts.task_id,
                        assignedToUserId = requts.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = requts.creation_date,
                        displayCreateDate = (requts.creation_date.HasValue) ?
                            requts.creation_date.Value.ToShortDateString() + " " + requts.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            determiniations = (

                    from determ in _icmsContext.MdReviewDeterminations

                    join sysusr in _icmsContext.SystemUsers
                    on determ.assigned_to_system_user_id equals sysusr.system_user_id

                    where determ.md_review_request_id.Equals(medicalReviewId)
                    && determ.md_review_determination_note != null

                    orderby determ.creation_date descending
                    select new UtilizationRequest
                    {
                        requestType = "Determination",
                        reviewRequestId = determ.md_review_determination_id,
                        requestNote = determ.md_review_determination_note,
                        taskId = determ.task_id,
                        assignedToUserId = determ.assigned_to_system_user_id,
                        assignedToUser = sysusr.system_user_first_name + " " + sysusr.system_user_last_name,
                        createDate = determ.creation_date,
                        displayCreateDate = (determ.creation_date.HasValue) ?
                            determ.creation_date.Value.ToShortDateString() + " " + determ.creation_date.Value.ToShortTimeString() :
                            ""
                    }
                )
                .ToList();

            if (questions != null || answers != null || requests != null || determiniations != null)
            {

                List<UtilizationRequest> reqQuestDeterm = new List<UtilizationRequest>();

                if (questions != null)
                {
                    foreach (UtilizationRequest quest in questions)
                    {
                        reqQuestDeterm.Add(quest);
                    }
                }

                if (answers != null)
                {
                    foreach (UtilizationRequest answr in answers)
                    {
                        reqQuestDeterm.Add(answr);
                    }
                }

                if (requests != null)
                {
                    foreach (UtilizationRequest req in requests)
                    {
                        reqQuestDeterm.Add(req);
                    }
                }

                if (determiniations != null)
                {
                    foreach (UtilizationRequest deter in determiniations)
                    {
                        reqQuestDeterm.Add(deter);
                    }
                }

                requestsQuestionsDeterminations = reqQuestDeterm.OrderByDescending(req => req.createDate)
                                                                .ThenBy(req => req.requestType)
                                                                .ToList();
            }


            return requestsQuestionsDeterminations;
        }

        private string getReferralAuthNumber(string referralNumber)
        {

            string authNumber = "";

            authNumber = (

                    from patref in _icmsContext.MemberReferrals
                    where patref.referral_number.Equals(referralNumber)
                    select patref.auth_number
                )
                .FirstOrDefault();

            return authNumber;
        }





        public Utilization addUtilizationsCrBill(Utilization util)
        {
            Utilization referral = null;

            try
            {

                ClinicalReviewBills addCrBill = new ClinicalReviewBills();

                addCrBill.type_of_review = "";
                addCrBill.other_type_of_review = "";
                addCrBill.description = "";
                addCrBill.is_physician_review = 0;
                addCrBill.is_nurse_review = 0;
                addCrBill.is_other_review = 0;
                addCrBill.review_cost = 0;
                addCrBill.other_review_cost = 0;

                addCrBill.member_id = util.memberId;
                addCrBill.referral_number = util.referralNumber;
                addCrBill.creation_date = util.startDate;
                addCrBill.user_id = util.userId;                

                switch (util.crBills[0].crBillType)
                {
                    case "tpa":
                        addCrBill.type_of_review = "TPA PEND";
                        break;
                    case "appeal":
                        addCrBill.type_of_review = "APPEAL";
                        break;
                }

                if (string.IsNullOrEmpty(addCrBill.type_of_review))
                {
                    addCrBill.other_type_of_review = util.crBills[0].crBillType;
                }

                if (!string.IsNullOrEmpty(util.crBills[0].crBillDescription))
                {
                    addCrBill.description = getCrBillDescription(Convert.ToInt32(util.crBills[0].crBillDescription));
                }

                if (util.crBills[0].crBillPhysicianReview)
                {
                    addCrBill.is_physician_review = 1;
                    addCrBill.review_cost = 250;                    
                }
                else if (util.crBills[0].crBillNurseReview)
                {
                    addCrBill.is_nurse_review = 1;
                    addCrBill.review_cost = 80;
                }
                else
                {
                    addCrBill.is_other_review = 1;
                    addCrBill.other_review_cost = (util.crBills[0].crBillCost > 0) ? util.crBills[0].crBillCost : 80;
                }

                _icmsContext.ClinicalReviewBillses.Add(addCrBill);

                if (_icmsContext.SaveChanges() > 0)
                {

                    List<Bill> crBills = getReferralCrBills(util.referralNumber, (Guid)util.memberId);

                    if (crBills != null)
                    {

                        referral = new Utilization();
                        referral.crBills = new List<Bill>();
                        referral.crBills = crBills;
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return referral;
        }

        public List<Bill> getReferralCrBills(string refNumber, Guid memId)
        {

            List<Bill> crBills = null;

            crBills = (
                    from clinicBills in _icmsContext.ClinicalReviewBillses
                    where clinicBills.referral_number.Equals(refNumber)
                    && clinicBills.member_id.Equals(memId)
                    orderby clinicBills.creation_date descending
                    select new Bill
                    {
                        crBillId = clinicBills.cr_bill_id,
                        memberId = clinicBills.member_id,
                        referralNumber = clinicBills.referral_number,
                        recordDate = clinicBills.creation_date,
                        displayRecordDate = (clinicBills.creation_date.HasValue) ?
                            clinicBills.creation_date.Value.ToShortDateString() + " " + clinicBills.creation_date.Value.ToShortTimeString() :
                            "",
                        crBillDescription = clinicBills.description,
                        crBillType = clinicBills.type_of_review,
                        crBillCost = (clinicBills.review_cost.HasValue) ? (decimal)clinicBills.review_cost.Value : 
                            (clinicBills.other_review_cost.HasValue) ? (decimal)clinicBills.other_review_cost : 0,
                    }
                )
                .ToList();

            return crBills;
        }



        public UtilizationItem getReferralUtilizationItem(string memId, string refId, string utilid)
        {
            UtilizationItem utilItem = null;

            int patCaseActId = 0;

            if (int.TryParse(utilid, out patCaseActId))
            {

                utilItem = (
                        from utilDay in _icmsContext.rUtilizationDayses

                        join utilrev in _icmsContext.rUtilizationReviewses
                        on new { p1 = utilDay.referral_number, p2 = utilDay.member_id, p3 = utilDay.line_number }
                        equals new { p1 = (string)utilrev.referral_number, p2 = (Guid)utilrev.member_id, p3 = (int)utilrev.line_number } into utilrevs
                        from utilReview in utilrevs.DefaultIfEmpty()

                        where utilDay.PatCaseActID.Equals(patCaseActId)
                        && (utilDay.removed.Equals(false) || utilDay.removed == null)

                        select new UtilizationItem
                        {
                            utilizationItemId = utilDay.PatCaseActID,
                            memberId = utilDay.member_id,
                            referralNumber = utilDay.referral_number,
                            referralType = utilDay.referral_type,
                            lineNumber = utilDay.line_number,
                            typeId = utilDay.type_id,
                            surgeryFlag = utilDay.surgery_flag,
                            surgeryOnFirstDayFlag = utilDay.surgery_on_first_day_flag,
                            startDate = utilDay.start_date,
                            endDate = utilDay.end_date,
                            decisionId = utilDay.util_decision_id,
                            decisionById = utilReview.review_type_items_id,
                            nextReviewDate = utilDay.next_review_date,
                            numberOfDays = utilDay.number_of_days,
                            visitsRecurring = utilDay.visits_recurring_flag,
                            visitsNumPerPeriodRequested = utilDay.visits_num_periods_requested,
                            visitsNumPerPeriodAuthorized = utilDay.visits_num_per_period_authorized,
                            visitsPeriodRequested = utilDay.visits_num_periods_requested,
                            visitsRequested = (utilDay.visits_period_requested != null) ? utilDay.visits_period_requested : "",
                            visitsPeriodAuthorized = utilDay.visits_num_periods_requested,
                            visitsNumPeriodsRequested = utilDay.visits_num_periods_requested,
                            visitsNumPeriodsAuthorized = utilDay.visits_num_periods_authorized,
                            visitsAuthorized = (utilDay.visits_period_authorized != null) ? utilDay.visits_period_authorized : "",
                            visitsAuthorizedEndDate = utilDay.visits_authorized_end_date,
                            visitsAuthorizedStartDate = utilDay.visits_authorized_start_date,
                            denialReasonId = utilDay.denial_reason_id,
                            dateUpdated = utilDay.DateUpdated
                        }
                    )
                    .FirstOrDefault();
            }

            return utilItem;
        }

        public bool priorUtilizationIsPendedReadyForReview(Utilization util)
        {

            if (!string.IsNullOrEmpty(util.referralNumber) && !string.IsNullOrEmpty(util.utilizationType) &&
                util.utilizations[0].decisionById > 0)
            {

                int priorUtilDecisionId = (

                        from utils in _icmsContext.rUtilizationDayses
                        where utils.referral_number.Equals(util.referralNumber)
                        && utils.referral_type.Equals(util.utilizationType)
                        && (utils.util_decision_id.HasValue && utils.util_decision_id.Value > 0)
                        && (!utils.removed.HasValue || utils.removed.Equals(0))
                        orderby utils.PatCaseActID descending
                        select (utils.util_decision_id.HasValue) ? utils.util_decision_id.Value : 0
                    )
                    .Take(1)
                    .FirstOrDefault();

                if (priorUtilDecisionId.Equals(4)) //previous utilization is pended
                {

                    bool isMdReview = utilizationIsDecisionByMdOrThirdParty(util.utilizations[0].decisionById);

                    if (isMdReview)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool utilizationIsDecisionByMdOrThirdParty(int? decisionById)
        {

            if (decisionById > 0)
            {

                ReviewTypeItems reviewItem = (

                        from revItm in _icmsContext.ReviewTypeItemses
                        where revItm.review_type_items_id.Equals(decisionById)
                        && (revItm.is_dr > 0 || revItm.is_third_party > 0)
                        select revItm
                    )
                    .FirstOrDefault();

                if (reviewItem != null)
                {
                    return true;
                }
            }

            return false;

        }

        public UtilizationWorkflow getReferralActionItem(string memId, string refId, string actid)
        {
            UtilizationWorkflow action = null;

            Guid workflowId = Guid.Empty;

            if (Guid.TryParse(actid, out workflowId))
            {

                action = (
                            from refwrkflws in _icmsContext.rMemberReferralWorkflows

                            join wrkflwxref in _icmsContext.rWorkflowXrefs
                            on refwrkflws.r_workflow_xref_id equals wrkflwxref.id

                            join pendreason in _icmsContext.ReferralActionReasons
                            on wrkflwxref.pendreason_id equals pendreason.id

                            join currstat in _icmsContext.ReferralCurrentStatus
                            on wrkflwxref.currentstatus_id equals currstat.id

                            where refwrkflws.workflow_id.Equals(workflowId)

                            orderby refwrkflws.created_date descending

                            select new UtilizationWorkflow
                            {
                                workflowId = refwrkflws.workflow_id,
                                workflowDescription = wrkflwxref.evaluation_text,
                                reasonId = wrkflwxref.pendreason_id,
                                reasonDescription = pendreason.label,
                                statusId = wrkflwxref.currentstatus_id,
                                statusDescription = currstat.label,
                                assignToUserId = refwrkflws.assigned_to_user_id,
                                creationDate = refwrkflws.created_date,
                                displayCreationDate = (refwrkflws.created_date != null) ?
                                    refwrkflws.created_date.ToShortDateString() + " " + refwrkflws.created_date.ToShortTimeString() : ""
                            }
                      )
                      .FirstOrDefault();
                      
            }

            return action;
        }

        public Letter getReferralLetterItem(string letterId)
        {

            Letter letter = null;

            int letrId = 0;

            if (int.TryParse(letterId, out letrId))
            {

                letter = (
                            from refLetters in _icmsContext.rMemberReferralLetterses
                            where refLetters.id.Equals(letrId)
                            select new Letter
                            {
                                letterId = refLetters.id,
                                memberId = (Guid)refLetters.member_id,
                                referralNumber = refLetters.referral_number,
                                letterFileName = refLetters.file_identifier,
                                creationDate = (refLetters.letter_created.HasValue) ? refLetters.letter_created.Value : null,
                                displayCreationDate = (refLetters.letter_created.HasValue) ?
                                    refLetters.letter_created.Value.ToShortDateString() + " " + refLetters.letter_created.Value.ToShortTimeString() : "",
                                letterContentType = "application/pdf",
                                letterPdf = refLetters.file_blob,
                                letterBase64 = Convert.ToBase64String(refLetters.file_blob)
                            }
                        )
                        .FirstOrDefault();
            }

            return letter;                                           
        }

        public Fax getReferralFaxItem(string faxId)
        {

            Fax fax = null;

            int fxId = 0;

            if (int.TryParse(faxId, out fxId))
            {

                fax = (
                            from refFaxes in _icmsContext.rInboundFaxes
                            where refFaxes.id.Equals(fxId)
                            select new Fax
                            {
                                FaxId = refFaxes.id,
                                CreateDate = refFaxes.fax_creationtime,
                                displayCreatedDate = (refFaxes.fax_creationtime.HasValue) ?
                                                refFaxes.fax_creationtime.Value.ToShortDateString() + " " + refFaxes.fax_creationtime.Value.ToShortTimeString() :
                                                "",
                                AssignedToUserId = refFaxes.assigned_to_user_id,
                                FaxImage = refFaxes.fax_image,
                                FaxQueueId = refFaxes.faxqueue_id,
                                emailFileName = refFaxes.email_filename,
                                FaxImageBase64 = Convert.ToBase64String(refFaxes.fax_image),
                                faxContentType = "application/pdf"
                            }
                        )
                        .FirstOrDefault();
            }

            return fax;
        }

        public List<Note> getReferralUtilizationSuspendNotes(string refId)
        {

            List<Note> suspendNotes = null;

            NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);

            suspendNotes = noteServ.getReferralSuspendedNotes(refId);

            return suspendNotes;
        }

        public Note getReferralUtilizationSuspendNote(string noteId)
        {

            Note suspendNotes = null;

            int suspendNoteId = 0;

            if (int.TryParse(noteId, out suspendNoteId))
            {

                NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);

                suspendNotes = noteServ.getReferralSuspendedNote(suspendNoteId);
            }

            return suspendNotes;
        }



        public ClaimDataMine getClaimDataReportAssets()
        {
            ClaimDataMine reportAssets = null;

            StandardService standServ = new StandardService(_icmsContext, _aspNetContext);
            IEnumerable<Tpas> systemTpas = standServ.GetTpas();

            if (systemTpas != null)
            {

                reportAssets = new ClaimDataMine();
                reportAssets.tpas = new List<Tpa>();

                foreach (Tpas tpa in systemTpas)
                {
                    Tpa claimTpa = new Tpa();
                    claimTpa.tpaId = tpa.tpa_id;
                    claimTpa.tpaName = tpa.tpa_name;

                    reportAssets.tpas.Add(claimTpa);
                }                
            }

            return reportAssets;
        }

        public int loadDataMinedClaims(ClaimDataMine reportParams)
        {
            int rptId = 0;

            List<ClaimDataMine> claims = null;

            removeUserDataMinedClaimReportItems(reportParams);

            ClaimDataService clmDataServ = new ClaimDataService(_icmsContext, _aspNetContext, _dataStagingContext);
            claims = clmDataServ.getDataMinedClaims(reportParams);

            if (claims != null && claims.Count > 0)
            {
                rptId = addClaimsToReport(reportParams, claims);
            }

            return rptId;
        }

        private void removeUserDataMinedClaimReportItems(ClaimDataMine reportParams)
        {
            List<RptClaimOutreachSearch> removeClaims = null;

            removeClaims = (
                                from rptClaims in _icmsContext.RptClaimOutreachSearchs
                                where rptClaims.report_user_id.Equals(reportParams.usr)
                                select rptClaims
                           )
                           .ToList();

            _icmsContext.RptClaimOutreachSearchs.RemoveRange(removeClaims);
            _icmsContext.SaveChanges();

        }

        private int addClaimsToReport(ClaimDataMine reportParams, List<ClaimDataMine> claims)
        {

            int rptId = getNextRptId();

            if (rptId > 0)
            {

                foreach (ClaimDataMine clm in claims)
                {
                    RptClaimOutreachSearch reportClaim = new RptClaimOutreachSearch();
                    reportClaim.report_id = rptId;
                    reportClaim.report_user_id = reportParams.usr;

                    reportClaim.member_id = clm.patientId;
                    reportClaim.member_full_name = clm.claimantFirstName + " " + clm.claimantLastName;
                    reportClaim.member_last_name = clm.claimantLastName;
                    reportClaim.member_first_name = clm.claimantFirstName;
                    reportClaim.member_birth = clm.patientBirth;
                    reportClaim.member_ssn = clm.patientSsn;
                    reportClaim.employer_name = clm.employerName;
                    reportClaim.tpa_name = clm.tpaName;
                    reportClaim.tpa_id = clm.tpaId;
                    reportClaim.member_in_lcm = ((bool)clm.inLcm) ? "Yes" : "No";
                    reportClaim.member_in_dm = ((bool)clm.inDm) ? "Yes" : "No";
                    reportClaim.diag_1 = clm.diag1;
                    reportClaim.diag_1_description = clm.diag1Desc;
                    reportClaim.diag_2 = clm.diag2;
                    reportClaim.diag_2_description = clm.diag2Desc;
                    reportClaim.diag_3 = clm.diag3;
                    reportClaim.diag_3_description = clm.diag3Desc;
                    reportClaim.diag_4 = clm.diag4;
                    reportClaim.diag_4_description = clm.diag4Desc;
                    reportClaim.diag_5 = clm.diag5;
                    reportClaim.cpt_code = clm.cpt;
                    reportClaim.hcpcs_code = clm.hcpcs;
                    reportClaim.service_date = clm.serviceDate;
                    reportClaim.provider_tin = clm.providerTin;
                    reportClaim.provider_name = clm.providerName;
                    reportClaim.pos_name = clm.posName;
                    reportClaim.claim_paid_amount = Convert.ToInt32(clm.paidAmount);

                    _icmsContext.RptClaimOutreachSearchs.Add(reportClaim);
                }

                if (_icmsContext.ChangeTracker.HasChanges())
                {
                    int results = _icmsContext.SaveChanges();

                    if (results > 0)
                    {
                        return rptId;
                    }
                }
            }

            return 0;
        }

        private int getNextRptId()
        {
            int reportId = 0;

            RptNextUniqueId rptId = (
                                        from nxtrptid in _icmsContext.RptNextUniqueIds
                                        select nxtrptid
                                    )
                                    .FirstOrDefault();

            int tmpRptId = (int)rptId.nxt_uniqueID;

            if (tmpRptId > 0)
            {
                rptId.nxt_uniqueID = tmpRptId + 1;

                _icmsContext.RptNextUniqueIds.Update(rptId);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {
                    reportId = tmpRptId;
                }
            }


            return reportId;
        }


        public Utilization createUmReferralNumber(Utilization referral)
        {

            Utilization returnReferral = null;


            string newReferralNumber = getNewReferralNumber();

            if (!string.IsNullOrEmpty(newReferralNumber))
            {

                rMemberReferral newReferral = new rMemberReferral();
                newReferral.referral_number = newReferralNumber;
                newReferral.member_id = (Guid)referral.memberId;
                newReferral.type_id = 2; //default to Outpatient (from r_REFERRALTYPE table)
                newReferral.created_date = referral.creationDate;
                newReferral.created_user_id = referral.userId;

                _icmsContext.MemberReferrals.Add(newReferral);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    returnReferral = new Utilization();

                    returnReferral.memberId = newReferral.member_id;
                    returnReferral.referralNumber = newReferral.referral_number;
                }
            }

            return returnReferral;
        }

        private string getNewReferralNumber()
        {

            string referralNumber = "";

            string connectString = _icmsContext.Database.GetDbConnection().ConnectionString;

            if (!string.IsNullOrEmpty(connectString))
            {

                SqlConnection sqlconnection = new SqlConnection(connectString);

                SqlCommand sqlcmd = new SqlCommand("GenRefNum", sqlconnection);
                sqlcmd.CommandText = "GenRefNum";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlconnection;

                Guid guid = new Guid("8C4AA16B-75FE-11D3-A7EE-00500499C350");
                sqlcmd.Parameters.Add("@callerid", SqlDbType.UniqueIdentifier).Value = guid;

                sqlconnection.Open();

                string tempReferralNumber = Convert.ToString(sqlcmd.ExecuteScalar());

                sqlcmd.Connection.Close();

                if (!string.IsNullOrEmpty(tempReferralNumber))
                {
                    referralNumber = tempReferralNumber;
                }
            }

            return referralNumber;
        }


        public Utilization updateUmAuthNumber(string refNum)
        {
            
            Utilization referral = null;

            rMemberReferral memref = GetReferral(refNum, Guid.Empty);

            if (memref != null)
            {
                
                string newAuthNumber = getNewAuthNumber(refNum);

                if (!string.IsNullOrEmpty(newAuthNumber))
                {

                    memref.auth_number = newAuthNumber;

                    _icmsContext.MemberReferrals.Update(memref);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {

                        referral = new Utilization();

                        referral.memberId = memref.member_id;
                        referral.referralNumber = memref.referral_number;
                        referral.authNumber = newAuthNumber;
                    }
                }
            }

            return referral;
        }

        private string getNewAuthNumber(string referralNumber)
        {
            
            string authNumber = "";

            string connectString = _icmsContext.Database.GetDbConnection().ConnectionString;

            if (!string.IsNullOrEmpty(connectString))
            {

                SqlConnection sqlconnection = new SqlConnection(connectString);

                SqlCommand sqlcmd = new SqlCommand("GenAuthNum", sqlconnection);
                sqlcmd.CommandText = "GenAuthNum";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlconnection;

                Guid guid = new Guid("8C4AA16B-75FE-11D3-A7EE-00500499C350");
                sqlcmd.Parameters.Add("@callerid", SqlDbType.UniqueIdentifier).Value = guid;

                sqlconnection.Open();

                string tempAuthNumber = Convert.ToString(sqlcmd.ExecuteScalar());

                sqlcmd.Connection.Close();

                if (!string.IsNullOrEmpty(tempAuthNumber))
                {
                    authNumber = tempAuthNumber;
                }
            }

            return authNumber;
        }


        public Utilization updateGeneralUm(Utilization util)
        {
            Utilization referral = null;

            rMemberReferral memref = GetReferral(util.referralNumber, (Guid)util.memberId);

            if (memref != null)
            {

                memref.type_id = util.referralTypeId;
                memref.context_id = util.referralContextId;
                memref.referral_category = util.referralCategoryId;
                memref.reason_id = util.referralReasonId;

                _icmsContext.MemberReferrals.Update(memref);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    referral = GetPatientReferral(memref.member_id.ToString(), memref.referral_number);
                }
            }

            return referral;
        }

        private void updateReferralStartEndDates(string referralNumber, DateTime startDate, DateTime endDate)
        {

            rMemberReferral referral = (

                    from memref in _icmsContext.MemberReferrals
                    where memref.referral_number.Equals(referralNumber)
                    select memref
            ).FirstOrDefault();

            if (referral != null)
            {

                bool updateStart = false;

                if (!startDate.Equals(DateTime.MinValue))
                {
                    referral.auth_start_date = startDate;
                    updateStart = true;
                }

                bool updateEnd = false;

                if (!endDate.Equals(DateTime.MinValue))
                {
                    referral.auth_end_date = endDate;
                    updateEnd = true;
                }

                if (updateStart || updateEnd)
                {
                    _icmsContext.MemberReferrals.Update(referral);
                    _icmsContext.SaveChanges();
                }
            }
        }


        public Utilization updateProviderUm(Utilization util)
        {
            Utilization referral = new Utilization();

            rMemberReferral memref = GetReferral(util.referralNumber, (Guid)util.memberId);


            if (memref != null)
            {
                if (util.referredByPcpId != null && !util.referredByPcpId.Equals(Guid.Empty))
                {                    
                    memref.referring_pcp_id = util.referredByPcpId;
                } else if (util.referredByPcpRemove)
                {
                    memref.referring_pcp_id = null;
                }

                if (util.referredToPcpId != null && !util.referredToPcpId.Equals(Guid.Empty))
                {                                        
                    memref.referred_to_pcp_id = util.referredToPcpId;
                } else if (util.referredToPcpRemove)
                {
                    memref.referred_to_pcp_id = null;
                }

                _icmsContext.MemberReferrals.Update(memref);
                int result = _icmsContext.SaveChanges();


                if (result > 0)
                {
                    referral.memberId = memref.member_id;
                    referral.referralNumber = memref.referral_number;
                    referral.referredByPcpId = memref.referring_pcp_id;
                    referral.referredToPcpId = memref.referred_to_pcp_id;
                }
            }

            return referral;
        }


        public Utilization updateFacilityUm(Utilization util)
        {
            Utilization referral = null;

            rMemberReferral memref = GetReferral(util.referralNumber, (Guid)util.memberId);

            if (memref != null)
            {
                if (util.referredToFacilityId > 0)
                {
                    if (util.referredToFacilityRemove)
                    {
                        memref.referred_to_department_id = 0;
                    }
                    else
                    {
                        memref.referred_to_department_id = util.referredToFacilityId;
                    }
                }
                else if (util.referredToFacilityRemove)
                {
                    memref.referred_to_department_id = 0;
                }
                
                _icmsContext.MemberReferrals.Update(memref);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    referral = new Utilization();

                    referral.memberId = memref.member_id;
                    referral.referralNumber = memref.referral_number;
                    referral.referredToFacilityId = memref.referred_to_department_id;
                    referral.referredToFacilityName = getFacilityName((int)memref.referred_to_department_id);
                    referral.referredToFacilityNpi = getFacilityNpi((int)memref.referred_to_department_id);
                }
            }

            return referral;
        }

        public Utilization updateFacilityAddress(Utilization util)
        {
            Utilization referral = null;

            rMemberReferral memref = GetReferral(util.referralNumber, (Guid)util.memberId);

            if (memref != null)
            {
                if (util.referredToFacilityId > 0 && memref.referred_to_department_id > 0)
                {

                    int result = 0;

                    FacilityAddress dbFacilityAddrs = getFacilityAddress((int)memref.referred_to_department_id);

                    if (dbFacilityAddrs != null)
                    {
                        result = refreshFacilityAddress(dbFacilityAddrs, util);
                    }
                    else
                    {
                        result = insertFacilityAddress(util);                        
                    }

                    if (result > 0)
                    {
                        referral = getReturnReferralFacility(memref);                            
                    }
                }
            }

            return referral;
        }

        private FacilityAddress getFacilityAddress(int facilityId)
        {

            FacilityAddress dbFacilityAddrs = (

                    from deptAddrs in _icmsContext.FacilityAddresses
                    where deptAddrs.id.Equals(facilityId)
                    select deptAddrs
                )
                .FirstOrDefault();

            return dbFacilityAddrs;
        }

        private string getFacilityName(int facilityId)
        {

            string dbFacilityName = (

                    from dept in _icmsContext.rDepartments
                    where dept.id.Equals(facilityId)
                    select dept.label
                )
                .FirstOrDefault();

            return dbFacilityName;
        }

        private string getFacilityNpi(int facilityId)
        {

            string dbFacilityNpi = (

                    from dept in _icmsContext.rDepartments
                    where dept.id.Equals(facilityId)
                    select dept.npi
                )
                .FirstOrDefault();

            return dbFacilityNpi;
        }

        private int refreshFacilityAddress(FacilityAddress dbFacilityAddrs, Utilization util)
        {

            int result = 0;

            try
            {

                dbFacilityAddrs.address_line_one = (util.referredToFacility.address1.Length > 50) ? util.referredToFacility.address1.Substring(0, 50) : util.referredToFacility.address1;

                if (util.referredToFacility.address1.Length > 50)
                {
                    dbFacilityAddrs.address_line_two = util.referredToFacility.address1.Substring(50, 50);
                }

                dbFacilityAddrs.city = util.referredToFacility.city;
                dbFacilityAddrs.state_abbrev = util.referredToFacility.stateAbbrev;
                dbFacilityAddrs.zip_code = util.referredToFacility.zip;

                _icmsContext.FacilityAddresses.Update(dbFacilityAddrs);
                result = _icmsContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }

            return result;
        }

        private int insertFacilityAddress(Utilization util)
        {

            int result = 0;

            FacilityAddress newAddr = new FacilityAddress();

            newAddr.id = util.referredToFacilityId;
            newAddr.address_line_one = util.referredToFacility.address1.Substring(0, 50);

            if (util.referredToFacility.address1.Length > 50)
            {
                newAddr.address_line_two = util.referredToFacility.address1.Substring(50, 50);
            }

            newAddr.city = util.referredToFacility.city;
            newAddr.state_abbrev = util.referredToFacility.stateAbbrev;
            newAddr.zip_code = util.referredToFacility.zip;

            _icmsContext.FacilityAddresses.Add(newAddr);
            result = _icmsContext.SaveChanges();

            return result;
        }

        private Utilization getReturnReferralFacility(rMemberReferral memref)
        {

            Utilization referral = new Utilization();

            referral.memberId = memref.member_id;
            referral.referralNumber = memref.referral_number;
            referral.referredToFacilityId = memref.referred_to_department_id;

            FacilityAddress dbFacilityAddrs = getFacilityAddress((int)memref.referred_to_department_id);

            if (dbFacilityAddrs != null)
            {

                HospitalFacility referredToFacility = convertFacilityAddressToHospitalAddress(dbFacilityAddrs);
                referral.referredToFacility = referredToFacility;
            }

            return referral;
        }

        private HospitalFacility convertFacilityAddressToHospitalAddress(FacilityAddress dbFacilityAddrs)
        {

            HospitalFacility referredToFacility = new HospitalFacility();

            referredToFacility.address1 = dbFacilityAddrs.address_line_one;
            referredToFacility.address2 = dbFacilityAddrs.address_line_two;
            referredToFacility.city = dbFacilityAddrs.city;
            referredToFacility.stateAbbrev = dbFacilityAddrs.state_abbrev;
            referredToFacility.zip = dbFacilityAddrs.zip_code;

            return referredToFacility;
        }

        public Utilization updateFacilityNpi(HospitalFacility facility)
        {

            Utilization referral = null;

            if (facility.hospitalId > 0)
            {
                rDepartment dbFacility = (

                        from refFac in _icmsContext.rDepartments
                        where refFac.id.Equals(facility.hospitalId)
                        select refFac
                    )
                    .FirstOrDefault();

                if (dbFacility != null)
                {

                    dbFacility.npi = facility.npi;

                    _icmsContext.rDepartments.Update(dbFacility);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0 && 
                        !facility.memberId.Equals(Guid.Empty) && 
                        !String.IsNullOrEmpty(facility.referralNumber))
                    {
                        referral = GetPatientReferral(facility.memberId.ToString(), facility.referralNumber);
                    }
                }
            }

            return referral;
        }


        public Utilization updateCodesUm(Utilization util)
        {
            Utilization referral = new Utilization();

            rMemberReferral memref = GetReferral(util.referralNumber, (Guid)util.memberId);


            if (memref != null)
            {
                switch (util.codeType)
                {
                    case "ICD10":
                        referral = updateDiagnosisUm(memref, util);
                        break;

                    case "CPT":
                        referral = updateCptUm(memref, util);
                        break;

                    case "HCPCS":
                        referral = updateHcpcsUm(memref, util);
                        break;
                }

            }

            return referral;
        }

        public Utilization updateDiagnosisUm(rMemberReferral memref, Utilization util)
        {
            
            Utilization referral = new Utilization();
            referral.memberId = memref.member_id;
            referral.referralNumber = memref.referral_number;            

            if (util.diagnosisCodes != null && util.removeCode)
            {
                int removeId = (int)util.diagnosisCodes[0].CodeId;

                List<rMemberReferralDiags> memrefdiags = GetReferralDiags(util.referralNumber, (Guid)util.memberId);

                foreach (rMemberReferralDiags icd10 in memrefdiags)
                {
                    if (icd10.id.Equals(removeId))
                    {
                        _icmsContext.MemberReferralDiags.Remove(icd10);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                        }
                    }
                    else
                    {
                        MedicalCode addCode = new MedicalCode();
                        addCode.CodeType = "ICD10";
                        addCode.CodeId = icd10.id;
                        addCode.Code = icd10.diagnosis_or_procedure_code;

                        DiagnosisCodes10 diagsDescr = (
                                                        from diacodes in _icmsContext.DiagnosisCodes10
                                                        where diacodes.diagnosis_code.Equals(addCode.Code)
                                                        select diacodes
                                                       )
                                                       .FirstOrDefault();

                        if (diagsDescr != null)
                        {
                            addCode.DisplayDescription = diagsDescr.medium_description;
                            addCode.LongDescription = diagsDescr.long_description;
                            addCode.ShortDescription = diagsDescr.short_description;
                        }


                        if (referral.diagnosisCodes == null)
                        {
                            referral.diagnosisCodes = new List<MedicalCode>();
                        }

                        referral.diagnosisCodes.Add(addCode);
                    }
                }

            } 
            else if (util.diagnosisCodes != null)
            {
                int diag10Id = (int)util.diagnosisCodes[0].CodeId;

                DiagnosisCodes10 diag10Code = (
                                                from diags in _icmsContext.DiagnosisCodes10
                                                where diags.diagnosis_codes_10_id.Equals(diag10Id)
                                                select diags
                                              )
                                              .FirstOrDefault();

                if (diag10Code != null)
                {

                    if (DiagnosisNotInUm(memref, diag10Code))
                    {
                        rMemberReferralDiags newReferralDiag = new rMemberReferralDiags();
                        newReferralDiag.creation_date = DateTime.Now;
                        newReferralDiag.diagnosis_or_procedure_code = diag10Code.diagnosis_code;
                        newReferralDiag.is_icd_10 = 1;
                        newReferralDiag.member_id = memref.member_id;
                        newReferralDiag.referral_number = memref.referral_number;

                        _icmsContext.MemberReferralDiags.Add(newReferralDiag);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {
                            
                        }
                    }
                    else
                    {
                        referral.returnMsg = "ICD10 already in auth/referral/precert";
                    }

                }

                referral.diagnosisCodes = GetReferralDiagnosisCodes(memref.referral_number, memref.member_id);
            }


            return referral;

        }

        public Utilization updateCptUm(rMemberReferral memref, Utilization util)
        {

            Utilization referral = new Utilization();
            referral.memberId = memref.member_id;
            referral.referralNumber = memref.referral_number;



            if (util.cptCodes != null && util.removeCode)
            {
                int removeId = (int)util.cptCodes[0].CodeId;

                List<rMemberReferralCpts> memrefcpts = GetReferralCpts(util.referralNumber, (Guid)util.memberId);

                foreach (rMemberReferralCpts cpt in memrefcpts)
                {
                    if (cpt.id.Equals(removeId))
                    {
                        _icmsContext.MemberReferralCpts.Remove(cpt);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                        }
                    }
                    else
                    {
                        MedicalCode addCode = new MedicalCode();
                        addCode.CodeType = "CPT";
                        addCode.CodeId = cpt.id;
                        addCode.Code = cpt.cpt_code;

                        CptCodes2015 cptDescr = (
                                                    from cptcodes in _icmsContext.CptCodes
                                                    where cptcodes.cpt_code.Equals(addCode.Code)
                                                    select cptcodes
                                                   )
                                                   .FirstOrDefault();

                        if (cptDescr != null)
                        {
                            addCode.DisplayDescription = cptDescr.cpt_descr;
                            addCode.LongDescription = cptDescr.cpt_descr;
                            addCode.ShortDescription = cptDescr.cpt_descr;
                        }


                        if (referral.cptCodes == null)
                        {
                            referral.cptCodes = new List<MedicalCode>();
                        }

                        referral.cptCodes.Add(addCode);
                    }
                }

            }
            else if (util.cptCodes != null)
            {
                int cpt2015Id = (int)util.cptCodes[0].CodeId;

                CptCodes2015 cpt2015Code = (
                                                from cpts in _icmsContext.CptCodes
                                                where cpts.cpt_codes_2015_id.Equals(cpt2015Id)
                                                select cpts
                                           )
                                           .FirstOrDefault();

                if (cpt2015Code != null)
                {

                    rMemberReferralCpts newReferralCpt = new rMemberReferralCpts();
                    newReferralCpt.creation_date = DateTime.Now;
                    newReferralCpt.cpt_code = cpt2015Code.cpt_code;
                    newReferralCpt.member_id = memref.member_id;
                    newReferralCpt.referral_number = memref.referral_number;

                    _icmsContext.MemberReferralCpts.Add(newReferralCpt);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {

                    }

                }

                referral.cptCodes = GetReferralCptCodes(memref.referral_number, memref.member_id);
            }


            return referral;

        }
        
        public Utilization updateHcpcsUm(rMemberReferral memref, Utilization util)
        {

            Utilization referral = new Utilization();
            referral.memberId = memref.member_id;
            referral.referralNumber = memref.referral_number;


            if (util.hcpcsCodes != null && util.removeCode)
            {
                int removeId = (int)util.hcpcsCodes[0].CodeId;

                List<rMemberReferralHcpcs> memrefhcpcs = GetReferralHcpcs(util.referralNumber, (Guid)util.memberId);

                foreach (rMemberReferralHcpcs hcpc in memrefhcpcs)
                {
                    if (hcpc.id.Equals(removeId))
                    {
                        _icmsContext.MemberReferralHcpcss.Remove(hcpc);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                        }
                    }
                    else
                    {
                        MedicalCode addCode = new MedicalCode();
                        addCode.CodeType = "HCPCS";
                        addCode.CodeId = hcpc.id;
                        addCode.Code = hcpc.hcpcs_code;

                        Hcpcs2015 hcpcsDesc = (
                                                from hcpcscodes in _icmsContext.HcpcsCodes
                                                where hcpcscodes.hcp_code.Equals(addCode.Code)
                                                select hcpcscodes
                                               )
                                               .FirstOrDefault();

                        if (hcpcsDesc != null)
                        {
                            addCode.DisplayDescription = hcpcsDesc.hcpcs_full;
                            addCode.LongDescription = hcpcsDesc.hcpcs_full;
                            addCode.ShortDescription = hcpcsDesc.hcpcs_short;
                        }


                        if (referral.hcpcsCodes == null)
                        {
                            referral.hcpcsCodes = new List<MedicalCode>();
                        }

                        referral.hcpcsCodes.Add(addCode);
                    }
                }

            }
            else if (util.hcpcsCodes != null)
            {
                int hcpcs2015Id = (int)util.hcpcsCodes[0].CodeId;

                Hcpcs2015 hcpcs2015Code = (
                                            from hcpcs in _icmsContext.HcpcsCodes
                                            where hcpcs.hcpcs_codes_2015_id.Equals(hcpcs2015Id)
                                            select hcpcs
                                          )
                                          .FirstOrDefault();

                if (hcpcs2015Code != null)
                {

                    if (HcpcsNotInUm(memref, hcpcs2015Code))
                    {
                        rMemberReferralHcpcs newReferralHcpcs = new rMemberReferralHcpcs();
                        newReferralHcpcs.creation_date = DateTime.Now;
                        newReferralHcpcs.hcpcs_code = hcpcs2015Code.hcp_code;
                        newReferralHcpcs.member_id = memref.member_id;
                        newReferralHcpcs.referral_number = memref.referral_number;

                        _icmsContext.MemberReferralHcpcss.Add(newReferralHcpcs);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {

                        }

                    }

                }

                referral.hcpcsCodes = GetReferralHcpcsCodes(memref.referral_number, memref.member_id);

            }


            return referral;

        }



        public Utilization addActionUm(Utilization util)
        {

            Utilization returnActions = null;


            if (completeLastWorkflowAction(util))
            {
                if (addWorkflowAction(util))
                {
                    returnActions = GetPatientReferral(util.memberId.ToString(), util.referralNumber);
                }
            }

            return returnActions;
        }

        private bool completeLastWorkflowAction(Utilization util)
        {

            rMemberReferralWorkflow lastAction = (
                                                    from workflw in _icmsContext.rMemberReferralWorkflows
                                                    where workflw.referral_number.Equals(util.referralNumber)
                                                    && workflw.member_id.Equals(util.memberId)
                                                    && workflw.latest_record.Equals(true)
                                                    select workflw
                                                 )
                                                 .FirstOrDefault();

            if (lastAction != null)
            {
                lastAction.completed_date = DateTime.Now;
                lastAction.completed_by_user_id = util.userId;
                lastAction.latest_record = false;

                _icmsContext.rMemberReferralWorkflows.Update(lastAction);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    return true;
                }

            }
            else
            {
                return true;
            }

            return false;
        }

        private bool addWorkflowAction(Utilization util)
        {
            try
            {

                rWorkflowXref wrkflwxref = getWorkflowXref(util);

                if (wrkflwxref != null)
                {

                    DateTime toBeCompletedDate = DateTime.Now.AddDays(7);

                    rMemberReferralWorkflow addAction = new rMemberReferralWorkflow();

                    addAction.referral_number = util.referralNumber;
                    addAction.member_id = (Guid)util.memberId;
                    addAction.r_workflow_xref_id = wrkflwxref.id;
                    addAction.eventtype_id = wrkflwxref.eventtype_id;
                    addAction.created_by_user_id = util.userId;
                    addAction.created_date = DateTime.Now;
                    addAction.assigned_to_user_id = (Guid)util.actions[0].assignToUserId;
                    addAction.to_be_completed_date = toBeCompletedDate;
                    addAction.latest_record = true;

                    switch (wrkflwxref.currentstatus_id)
                    {
                        //close, error, contact with patient
                        case 6:
                        case 11:
                        case 13:
                            addAction.completed_by_user_id = util.userId;
                            addAction.completed_date = toBeCompletedDate;
                            break;
                    }

                    _icmsContext.rMemberReferralWorkflows.Add(addAction);
                    int results = _icmsContext.SaveChanges();

                    if (results > 0)
                    {
                        return true;
                    }

                }

            }
            catch(Exception ex)
            {

            }

            return false;
        }

        private rWorkflowXref getWorkflowXref(Utilization util)
        {
            rWorkflowXref returnXref = null;

            if (util.actions != null && util.actions.Count.Equals(1))
            {

                int currentStatusId = 0;
                int reasonId = 0;

                currentStatusId = util.actions[0].statusId;
                reasonId = util.actions[0].reasonId;

                if (currentStatusId > 0 && reasonId > 0)
                {

                    returnXref = (
                                    from wrkflwxref in _icmsContext.rWorkflowXrefs
                                    where wrkflwxref.currentstatus_id.Equals(currentStatusId)
                                    && wrkflwxref.pendreason_id.Equals(reasonId)
                                    select wrkflwxref
                                 )
                                 .FirstOrDefault();

                }

            }
                         

            return returnXref;
        }




        public Utilization addUtilizationsUm(Utilization util)
        {

            Utilization returnUtilizations = null;

            if (!string.IsNullOrEmpty(util.utilizationType))
            {

                rUtilizationDays newUtil = null;

                switch (util.utilizationType)
                {
                    case "I":
                        newUtil = getNewInpatientUtilization(util);
                        break;

                    case "O":
                        newUtil = getNewOutpatientUtilization(util);
                        break;
                }

                if (newUtil != null)
                {

                    _icmsContext.rUtilizationDayses.Add(newUtil);
                    int results = _icmsContext.SaveChanges();

                    if (results > 0)
                    {
                        bool reviewAdded = addUtilizationsReviews(util, newUtil.line_number);
                    }

                    returnUtilizations = new Utilization();

                    returnUtilizations.utilizations = getReferralUtilizations(util.referralNumber, (Guid)util.memberId, util.utilizationType);

                    if (returnUtilizations.utilizations != null && returnUtilizations.utilizations.Count > 0)
                    {
                        setReferralStartEndDates(ref returnUtilizations);
                    }

                    returnUtilizations.utilizationType = util.utilizationType;
                }
            }

            return returnUtilizations;
        }

        private rUtilizationDays getNewInpatientUtilization(Utilization util)
        {
            rUtilizationDays newUtil = null;

            if (!util.memberId.Equals(Guid.Empty) && !string.IsNullOrEmpty(util.referralNumber) && 
                !string.IsNullOrEmpty(util.utilizationType) && util.utilizations.Count.Equals(1))
            {

                int nextLineNumber = getReferralNextUtilizationLineNumber(util.referralNumber, (Guid)util.memberId);

                if (nextLineNumber > 0)
                {

                    newUtil = new rUtilizationDays();

                    newUtil.referral_number = util.referralNumber;
                    newUtil.member_id = (Guid)util.memberId;
                    newUtil.referral_type = util.utilizationType;
                    newUtil.line_number = nextLineNumber;

                    newUtil.type_id = util.utilizations[0].typeId;
                    newUtil.surgery_flag = util.utilizations[0].surgeryFlag;
                    newUtil.surgery_on_first_day_flag = util.utilizations[0].surgeryOnFirstDayFlag;
                    newUtil.start_date = util.utilizations[0].startDate;
                    newUtil.next_review_date = util.utilizations[0].nextReviewDate;
                    newUtil.util_decision_id = util.utilizations[0].decisionId;
                    newUtil.denial_reason_id = util.utilizations[0].denialReasonId;

                    newUtil.Date_Created = util.utilizations[0].creationDate;

                    newUtil.visits_recurring_flag = false;
                }
            }

            return newUtil;
        }

        private rUtilizationDays getNewOutpatientUtilization(Utilization util)
        {
            rUtilizationDays newUtil = null;

            if (!util.memberId.Equals(Guid.Empty) && !string.IsNullOrEmpty(util.referralNumber) &&
                !string.IsNullOrEmpty(util.utilizationType))
            {

                int nextLineNumber = getReferralNextUtilizationLineNumber(util.referralNumber, (Guid)util.memberId);

                if (nextLineNumber > 0)
                {

                    newUtil = new rUtilizationDays();

                    newUtil.referral_number = util.referralNumber;
                    newUtil.member_id = (Guid)util.memberId;
                    newUtil.referral_type = util.utilizationType;
                    newUtil.line_number = nextLineNumber;

                    newUtil.visits_recurring_flag = util.utilizations[0].visitsRecurring;
                    newUtil.visits_num_per_period_requested = util.utilizations[0].visitsNumPerPeriodRequested;
                    newUtil.visits_num_per_period_authorized = util.utilizations[0].visitsNumPerPeriodAuthorized;
                    newUtil.visits_period_requested = util.utilizations[0].visitsRequested;
                    newUtil.visits_period_authorized = util.utilizations[0].visitsAuthorized;
                    newUtil.visits_num_periods_requested = util.utilizations[0].visitsNumPeriodsRequested;
                    newUtil.visits_num_periods_authorized = util.utilizations[0].visitsNumPeriodsAuthorized;
                    newUtil.visits_authorized_end_date = util.utilizations[0].visitsAuthorizedEndDate;
                    newUtil.visits_authorized_start_date = util.utilizations[0].visitsAuthorizedStartDate;
                    newUtil.util_decision_id = util.utilizations[0].decisionId;
                    newUtil.denial_reason_id = util.utilizations[0].denialReasonId;

                    newUtil.Date_Created = util.utilizations[0].creationDate;

                    newUtil.surgery_flag = false;
                    newUtil.surgery_on_first_day_flag = false;
                }

            }

            return newUtil;
        }

        private int getReferralNextUtilizationLineNumber(string referralNumber, Guid memberId)
        {

            int nextLineNumber = 0;

            int currentLineNumber = (
                    from rDays in _icmsContext.rUtilizationDayses
                    where rDays.referral_number.Equals(referralNumber)
                    && rDays.member_id.Equals(memberId)
                    orderby rDays.line_number descending
                    select rDays.line_number
                )
                .Take(1)
                .FirstOrDefault();

            if (currentLineNumber > 0)
            {
                nextLineNumber = currentLineNumber + 1;
            } 
            else
            {
                nextLineNumber = 1;
            }

            return nextLineNumber;
        }

        private bool addUtilizationsReviews(Utilization util, int lineNumber)
        {

            rUtilizationReviews newReview = null;

            if (lineNumber > 0)
            {

                newReview = new rUtilizationReviews();

                newReview.member_id = util.memberId;
                newReview.referral_number = util.referralNumber;
                newReview.line_number = lineNumber;
                newReview.denial_reason_id = util.utilizations[0].denialReasonId;
                newReview.util_decision_id = util.utilizations[0].decisionId;
                newReview.review_type_items_id = util.utilizations[0].decisionById;
                newReview.creation_date = util.utilizations[0].creationDate;
                newReview.created_user_id = util.utilizations[0].usr;

                _icmsContext.rUtilizationReviewses.Add(newReview);
                int results = _icmsContext.SaveChanges();

                if (results > 0)
                {
                    return true;
                }
            }

            return false;
        }


        public Utilization updateUtilizationsUm(Utilization util)
        {

            Utilization returnUtilizations = null;

            if (util.utilizations != null && util.utilizations.Count.Equals(1) && util.utilizations[0].utilizationItemId > 0)
            {

                rUtilizationDays dbUtil = getUmUtilizationItem(util);

                if (dbUtil != null)
                {

                    switch (util.utilizationType)
                    {
                        case "I":
                            setUtilizationInpatientUpdateItems(util, ref dbUtil);
                            break;

                        case "O":
                            setUtilizationOutpatientUpdateItems(util, ref dbUtil);
                            break;
                    }

                    _icmsContext.rUtilizationDayses.Update(dbUtil);
                    int results = _icmsContext.SaveChanges();

                    if (results > 0)
                    {
                        bool reviewAdded = updateUtilizationsReviews(util, dbUtil.line_number);
                    }

                    returnUtilizations = new Utilization();

                    returnUtilizations.utilizationType = util.utilizationType;
                    returnUtilizations.utilizations = getReferralUtilizations(util.referralNumber, (Guid)util.memberId, util.utilizationType);

                    if (returnUtilizations.utilizations != null && returnUtilizations.utilizations.Count > 0)
                    {
                        setReferralStartEndDates(ref returnUtilizations);
                    }
                }
            }

            return returnUtilizations;
        }

        private rUtilizationDays getUmUtilizationItem(Utilization util)
        {
            
            rUtilizationDays dbUtil = null;

            dbUtil = (

                    from rDays in _icmsContext.rUtilizationDayses
                    where rDays.PatCaseActID.Equals(util.utilizations[0].utilizationItemId)
                    select rDays
                )
                .FirstOrDefault();

            return dbUtil;
        }

        private void setUtilizationInpatientUpdateItems(Utilization util, ref rUtilizationDays dbUtil)
        {

            dbUtil.type_id = util.utilizations[0].typeId;
            dbUtil.surgery_flag = util.utilizations[0].surgeryFlag;
            dbUtil.surgery_on_first_day_flag = util.utilizations[0].surgeryOnFirstDayFlag;
            dbUtil.start_date = util.utilizations[0].startDate;
            dbUtil.next_review_date = util.utilizations[0].nextReviewDate;
            dbUtil.util_decision_id = util.utilizations[0].decisionId;
            dbUtil.denial_reason_id = util.utilizations[0].denialReasonId;

            dbUtil.DateUpdated = util.utilizations[0].creationDate;
        }

        private void setUtilizationOutpatientUpdateItems(Utilization util, ref rUtilizationDays dbUtil)
        {

            dbUtil.visits_recurring_flag = util.utilizations[0].visitsRecurring;
            dbUtil.visits_num_per_period_requested = util.utilizations[0].visitsNumPerPeriodRequested;
            dbUtil.visits_num_per_period_authorized = util.utilizations[0].visitsNumPerPeriodAuthorized;
            dbUtil.visits_period_requested = util.utilizations[0].visitsRequested;
            dbUtil.visits_period_authorized = util.utilizations[0].visitsAuthorized;
            dbUtil.visits_num_periods_requested = util.utilizations[0].visitsNumPeriodsRequested;
            dbUtil.visits_num_periods_authorized = util.utilizations[0].visitsNumPeriodsAuthorized;
            dbUtil.visits_authorized_end_date = util.utilizations[0].visitsAuthorizedEndDate;
            dbUtil.visits_authorized_start_date = util.utilizations[0].visitsAuthorizedStartDate;
            dbUtil.util_decision_id = util.utilizations[0].decisionId;
            dbUtil.denial_reason_id = util.utilizations[0].denialReasonId;

            dbUtil.DateUpdated = util.utilizations[0].creationDate;
        }

        private bool updateUtilizationsReviews(Utilization util, int lineNumber)
        {
            rUtilizationReviews dbReview = null;

            dbReview = (

                    from rRev in _icmsContext.rUtilizationReviewses
                    where rRev.referral_number.Equals(util.referralNumber)
                    && rRev.member_id.Equals(util.memberId)
                    && rRev.line_number.Equals(lineNumber)
                    select rRev
                )
                .FirstOrDefault();

            if (dbReview != null)
            {

                dbReview.denial_reason_id = util.utilizations[0].denialReasonId;
                dbReview.util_decision_id = util.utilizations[0].decisionId;
                dbReview.review_type_items_id = util.utilizations[0].decisionById;
                dbReview.last_update_date = util.utilizations[0].creationDate;
                dbReview.last_update_user_id = util.utilizations[0].usr;

                _icmsContext.rUtilizationReviewses.Update(dbReview);
                int results = _icmsContext.SaveChanges();

                if (results > 0)
                {
                    return true;
                }
            }

            return false;
        }



        public Utilization addUtilizationsUmNote(Note utilNote)
        {

            Utilization returnUtilizations = null;


            double lengthOfNote = utilNote.noteText.Length;

            if (lengthOfNote <= 512)
            {
                returnUtilizations = addUtilizationUmNoteSingle(utilNote);
            } 
            else 
            {
                returnUtilizations = addUtilizationUmNoteMultiple(utilNote, lengthOfNote);                
            }

            return returnUtilizations;
        }

        private Utilization addUtilizationUmNoteSingle(Note note)
        {

            Utilization returnUtilizations = null;

            rUtilizationDaysNotes newNote = new rUtilizationDaysNotes();

            newNote.member_id = note.memberId;
            newNote.referral_number = note.referralNumber;
            newNote.referral_type = note.referralType;
            newNote.record_date = note.recordDate;
            newNote.system_user_id = (Guid)note.caseOwnerId;
            newNote.line_number = note.lineNumber;
            newNote.onletter = note.onLetter;
            newNote.billing_id = note.billingId;
            newNote.RN_notes = note.billingMinutes;

            newNote.record_seq_num = 1;
            newNote.evaluation_text = note.noteText;

            _icmsContext.rUtilizationDaysNoteses.Add(newNote);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                returnUtilizations = new Utilization();
                returnUtilizations.notes = getReferralNotes(note.referralNumber, note.memberId);
            }

            return returnUtilizations;
        }

        private Utilization addUtilizationUmNoteMultiple(Note note, double lengthOfNote)
        {

            Utilization returnUtilizations = null;
            bool added = false;

            double numberOfLines = 0;
            int totalNumberOfLines = 0;
            int starting = 0;
            double remainingLengthOfNote = 0;

            numberOfLines = lengthOfNote / 512;

            if (numberOfLines > 0)
            {

                totalNumberOfLines = getUtilizationNoteTotalNumberOfLines(numberOfLines);

                if (totalNumberOfLines > 0)
                {

                    for (int i = 0; i < totalNumberOfLines; i++)
                    {

                        rUtilizationDaysNotes newNote = new rUtilizationDaysNotes();

                        newNote.member_id = note.memberId;
                        newNote.referral_number = note.referralNumber;
                        newNote.referral_type = note.referralType;
                        newNote.record_date = note.recordDate;
                        newNote.system_user_id = (Guid)note.caseOwnerId;
                        newNote.line_number = note.lineNumber;
                        newNote.onletter = note.onLetter;
                        newNote.billing_id = note.billingId;
                        newNote.RN_notes = note.billingMinutes;
                        newNote.record_seq_num = i + 1;

                        starting = i * 512 + 1;

                        if (starting > 512)
                        {

                            remainingLengthOfNote = lengthOfNote - starting;                            
                            newNote.evaluation_text = note.noteText.Substring(starting, Convert.ToInt32(remainingLengthOfNote));
                        }
                        else
                        {
                            newNote.evaluation_text = note.noteText.Substring(starting, 512);
                        }

                        _icmsContext.rUtilizationDaysNoteses.Add(newNote);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0 && !added)
                        {
                            added = true;
                        }
                    }
                }
            }


            if (added)
            {

                returnUtilizations = new Utilization();
                returnUtilizations.notes = getReferralNotes(note.referralNumber, note.memberId);
            }

            return returnUtilizations;
        }

        private int getUtilizationNoteTotalNumberOfLines(double numberOfLines)
        {

            if (numberOfLines == Convert.ToInt32(numberOfLines))
            {
                return Convert.ToInt32(numberOfLines);
            }
            else
            {
                return Convert.ToInt32(numberOfLines) + 1;
            }
        }


        public List<Note> suspendUtilizationsUmNote(Note utilNote)
        {

            List<Note> suspendNotes = null;

            if (!string.IsNullOrEmpty(utilNote.referralNumber))
            {

                SuspendedNotes newSuspend = new SuspendedNotes();

                newSuspend.note_type = "U/M";
                newSuspend.referral_number = utilNote.referralNumber;
                newSuspend.note_text = utilNote.noteText;
                newSuspend.billing_id = utilNote.billingId;
                newSuspend.RN_notes = utilNote.billingMinutes;
                newSuspend.creation_date = utilNote.recordDate;
                newSuspend.creation_user_id = utilNote.caseOwnerId;

                _icmsContext.SuspendedNoteses.Add(newSuspend);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    suspendNotes = getReferralSuspendNotes(utilNote.referralNumber);
                }
            }

            return suspendNotes;
        }

        public bool removeSuspendNote(Note utilNote)
        {

            // **-> The "removeSuspendNote" function checks the "utilNote.suspendNoteId" value before removing
            // **-> "Suspend Note"

            NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);
            return noteServ.removeSuspendNote(utilNote);
        }



        public Utilization addUtilizationsUmSaving(Saving utilSaving)
        {

            Utilization returnUtilizations = null;

            int savingsLine = getUtilizationSavingsNextLine(utilSaving);

            rUtilizationSavings refSavings = new rUtilizationSavings();

            refSavings.utilization_savings_id = Guid.NewGuid();
            refSavings.member_id = utilSaving.memberId;
            refSavings.referral_number = utilSaving.referralNumber;
            refSavings.referral_type = utilSaving.referralType;
            refSavings.line_number = utilSaving.utilizationLineNumber;
            refSavings.savings_line = savingsLine;
            refSavings.item_description = utilSaving.itemDescription;
            refSavings.saving_units_id = utilSaving.savingUnitsId;
            refSavings.quantity = utilSaving.quantity;
            refSavings.cost = utilSaving.cost;
            refSavings.negotiated = utilSaving.negotiated;
            refSavings.savings = utilSaving.savings;
            refSavings.system_user_id = utilSaving.usr;
            refSavings.date_updated = utilSaving.createdDate;

            _icmsContext.rUtilizationSavingses.Add(refSavings);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                returnUtilizations = new Utilization();
                returnUtilizations.savings = getReferralSavings(utilSaving.referralNumber, (Guid)utilSaving.memberId);
            }

            return returnUtilizations;
        }

        private int getUtilizationSavingsNextLine(Saving utilSaving)
        {
            int lineNumber = 1;


            rUtilizationSavings refSave = (

                    from utilsave in _icmsContext.rUtilizationSavingses
                    where utilsave.referral_number.Equals(utilSaving.referralNumber)
                    && utilsave.referral_type.Equals(utilSaving.referralType)
                    orderby utilsave.savings_line descending
                    select utilsave
                )
                .Take(1)
                .FirstOrDefault();

            if (refSave != null && refSave.line_number > 0)
            {
                lineNumber = (int)refSave.line_number + 1;
            }

            return lineNumber;
        }



        public Utilization addUtilizationsUmMedicalReviewRequest(Utilization util)
        {
            
            Utilization utilRequest = addUtilizationsUmRequest(util.requests[0]);
            Utilization utilAction = addActionUm(util);

            Utilization returnUtilizations = GetPatientReferral(util.memberId.ToString(), util.referralNumber);            

            return returnUtilizations;
        }



        public Utilization addUtilizationsUmRequest(UtilizationRequest utilReq)
        {

            Utilization returnUtilizations = null;

            int requestId = 0;

            DateTime endDate = (DateTime)utilReq.createDate.Value.AddDays(2);

            if (utilReq.reviewRequestId > 0)
            {
                if (utilReq.reviewQuestionId > 0)
                {
                    requestId = addReviewAnswer(utilReq, endDate);
                }
                else
                {
                    requestId = addReviewQuestion(utilReq, endDate);
                }
            } 
            else
            {
                requestId = addReviewRequest(utilReq, endDate);                
            }

            if (requestId > 0)
            {
                if (!string.IsNullOrEmpty(utilReq.emailAddress))
                {

                    int emailId = 0;

                    if (utilReq.reviewRequestId > 0)
                    {

                        if (utilReq.reviewQuestionId > 0)
                        {
                            emailId = sendAnswerEmail(utilReq);
                        }
                        else
                        {
                            emailId = sendQuestionEmail(utilReq);   
                        }

                    } else
                    {
                        emailId = sendRequestEmail(utilReq);
                    }     
                    
                    if (emailId > 0)
                    {

                        if (utilReq.reviewRequestId > 0)
                        {
                            if (utilReq.reviewQuestionId > 0)
                            {
                                addReviewAnswerEmailReference(requestId, emailId);
                            }
                            else
                            {
                                addReviewQuestionEmailReference(requestId, emailId);
                            }
                        }
                        else
                        {
                            addReviewRequestEmailReference(requestId, emailId);
                        }
                    }
                }

                returnUtilizations = new Utilization();
                returnUtilizations.requests = getReferralRequests(utilReq.referralNumber, (Guid)utilReq.memberId);
            }

            return returnUtilizations;
        }

        private int addReviewAnswer(UtilizationRequest utilReq, DateTime endDate)
        {

            MdReviewQuestion dbQuestions = null;

            if (utilReq.reviewQuestionId > 0)
            {
                dbQuestions = (

                        from mdQuest in _icmsContext.MdReviewQuestions
                        where mdQuest.md_review_question_id.Equals(utilReq.reviewQuestionId)
                        select mdQuest
                    )
                    .FirstOrDefault();

                if (dbQuestions != null)
                {

                    dbQuestions.md_answer_note = utilReq.requestNote;
                    dbQuestions.completed = true;
                    dbQuestions.completed_date = utilReq.createDate;
                    dbQuestions.completed_by_system_user_id = utilReq.usr;
                    dbQuestions.actual_end_action_date = utilReq.createDate;

                    _icmsContext.MdReviewQuestions.Update(dbQuestions);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        return dbQuestions.md_review_question_id;
                    }
                }
            }

            return 0;
        }
        private int addReviewQuestion(UtilizationRequest utilReq, DateTime endDate)
        {

            MdReviewQuestion newQuestion = new MdReviewQuestion();

            newQuestion.task_id = 399;
            newQuestion.task_note = "A MD Review Question was entered via SIMS";
            newQuestion.assigned_to_system_user_id = utilReq.assignedToUserId;
            newQuestion.start_action_date = utilReq.createDate;
            newQuestion.end_action_date = endDate;
            newQuestion.completed = false;
            newQuestion.entered_by_system_user_id = utilReq.usr;
            newQuestion.date_entered = utilReq.createDate;
            newQuestion.creation_date = utilReq.createDate;
            newQuestion.md_question_note = utilReq.requestNote;
            newQuestion.referral_number = utilReq.referralNumber;
            newQuestion.member_id = utilReq.memberId;
            newQuestion.md_review_request_id = utilReq.reviewRequestId;

            _icmsContext.MdReviewQuestions.Add(newQuestion);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return newQuestion.md_review_question_id;
            }
            
            return 0;
        }
        private int addReviewRequest(UtilizationRequest utilReq, DateTime endDate)
        {

            MdReviewRequest newRequest = new MdReviewRequest();

            newRequest.task_id = 401;
            newRequest.task_note = "A MD Review Request was entered via SIMS";
            newRequest.assigned_to_system_user_id = utilReq.assignedToUserId;
            newRequest.start_action_date = utilReq.createDate;
            newRequest.end_action_date = endDate;
            newRequest.completed = false;
            newRequest.entered_by_system_user_id = utilReq.usr;
            newRequest.date_entered = utilReq.createDate;
            newRequest.creation_date = utilReq.createDate;
            newRequest.md_review_appeal_note = utilReq.requestNote;
            newRequest.referral_number = utilReq.referralNumber;
            newRequest.member_id = utilReq.memberId;

            _icmsContext.MdReviewRequests.Add(newRequest);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return newRequest.md_review_request_id;
            }

            return 0;
        }
        private int sendRequestEmail(UtilizationRequest utilReq)
        {

            Email requestEmail = new Email();

            requestEmail.creationDate = ((utilReq.createDate.HasValue) ? utilReq.createDate.Value : DateTime.Now);
            requestEmail.usr = (Guid)utilReq.usr;
            requestEmail.referralNumber = utilReq.referralNumber;
            requestEmail.memberId = utilReq.memberId;

            requestEmail.emailTypeId = 22; //md request
            requestEmail.emailAddress = utilReq.emailAddress;
            requestEmail.emailSubject = "MD Review Submitted On " +
                ((utilReq.createDate.HasValue) ? utilReq.createDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString());

            string text = "A MD review request has recently been entered in the DBMS system. ";
            text += "Please review this request at your earliest convenience by visiting: " + Environment.NewLine + Environment.NewLine;
            text += "https://ecare.dbms-inc.com/EMRSpanish/Roles/Common/NonAuth/Login.aspx";
            text += Environment.NewLine + Environment.NewLine;
            text += "DBMS, Inc." + Environment.NewLine;
            text += "317.582.1200 " + Environment.NewLine;
            text += "107 Crosspoint Blvd Suite 250 Indianapolis IN 46256";
            text += Environment.NewLine + Environment.NewLine;

            string confident = getConfidentialityNoticeText();

            requestEmail.emailText = text + confident;

            return sendOutboundEmail(requestEmail);
        }
        private int sendQuestionEmail(UtilizationRequest utilReq)
        {

            Email requestEmail = new Email();

            requestEmail.creationDate = ((utilReq.createDate.HasValue) ? utilReq.createDate.Value : DateTime.Now);
            requestEmail.usr = (Guid)utilReq.usr;
            requestEmail.referralNumber = utilReq.referralNumber;
            requestEmail.memberId = utilReq.memberId;

            string authNumber = "";

            if (!string.IsNullOrEmpty(requestEmail.referralNumber))
            {
                authNumber = getReferralAuthNumber(requestEmail.referralNumber);

                if (string.IsNullOrEmpty(authNumber))
                {
                    authNumber = requestEmail.referralNumber;
                }
            }

            requestEmail.emailTypeId = 22; //md request
            requestEmail.emailAddress = utilReq.emailAddress;
            requestEmail.emailSubject = "MD Review QUESTION Submitted On " +
                ((utilReq.createDate.HasValue) ? utilReq.createDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString());

            string text = "A MD review question has recently been entered in the DBMS system. ";
            text += "Please review this question at your earliest convenience by visiting: " + Environment.NewLine + Environment.NewLine;
            text += "https://ecare.dbms-inc.com/EMRSpanish/Roles/Common/NonAuth/Login.aspx";
            text += Environment.NewLine + Environment.NewLine;
            text += "Auth/Referral #: " + authNumber + Environment.NewLine + Environment.NewLine;
            text += "DBMS, Inc." + Environment.NewLine;
            text += "317.582.1200 " + Environment.NewLine;
            text += "107 Crosspoint Blvd Suite 250 Indianapolis IN 46256";
            text += Environment.NewLine + Environment.NewLine;

            string confident = getConfidentialityNoticeText();

            requestEmail.emailText = text + confident;

            return sendOutboundEmail(requestEmail);
        }
        private int sendAnswerEmail(UtilizationRequest utilReq)
        {

            Email requestEmail = new Email();

            requestEmail.creationDate = ((utilReq.createDate.HasValue) ? utilReq.createDate.Value : DateTime.Now);
            requestEmail.usr = (Guid)utilReq.usr;
            requestEmail.referralNumber = utilReq.referralNumber;
            requestEmail.memberId = utilReq.memberId;

            string authNumber = "";

            if (!string.IsNullOrEmpty(requestEmail.referralNumber))
            {
                authNumber = getReferralAuthNumber(requestEmail.referralNumber);

                if (string.IsNullOrEmpty(authNumber))
                {
                    authNumber = requestEmail.referralNumber;
                }
            }

            requestEmail.emailTypeId = 22; //md request
            requestEmail.emailAddress = utilReq.emailAddress;
            requestEmail.emailSubject = "MD Review ANSWER Submitted On " +
                ((utilReq.createDate.HasValue) ? utilReq.createDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString());

            string text = "A MD review answer has recently been entered in the DBMS system. ";
            text += "Please review this answer at your earliest convenience by visiting: " + Environment.NewLine + Environment.NewLine;
            text += "https://ecare.dbms-inc.com/EMRSpanish/Roles/Common/NonAuth/Login.aspx";
            text += Environment.NewLine + Environment.NewLine;
            text += "Auth/Referral #: " + authNumber + Environment.NewLine + Environment.NewLine;
            text += "DBMS, Inc." + Environment.NewLine;
            text += "317.582.1200 " + Environment.NewLine;
            text += "107 Crosspoint Blvd Suite 250 Indianapolis IN 46256";
            text += Environment.NewLine + Environment.NewLine;

            string confident = getConfidentialityNoticeText();

            requestEmail.emailText = text + confident;

            return sendOutboundEmail(requestEmail);
        }
        private void addReviewAnswerEmailReference(int reviewQuestionId, int emailId)
        {

            MdReviewQuestion mdQuestion = null;

            mdQuestion = (

                    from mdQuest in _icmsContext.MdReviewQuestions
                    where mdQuest.md_review_question_id.Equals(reviewQuestionId)
                    select mdQuest
                )
                .FirstOrDefault();

            if (mdQuestion != null)
            {

                mdQuestion.answer_email_outbound_id = emailId;

                _icmsContext.MdReviewQuestions.Update(mdQuestion);
                _icmsContext.SaveChanges();
            }
        }
        private void addReviewQuestionEmailReference(int reviewQuestionId, int emailId)
        {

            MdReviewQuestion mdQuestion = null;

            mdQuestion = (

                    from mdQuest in _icmsContext.MdReviewQuestions
                    where mdQuest.md_review_question_id.Equals(reviewQuestionId)
                    select mdQuest
                )
                .FirstOrDefault();

            if (mdQuestion != null)
            {

                mdQuestion.question_email_outbound_id = emailId;

                _icmsContext.MdReviewQuestions.Update(mdQuestion);
                _icmsContext.SaveChanges();
            }
        }
        private void addReviewRequestEmailReference(int reviewId, int emailId)
        {

            MdReviewRequest mdRequest = null;

            mdRequest = (

                    from mdReq in _icmsContext.MdReviewRequests
                    where mdReq.md_review_request_id.Equals(reviewId)
                    select mdReq 
                )
                .FirstOrDefault();

            if (mdRequest != null)
            {

                mdRequest.request_email_outbound_id = emailId;

                _icmsContext.MdReviewRequests.Update(mdRequest);
                _icmsContext.SaveChanges();
            }
        }
        private void addReviewDeterminationEmailReference(int reviewDeterminationId, int emailId)
        {

            MdReviewDetermination mdDetermination = null;

            mdDetermination = (

                    from mdDeterm in _icmsContext.MdReviewDeterminations
                    where mdDeterm.md_review_determination_id.Equals(reviewDeterminationId)
                    select mdDeterm
                )
                .FirstOrDefault();

            if (mdDetermination != null)
            {

                mdDetermination.determination_email_outbound_id = emailId;

                _icmsContext.MdReviewDeterminations.Update(mdDetermination);
                _icmsContext.SaveChanges();
            }
        }


        public Utilization addUtilizationsUmRequestDetermination(UtilizationRequest utilReq)
        {

            Utilization returnUtilizations = null;            

            DateTime endDate = (DateTime)utilReq.createDate.Value.AddDays(2);

            int reviewDeterminationId = addReviewDetermination(utilReq, endDate);

            if (reviewDeterminationId > 0)
            {

                utilReq.reviewDeterminationId = reviewDeterminationId;

                addDeterminationUmNote(utilReq);
                completeUtilizationRequestUm(utilReq);

                if (!utilReq.usr.Equals(Guid.Empty))
                {

                    IcmsUserService userServ = new IcmsUserService(_icmsContext, _aspNetContext);
                    IcmsUser reviewUser = userServ.getIcmsUser(utilReq.usr.ToString());

                    if (!string.IsNullOrEmpty(reviewUser.emailAddress))
                    {
                        utilReq.emailAddress = reviewUser.emailAddress;

                        sendDeterminationEmail(utilReq);
                    }
                }

                returnUtilizations = new Utilization();
                returnUtilizations.requests = getReferralRequests(utilReq.referralNumber, (Guid)utilReq.memberId);
            }

            return returnUtilizations;
        }
        private int addReviewDetermination(UtilizationRequest utilReq, DateTime endDate)
        {

            int reviewDeterminationId = 0;

            MdReviewDetermination newDetermination = new MdReviewDetermination();

            newDetermination.task_id = 400;
            newDetermination.task_note = "A MD Review Determination was entered via SIMS";
            newDetermination.assigned_to_system_user_id = utilReq.assignedToUserId;
            newDetermination.start_action_date = utilReq.createDate;
            newDetermination.end_action_date = endDate;
            newDetermination.completed = false;
            newDetermination.entered_by_system_user_id = utilReq.usr;
            newDetermination.date_entered = utilReq.createDate;
            newDetermination.creation_date = utilReq.createDate;
            newDetermination.md_review_determination_note = utilReq.requestNote;
            newDetermination.referral_number = utilReq.referralNumber;
            newDetermination.member_id = utilReq.memberId.ToString();
            newDetermination.md_review_request_id = utilReq.reviewRequestId;
            newDetermination.util_decision_id = utilReq.decisionId;

            _icmsContext.MdReviewDeterminations.Add(newDetermination);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                reviewDeterminationId = newDetermination.md_review_determination_id;
            }

            return reviewDeterminationId;
        }
        private void addDeterminationUmNote(UtilizationRequest utilReq)
        {

            Note utilNote = new Note();

            utilNote.memberId = utilReq.memberId;
            utilNote.referralNumber = utilReq.referralNumber;
            utilNote.referralType = getReferralUtilizationType(utilReq.referralNumber, utilReq.memberId);
            utilNote.lineNumber = 0;
            utilNote.recordDate = (DateTime)utilReq.createDate;
            utilNote.noteText = utilReq.requestNote;
            utilNote.caseOwnerId = utilReq.assignedToUserId;
            utilNote.onLetter = true;

            addUtilizationsUmNote(utilNote);
        }
        private void sendDeterminationEmail(UtilizationRequest utilReq)
        {

            Email requestEmail = new Email();

            requestEmail.creationDate = ((utilReq.createDate.HasValue) ? utilReq.createDate.Value : DateTime.Now);
            requestEmail.usr = (Guid)utilReq.usr;
            requestEmail.referralNumber = utilReq.referralNumber;
            requestEmail.memberId = utilReq.memberId;

            string authNumber = "";

            if (!string.IsNullOrEmpty(requestEmail.referralNumber))
            {
                authNumber = getReferralAuthNumber(requestEmail.referralNumber);

                if (string.IsNullOrEmpty(authNumber))
                {
                    authNumber = requestEmail.referralNumber;
                }
            } 

            requestEmail.emailTypeId = 22; //md request
            requestEmail.emailAddress = utilReq.emailAddress;
            requestEmail.emailSubject = "MD Review DETERMINATION Submitted On " +
                ((utilReq.createDate.HasValue) ? utilReq.createDate.Value.ToShortDateString() : DateTime.Now.ToShortDateString());

            string text = "A MD review determination has recently been entered in the DBMS system. ";
            text += "Please review this determination at your earliest convenience by visiting: " + Environment.NewLine + Environment.NewLine;
            text += "https://ecare.dbms-inc.com/EMRSpanish/Roles/Common/NonAuth/Login.aspx";
            text += Environment.NewLine + Environment.NewLine;
            text += "Auth/Referral #: " + authNumber + Environment.NewLine + Environment.NewLine;
            text += "DBMS, Inc." + Environment.NewLine;
            text += "317.582.1200 " + Environment.NewLine;
            text += "107 Crosspoint Blvd Suite 250 Indianapolis IN 46256";
            text += Environment.NewLine + Environment.NewLine;

            string confident = getConfidentialityNoticeText();

            requestEmail.emailText = text + confident;

            sendOutboundEmail(requestEmail);
        }

        private bool completeUtilizationRequestUm(UtilizationRequest utilReq)
        {

            if (utilReq.reviewRequestId > 0)
            {

                MdReviewRequest medReview = (

                        from medRev in _icmsContext.MdReviewRequests
                        where medRev.md_review_request_id.Equals(utilReq.reviewRequestId)
                        select medRev
                    )
                    .FirstOrDefault();

                if (medReview != null)
                {

                    medReview.completed = true;
                    medReview.completed_date = DateTime.Now;
                    medReview.completed_by_system_user_id = utilReq.usr;

                    if (utilReq.reviewDeterminationId > 0)
                    {                    
                        medReview.md_review_determination_id = utilReq.reviewDeterminationId;
                    }

                    _icmsContext.MdReviewRequests.Update(medReview);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private string getConfidentialityNoticeText()
        {

            string notice = "CONFIDENTIALITY NOTICE:  The materials in this electronic mail transmission (including all attachments) are private ";
            notice += "and confidential and are the property of the sender. The information contained in the material is privileged and is ";
            notice += "intended only for the use of the named and intended recipient. If you are not the intended recipient, please be advised ";
            notice += "that any unauthorized disclosure, copying, distribution, or use of this electronic mail and/or attachments, or the taking ";
            notice += "of any action in reliance on the contents of this material is strictly prohibited. If you have received this electronic ";
            notice += "mail message in error, please notify us immediately by returning it to the sender and deleting it from your system. ";
            notice += "Delivery of this message to any person other than the intended recipient is not intended in any way to waive privilege ";
            notice += "or confidentiality.";

            return notice;
        }




        private rMemberReferralLetters getUmLetterItem(Utilization util)
        {

            rMemberReferralLetters dbLetter = null;

            dbLetter = (

                    from refLetter in _icmsContext.rMemberReferralLetterses
                    where refLetter.id.Equals(util.letters[0].letterId)
                    select refLetter
                )
                .FirstOrDefault();

            return dbLetter;
        }


        public Utilization removeUtilizationsUm(Utilization util)
        {

            Utilization returnUtilizations = null;

            if (util.utilizations != null && util.utilizations.Count.Equals(1) && util.utilizations[0].utilizationItemId > 0)
            {

                rUtilizationDays dbUtil = getUmUtilizationItem(util);

                if (dbUtil != null)
                {

                    dbUtil.removed = true;
                    dbUtil.removed_date = util.creationDate;
                    dbUtil.removed_user_id = util.userId;

                    _icmsContext.rUtilizationDayses.Update(dbUtil);
                    _icmsContext.SaveChanges();

                    returnUtilizations = new Utilization();

                    returnUtilizations.utilizationType = util.utilizationType;
                    returnUtilizations.utilizations = getReferralUtilizations(util.referralNumber, (Guid)util.memberId, util.utilizationType);

                    if (returnUtilizations.utilizations != null && returnUtilizations.utilizations.Count > 0)
                    {
                        setReferralStartEndDates(ref returnUtilizations);
                    }
                }
            }

            return returnUtilizations;
        }

        public Utilization removeLetterUm(Utilization util)
        {

            Utilization returnUtilizations = null;

            if (util.letters != null && util.letters.Count.Equals(1) && util.letters[0].letterId > 0)
            {

                rMemberReferralLetters dbLetter = getUmLetterItem(util);

                if (dbLetter != null)
                {

                    dbLetter.removed = true;
                    dbLetter.removed_date = util.creationDate;
                    dbLetter.removed_user_id = util.userId;

                    _icmsContext.rMemberReferralLetterses.Update(dbLetter);
                    _icmsContext.SaveChanges();

                    returnUtilizations = new Utilization();

                    returnUtilizations.letters = getReferralLetters(util.referralNumber, (Guid)util.memberId);
                }
            }

            return returnUtilizations;
        }



        public bool DiagnosisNotInUm(rMemberReferral memref, DiagnosisCodes10 diag10Code)
        {
            rMemberReferralDiags memrefDiags = (
                                                from memreficd in _icmsContext.MemberReferralDiags
                                                where memreficd.referral_number.Equals(memref.referral_number)
                                                && memreficd.member_id.Equals(memref.member_id)
                                                && memreficd.diagnosis_or_procedure_code.Equals(diag10Code.diagnosis_code)
                                                select memreficd    
                                               )
                                               .FirstOrDefault();

            if (memrefDiags != null)
            {
                return false;
            }


            return true;

        }

        
        public bool HcpcsNotInUm(rMemberReferral memref, Hcpcs2015 hcpcs2015Code)
        {
            rMemberReferralHcpcs memrefHcpcs = (
                                                    from memrefhcp in _icmsContext.MemberReferralHcpcss
                                                    where memrefhcp.referral_number.Equals(memref.referral_number)
                                                    && memrefhcp.member_id.Equals(memref.member_id)
                                                    && memrefhcp.hcpcs_code.Equals(hcpcs2015Code.hcp_code)
                                                    select memrefhcp
                                               )
                                               .FirstOrDefault();

            if (memrefHcpcs != null)
            {
                return false;
            }


            return true;

        }



        private int sendOutboundEmail(Email outboundEmail)
        {

            EmailsOutbound newEmail = new EmailsOutbound();
            
            newEmail.creation_date = outboundEmail.creationDate;
            newEmail.user_id = outboundEmail.usr;
            newEmail.referral_number = outboundEmail.referralNumber;
            newEmail.member_id = outboundEmail.memberId;

            newEmail.email_type_id = outboundEmail.emailTypeId;
            newEmail.email_address = outboundEmail.emailAddress;
            newEmail.email_to = outboundEmail.emailAddress;
            newEmail.email_subject = outboundEmail.emailSubject;
            newEmail.email_message = outboundEmail.emailText;

            if (!string.IsNullOrEmpty(outboundEmail.ccEmailList))
            {
                newEmail.email_cc_list = outboundEmail.ccEmailList;
            }

            _icmsContext.EmailsOutbounds.Add(newEmail);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return newEmail.email_outbound_id;
            }

            return 0;
        }

    }

}

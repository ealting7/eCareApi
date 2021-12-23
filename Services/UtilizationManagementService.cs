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

namespace eCareApi.Services
{
    public class UtilizationManagementService : IUtilizationManagement
    {
        private readonly IcmsContext _icmsContext;


        public UtilizationManagementService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
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



        public List<Doctor> GetProviderSearch(SearchParams srchParams)
        {
            List<Doctor> drs = new List<Doctor>();

            DoctorService docService = new DoctorService(_icmsContext);

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

                                       where memrefs.member_id.Equals(memberId)
                                       && memrefs.referral_number.Equals(refId)
                                       select new Utilization
                                       {
                                           memberId = memrefs.member_id,
                                           referralNumber = memrefs.referral_number,
                                           startDate = memrefs.auth_start_date,
                                           endDate = memrefs.auth_end_date,
                                           referralTypeId = memrefs.type_id,
                                           referralType = referralTypes.label,
                                           referralContextId = memrefs.context_id,
                                           referralCategoryId = memrefs.referral_category,
                                           referralReasonId = memrefs.reason_id,
                                           referredByPcpId = memrefs.referring_pcp_id,
                                           referredByPcpName = byProviders.provider_first_name + " " + byProviders.provider_last_name,
                                           referredByPcpNpi = (!string.IsNullOrEmpty(byProviders.npi)) ? byProviders.npi : byProviders.billing_npi,
                                           referredToPcpId = memrefs.referred_to_pcp_id,
                                           referredToPcpName = toProviders.provider_first_name + " " + toProviders.provider_last_name,
                                           referredToPcpNpi = (!string.IsNullOrEmpty(toProviders.npi)) ? toProviders.npi : toProviders.billing_npi,
                                       }
                       )
                       .OrderByDescending(auth => auth.startDate)
                       .ThenByDescending(auth => auth.endDate)
                       .FirstOrDefault();


                if (patRefs != null)
                {

                    referral.memberId = patRefs.memberId;
                    referral.referralNumber = patRefs.referralNumber;
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

                    //CODES
                    referral.diagnosisCodes = GetReferralCodes("ICD10", refId, memberId);
                    referral.cptCodes = GetReferralCodes("CPT", refId, memberId);
                    referral.hcpcsCodes = GetReferralCodes("HCPCS", refId, memberId);

                }

            }



            return referral;
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

            returnReferral = (
                                from rmemref in _icmsContext.MemberReferrals
                                where rmemref.referral_number.Equals(refNumber)
                                && rmemref.member_id.Equals(memId)
                                select rmemref
                             )
                             .FirstOrDefault();

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




        public Utilization updateGeneralUm(Utilization util)
        {
            Utilization referral = new Utilization();

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
                    referral.memberId = memref.member_id;
                    referral.referralNumber = memref.referral_number;
                    referral.referralTypeId = memref.type_id;
                    referral.referralContextId = memref.context_id;
                    referral.referralCategoryId = memref.referral_category;
                    referral.referralReasonId = memref.reason_id;
                }
            }

            return referral;
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

    }

}

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
    public class CaseManagementService : ICaseManagement
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _dataStagingContext;

        public CaseManagementService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext dataStagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _dataStagingContext = dataStagingContext ?? throw new ArgumentNullException(nameof(dataStagingContext));
        }


        public List<IcmsUser> getCmCaseOwners()
        {

            List<IcmsUser> caseOwners = null;

            caseOwners = (

                    from sysUsr in _icmsContext.SystemUsers

                    join sysUsrRle in _icmsContext.SystemUserRoles
                    on sysUsr.system_user_id equals sysUsrRle.system_user_id

                    where sysUsrRle.system_role_id.Equals(new Guid("A3B1DAEF-E201-4B0A-B624-46A5E39212EF"))
                    && sysUsr.user_inactive_flag.Equals(false)
                    && (sysUsr.discipline_id.Equals(53) || sysUsr.client_services_admin_flag > 0)

                    orderby sysUsr.system_user_last_name, sysUsr.system_user_first_name

                    select new IcmsUser
                    {
                        UserId = sysUsr.system_user_id,
                        FullName = sysUsr.system_user_first_name + " " + sysUsr.system_user_last_name,
                        FirstName = sysUsr.system_user_first_name,
                        LastName = sysUsr.system_user_last_name
                    }
                )
                .ToList();

            return caseOwners;
        }

        public List<Case> getCaseOwnerCases(string caseOwnerId)
        {
            
            List<Case> cases = null;

            Guid userId = Guid.Empty;

            if (Guid.TryParse(caseOwnerId, out userId))
            {
                List<Case> foundCases = getCasesAssociatedWithCaseOwner(userId);

                if (foundCases != null && foundCases.Count > 0)
                {
                 
                    getCasePatientInfo(ref foundCases);
                    cases = foundCases.OrderBy(mem => mem.lastName).ThenBy(mem => mem.firstName).ToList();
                }                
            }

            return cases;
        }

        private List<Case> getCasesAssociatedWithCaseOwner(Guid userId)
        {
            List<Case> returnCases = null;

            List<Case> foundCases = (

                                        from mem in _icmsContext.Patients

                                        join caseOwn in _icmsContext.CaseOwners
                                        on mem.member_id equals caseOwn.member_id

                                        join memEnroll in _icmsContext.MemberEnrollments
                                        on mem.member_id equals memEnroll.member_id

                                        join emply in _icmsContext.Employers
                                        on memEnroll.employer_id equals emply.employer_id

                                        join tpaEmply in _icmsContext.TpaEmployers
                                        on emply.employer_id equals tpaEmply.employer_id

                                        where caseOwn.system_user_id.Equals(userId)
                                        && mem.member_in_lcm.Equals(true)
                                        && (mem.is_test_member.Value.Equals(0) || !mem.is_test_member.HasValue)
                                        && emply.lcm_billable.Equals(true)
                                        && emply.active_flag.Equals(true)
                                        && (tpaEmply.termination_date.Equals("12/31/2199") || !tpaEmply.termination_date.HasValue)

                                        orderby mem.member_last_name, mem.member_first_name, caseOwn.assigned_date descending

                                        select new Case
                                        {
                                            memberId = mem.member_id,
                                            firstName = mem.member_first_name,
                                            lastName = mem.member_last_name,
                                            patient = new Patient
                                            {
                                                FullName = mem.member_first_name + " " + mem.member_last_name,
                                                firstName = mem.member_first_name,
                                                lastName = mem.member_last_name,
                                                dateOfBirth = mem.member_birth,
                                                dateOfBirthDisplay = (mem.member_birth.HasValue) ? mem.member_birth.Value.ToShortDateString() : "",
                                                ssn = mem.member_ssn,
                                                employerId = memEnroll.employer_id,
                                                tpaId = tpaEmply.tpa_id
                                            },
                                            caseOwnerId = caseOwn.system_user_id
                                        }
                                    )
                                    .ToList();

            if (foundCases != null)
            {

                foreach(Case cse in foundCases)
                {

                    if (currentCaseOwnerOfCase(cse))
                    {     
                        
                        if (returnCases == null)
                        {
                            returnCases = new List<Case>();
                        }

                        returnCases.Add(cse);
                    }
                }
            }

            return returnCases;
        }

        private bool currentCaseOwnerOfCase(Case potentialCase)
        {

            List<CaseOwner> caseOwn = (

                    from cseOwn in _icmsContext.CaseOwners
                    where cseOwn.member_id.Equals(potentialCase.memberId)
                    orderby cseOwn.assigned_date descending
                    select cseOwn
                )
                .ToList();

            if (caseOwn != null)
            {

                if (caseOwn[0].system_user_id.Equals(potentialCase.caseOwnerId))
                {
                    return true;
                } 
            }


            return false;
        }

        private void getCasePatientInfo(ref List<Case>foundCases)
        {

            foreach (Case patCase in foundCases)
            {

                patCase.patient.addresses = getPatientCmAddress(patCase.memberId);
                patCase.patient.homePhoneNumber = getPatientCmPhone(patCase.memberId);
                patCase.cmNotes = getPatientCurrentMonthCmNotes(patCase.memberId);
            }
        }

        private List<MemberAddress> getPatientCmAddress(Guid memberId)
        {

            List<MemberAddress> address = null;

            address = (

                    from addr in _icmsContext.MemberAddresses
                    where addr.member_id.Equals(memberId)
                    where (addr.is_alternate.Equals(false) || !addr.is_alternate.HasValue)
                    orderby addr.member_address_id descending
                    select addr
                )
                .Take(1)
                .ToList();

            return address;
        }

        private PhoneNumber getPatientCmPhone(Guid memberId)
        {
            
            PhoneNumber homePhone = null;

            PhoneNumber dayPhone = (

                    from dayPhn in _icmsContext.MemberPhoneNumbers

                    where dayPhn.member_id.Equals(memberId)
                    && dayPhn.phone_number != null
                    && dayPhn.phone_type_id.Equals(1)

                    select new PhoneNumber
                    {
                        Number = dayPhn.phone_number
                    }
                )
                .FirstOrDefault();

            PhoneNumber evenPhone = (

                    from dayPhn in _icmsContext.MemberPhoneNumbers

                    where dayPhn.member_id.Equals(memberId)
                    && dayPhn.phone_number != null
                    && dayPhn.phone_type_id.Equals(2)

                    select new PhoneNumber
                    {
                        Number = dayPhn.phone_number
                    }
                )
                .FirstOrDefault();

            PhoneNumber cell = (

                    from dayPhn in _icmsContext.MemberPhoneNumbers

                    where dayPhn.member_id.Equals(memberId)
                    && dayPhn.phone_number != null
                    && dayPhn.phone_type_id.Equals(4)

                    select new PhoneNumber
                    {
                        Number = dayPhn.phone_number
                    }
                )
                .FirstOrDefault();

            if (dayPhone != null || evenPhone != null || cell != null)
            {

                homePhone = new PhoneNumber();

                if (evenPhone != null)
                {
                    homePhone = evenPhone;
                } else if (dayPhone != null)
                {
                    homePhone = dayPhone;
                } else
                {
                    homePhone = cell;
                }
            }

            return homePhone;
        }

        private List<Note> getPatientCurrentMonthCmNotes(Guid memberId)
        {

            List<Note> cmNotes = null;

            DateTime dateNow = DateTime.Now;
            int month = dateNow.Month;
            int year = dateNow.Year;

            cmNotes = (

                    from notes in _icmsContext.MemberNotes                    
                    
                    where notes.member_id.Equals(memberId)
                    && notes.record_date.Month.Equals(month)
                    && notes.record_date.Year.Equals(year)

                    orderby notes.record_date descending
                    select new Note
                    {
                        recordDate = notes.record_date
                    }
                )
                .Distinct()
                .ToList();

            return cmNotes;
        }

        public Case getCmPatient(string patId)
        {

            Case returnCase = null;

            Patient dbPatient = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                dbPatient = (

                        from member in _icmsContext.Patients

                        join caseOwn in _icmsContext.CaseOwners
                        on member.member_id equals caseOwn.member_id into caseOwns
                        from caseOwners in caseOwns.DefaultIfEmpty()                        

                        join caseSysUsr in _icmsContext.SystemUsers
                        on caseOwners.system_user_id equals caseSysUsr.system_user_id into caseSysUsrs
                        from caseSystemUsers in caseSysUsrs.DefaultIfEmpty()

                        where member.member_id.Equals(memberId)

                        orderby caseOwners.assigned_date descending

                        select new Patient
                        {
                            PatientId = member.member_id,
                            FullName = member.member_first_name + " " + member.member_last_name,
                            firstName = member.member_first_name,
                            lastName = member.member_last_name,
                            dateOfBirth = member.member_birth,
                            dateOfBirthDisplay = (member.member_birth.HasValue) ? member.member_birth.Value.ToShortDateString() : "",
                            ssn = member.member_ssn,
                            inLcm = member.member_in_lcm,
                            caseOwnerId = caseOwners.system_user_id,
                            caseOwner = (caseSystemUsers.system_user_id != null) ? 
                                caseSystemUsers.system_user_first_name + " " + caseSystemUsers.system_user_last_name : 
                                "",
                        }
                    )
                    .FirstOrDefault();

                if (dbPatient != null)
                {

                    returnCase = new Case();
                    returnCase.patient = dbPatient;
                }
            }

            return returnCase;
        }

        public List<Patient> getCmPatientSearch(Patient srchPatients)
        {

            List<Patient> returnPatients = null;

            PatientService patSer = new PatientService(_icmsContext, _aspNetContext);
            IEnumerable<Member> patients = patSer.GetPatientsUsingFirstLastDob(srchPatients.firstName, srchPatients.lastName, null);

            if (patients != null)
            {

                List<Patient> tempPatList = new List<Patient>();

                foreach(Member pat in patients)
                {

                    Patient addPatient = new Patient();

                    addPatient.PatientId = pat.member_id;
                    addPatient.firstName = pat.member_first_name;
                    addPatient.lastName = pat.member_last_name;
                    addPatient.FullName = addPatient.firstName + " " + addPatient.lastName;
                    addPatient.dateOfBirth = pat.member_birth;
                    addPatient.dateOfBirthDisplay = (addPatient.dateOfBirth.HasValue) ? addPatient.dateOfBirth.Value.ToShortDateString() : "";
                    addPatient.ssn = pat.member_ssn;

                    tempPatList.Add(addPatient);
                }

                returnPatients = tempPatList.OrderBy(last => last.lastName).ThenBy(first => first.firstName).ThenByDescending(dob => dob.dateOfBirth).ToList();
            }

            return returnPatients;
        }

        public Case getCmPatientReports(string patId)
        {
            
            Case report = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                report = new Case();

                report.lcmReports = null;

                report.lcmReports = (

                        from lcmInit in _icmsContext.MemberLcmInitials

                        join lcmFollow in _icmsContext.MemberLcmFollowups
                        on lcmInit.lcn_case_number equals lcmFollow.lcn_case_number into lcmFollows
                        from lcmFollowup in lcmFollows.DefaultIfEmpty()

                        where lcmInit.member_id.Equals(memberId)

                        orderby lcmInit.lcm_open_date descending, lcmFollowup.lcm_followup_date descending

                        select new LcmReports
                        {
                            reportType = (lcmFollowup.lcm_followup_id > 0) ? "Comprehensive" : "Initial",
                            lcnCaseNumber = lcmInit.lcn_case_number, 
                            lcmFollowupId = (lcmFollowup.lcm_followup_id > 0) ? lcmFollowup.lcm_followup_id : 0,
                            memberId = lcmInit.member_id,
                            referralNumber = lcmInit.referral_number,
                            lcmOpenDate = (lcmFollowup.lcm_followup_id > 0) ? lcmFollowup.lcm_followup_date : lcmInit.lcm_open_date,
                            displayLcmOpenDate = (lcmFollowup.lcm_followup_id > 0) ? 
                                lcmFollowup.lcm_followup_date.ToShortDateString() : lcmInit.lcm_open_date.ToShortDateString(),
                            lcmCloseDate = (lcmFollowup.lcm_followup_id > 0) ? lcmFollowup.lcm_close_date : lcmInit.lcm_close_date,
                            displayLcmCloseDate = (lcmFollowup.lcm_followup_id > 0) ?
                                (lcmFollowup.lcm_close_date.HasValue) ? lcmFollowup.lcm_close_date.Value.ToShortDateString() : "" 
                                : (lcmInit.lcm_close_date.HasValue) ? lcmInit.lcm_close_date.Value.ToShortDateString() : "",
                            nextReportDate = (lcmFollowup.lcm_followup_id > 0) ? lcmFollowup.next_report_date : lcmInit.next_report_date,
                            displayNextReportDate = (lcmFollowup.lcm_followup_id > 0) ?
                                (lcmFollowup.next_report_date.HasValue) ? lcmFollowup.next_report_date.Value.ToShortDateString() : ""
                                : (lcmInit.next_report_date.HasValue) ? lcmInit.next_report_date.Value.ToShortDateString() : "",
                        }
                    )
                    .ToList();
            }

            return report;
        }

        public Case getCmPatientComphrehensiveReport(string patId, int compId)
        {
            Case report = null;

            if (compId > 0)
            {

                LcmReports dbRpt = (

                        from lcmFollow in _icmsContext.MemberLcmFollowups

                        join lcmInit in _icmsContext.MemberLcmInitials
                        on lcmFollow.lcn_case_number equals lcmInit.lcn_case_number

                        join memEnroll in _icmsContext.MemberEnrollments
                        on lcmInit.member_id equals memEnroll.member_id

                        join emply in _icmsContext.Employers
                        on memEnroll.employer_id equals emply.employer_id into emplys
                        from employers in emplys.DefaultIfEmpty()

                        join benfit in _icmsContext.BenefitReferrals
                        on employers.employer_id equals benfit.employer_id into benfits
                        from benefits in benfits.DefaultIfEmpty()

                        join facility in _icmsContext.rDepartments
                        on lcmFollow.facility_id equals facility.id into facilitys 
                        from facilities in facilitys.DefaultIfEmpty()

                        where lcmFollow.lcm_followup_id.Equals(compId)
                        select new LcmReports
                        {

                            referralNumber = lcmInit.referral_number,
                            authNumber = lcmInit.auth_number,
                            procedure = lcmFollow.Procedure,
                            primaryDiagnosis = lcmFollow.primary_dx,
                            secondaryDiagnosis = lcmFollow.secondary_dx,
                            otherDiagnosis = lcmFollow.other_dx,
                            cancerRelated = lcmFollow.cancer_related,
                            hospitalized = lcmFollow.hospitalized,
                            hospitalFiveDays = lcmFollow.hospital_five_days,
                            senttoadmin = (bool)lcmInit.senttoadmin,
                            senttorein = (bool)lcmInit.senttorein,
                            stopLoss = benefits.benefit_stoploss,


                            lcmFollowupId = lcmFollow.lcm_followup_id,
                            lcnCaseNumber = lcmFollow.lcn_case_number,

                            lcmOpenDate = lcmInit.lcm_open_date,
                            displayLcmOpenDate = (lcmInit.lcm_open_date != null) ?
                                lcmInit.lcm_open_date.ToShortDateString() + " " + lcmInit.lcm_open_date.ToShortTimeString() :
                                "",

                            lcmFinalizedDate = lcmFollow.lcm_followup_date,
                            displayLcmFinalizedDate = (lcmFollow.lcm_followup_date != null) ?
                                lcmFollow.lcm_followup_date.ToShortDateString() + " " + lcmFollow.lcm_followup_date.ToShortTimeString() :
                                "",

                            nextReportDate = lcmFollow.next_report_date,
                            displayNextReportDate = (lcmFollow.next_report_date.HasValue) ?
                                lcmFollow.next_report_date.Value.ToShortDateString() + " " + lcmFollow.next_report_date.Value.ToShortTimeString() :
                                "",

                            lcmCloseDate = lcmFollow.lcm_close_date,
                            displayLcmCloseDate = (lcmFollow.lcm_close_date.HasValue) ?
                                lcmFollow.lcm_close_date.Value.ToShortDateString() + " " + lcmFollow.lcm_close_date.Value.ToShortTimeString() :
                                "",

                            facilityId = lcmFollow.facility_id,
                            facilityName = facilities.label,
                            facilityType = lcmFollow.facility_type,
                            staging = lcmFollow.staging,
                            stagingStatus = lcmFollow.staging_status,
                            acuity = lcmFollow.acuity,
                            acuityChanged = lcmFollow.acuity_changed,
                            acuityDate = lcmFollow.acuity_date,
                            completedByUser = lcmFollow.system_user_id,

                            prognosis = lcmFollow.prognosis_report,
                            planOfCare = lcmFollow.plan_of_care,
                        }
                    )
                    .FirstOrDefault();

                if (dbRpt != null)
                {

                    report = new Case();
                    report.lcmReports = new List<LcmReports>();

                    dbRpt.codes = getLcmReportCodes(patId, compId, dbRpt.lcnCaseNumber, dbRpt.referralNumber);
                    dbRpt.savings = getLcmReportSavings(compId, dbRpt.referralNumber);
                    dbRpt.reportNotes = getLcmReportNotes(compId);

                    report.lcmReports.Add(dbRpt);
                }
            }

            return report;
        }

        private List<MedicalCode> getLcmReportCodes(string patId, int compId, int lcnCaseNumber, string referralNumber)
        {

            List<MedicalCode> reportCodes = null;

            List<MedicalCode> icd10s = getLcmReportDiagnosisCodes(patId, compId, lcnCaseNumber, referralNumber);
            List<MedicalCode> cpts = getLcmReportCptCodes(patId, compId, lcnCaseNumber, referralNumber);
            List<MedicalCode> hcpcs = getLcmReportHcpcsCodes(patId, compId, lcnCaseNumber, referralNumber);

            if ((icd10s != null && icd10s.Count > 0) ||
                (cpts != null && cpts.Count > 0) ||
                (hcpcs != null && hcpcs.Count > 0))
            {

                reportCodes = new List<MedicalCode>();

                if (icd10s != null && icd10s.Count > 0)
                {

                    foreach(MedicalCode icd in icd10s)
                    {
                        reportCodes.Add(icd);
                    }
                }

                if (cpts != null && cpts.Count > 0)
                {

                    foreach(MedicalCode cpt in cpts)
                    {
                        reportCodes.Add(cpt);
                    }
                }

                if (hcpcs != null && hcpcs.Count > 0)
                {

                    foreach (MedicalCode hcp in hcpcs)
                    {
                        reportCodes.Add(hcp);
                    }
                }
            }

            return reportCodes;
        }
        private List<MedicalCode> getLcmReportDiagnosisCodes(string patId, int compId, int lcnCaseNumber, string referralNumber)
        {

            List<MedicalCode> reportCodes = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {

                reportCodes = (

                        from refDiags in _icmsContext.MemberReferralDiags

                        join diags in _icmsContext.DiagnosisCodes10
                        on refDiags.diagnosis_or_procedure_code equals diags.diagnosis_code into diagCode
                        from diagCodes in diagCode.DefaultIfEmpty()

                        join diagOld in _icmsContext.DiagnosisCodes
                        on refDiags.diagnosis_or_procedure_code equals diagOld.Diagnosis_Code into oldDiagCode
                        from oldDiagCodes in oldDiagCode.DefaultIfEmpty()

                        orderby refDiags.id descending
                        where refDiags.referral_number.Equals(referralNumber)

                        select new MedicalCode
                        {

                            CodeId = refDiags.id,
                            CodeType = "ICD10",
                            Code = refDiags.diagnosis_or_procedure_code,
                            DisplayDescription = (diagCodes.medium_description != null) ? diagCodes.medium_description : oldDiagCodes.Diagnosis_Descr,
                            referralCode = true
                        }
                    )
                    .Take(15)
                    .ToList();
            } 
            else if (lcnCaseNumber > 0)
            {

                MemberLcmInitial dbInit = (

                        from rptInit in _icmsContext.MemberLcmInitials
                        where rptInit.lcn_case_number.Equals(lcnCaseNumber)
                        select rptInit
                    )
                    .FirstOrDefault();

                if (dbInit != null)
                {

                    if (!string.IsNullOrEmpty(dbInit.primary_diagnosis) || 
                        !string.IsNullOrEmpty(dbInit.secondary_diagnosis) ||
                        !string.IsNullOrEmpty(dbInit.other_diagnosis))
                    {
                        
                        reportCodes = new List<MedicalCode>();

                        if(!string.IsNullOrEmpty(dbInit.primary_diagnosis))
                        {

                            string primaryDesc = (

                                    from primDiag in _icmsContext.DiagnosisCodes10
                                    where primDiag.diagnosis_code.Equals(dbInit.primary_diagnosis)
                                    select primDiag.medium_description
                                )
                                .Take(1)
                                .FirstOrDefault();

                            MedicalCode primary = new MedicalCode();

                            primary.CodeId = 1;
                            primary.CodeType = "ICD10";
                            primary.Code = dbInit.primary_diagnosis;
                            primary.DisplayDescription = (!string.IsNullOrEmpty(primaryDesc)) ? primaryDesc : "";

                            reportCodes.Add(primary);
                        }

                        if (!string.IsNullOrEmpty(dbInit.secondary_diagnosis))
                        {

                            string secondaryDesc = (

                                    from secondDiag in _icmsContext.DiagnosisCodes10
                                    where secondDiag.diagnosis_code.Equals(dbInit.secondary_diagnosis)
                                    select secondDiag.medium_description
                                )
                                .Take(1)
                                .FirstOrDefault();

                            MedicalCode secondary = new MedicalCode();

                            secondary.CodeId = 2;
                            secondary.CodeType = "ICD10";
                            secondary.Code = dbInit.secondary_diagnosis;
                            secondary.DisplayDescription = (!string.IsNullOrEmpty(secondaryDesc)) ? secondaryDesc : "";

                            reportCodes.Add(secondary);
                        }

                        if (!string.IsNullOrEmpty(dbInit.other_diagnosis))
                        {

                            string otherDesc = (

                                    from othrDiag in _icmsContext.DiagnosisCodes10
                                    where othrDiag.diagnosis_code.Equals(dbInit.secondary_diagnosis)
                                    select othrDiag.medium_description
                                )
                                .Take(1)
                                .FirstOrDefault();

                            MedicalCode other = new MedicalCode();

                            other.CodeId = 3;
                            other.CodeType = "ICD10";
                            other.Code = dbInit.other_diagnosis;
                            other.DisplayDescription = (!string.IsNullOrEmpty(otherDesc)) ? otherDesc : "";

                            reportCodes.Add(other);
                        }
                    }

                }
            }

            return reportCodes;
        }
        private List<MedicalCode> getLcmReportCptCodes(string patId, int compId, int lcnCaseNumber, string referralNumber)
        {

            List<MedicalCode> reportCodes = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {

                reportCodes = (

                        from refCpts in _icmsContext.MemberReferralCpts

                        join cpts in _icmsContext.CptCodes
                        on refCpts.cpt_code equals cpts.cpt_code into cptCode
                        from cptCodes in cptCode.DefaultIfEmpty()

                        orderby refCpts.id descending
                        where refCpts.referral_number.Equals(referralNumber)

                        select new MedicalCode
                        {

                            CodeId = refCpts.id,
                            CodeType = "CPT",
                            Code = cptCodes.cpt_code,
                            DisplayDescription = cptCodes.cpt_descr,
                            referralCode = true
                        }
                    )
                    .Take(15)
                    .ToList();
            }
            else if (lcnCaseNumber > 0)
            {

                MemberLcmInitial dbInit = (

                        from rptInit in _icmsContext.MemberLcmInitials
                        where rptInit.lcn_case_number.Equals(lcnCaseNumber)
                        select rptInit
                    )
                    .FirstOrDefault();

                if (dbInit != null)
                {

                    if (!string.IsNullOrEmpty(dbInit.procedure))
                    {

                        string procedureDesc = (

                                from procDesc in _icmsContext.CptCodes
                                where procDesc.cpt_code.Equals(dbInit.procedure)
                                select procDesc.cpt_descr
                            )
                            .Take(1)
                            .FirstOrDefault();

                        reportCodes = new List<MedicalCode>();
                        MedicalCode proc = new MedicalCode();

                        if (!string.IsNullOrEmpty(procedureDesc))
                        {

                            proc.CodeId = 1;
                            proc.CodeType = "CPT";
                            proc.Code = dbInit.procedure;
                            proc.DisplayDescription = (!string.IsNullOrEmpty(procedureDesc)) ? procedureDesc : "";

                            reportCodes.Add(proc);

                        } 
                    }
                }
            }

            return reportCodes;
        }
        private List<MedicalCode> getLcmReportHcpcsCodes(string patId, int compId, int lcnCaseNumber, string referralNumber)
        {

            List<MedicalCode> reportCodes = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {

                reportCodes = (

                        from refHcpcs in _icmsContext.MemberReferralHcpcss

                        join hcpcs in _icmsContext.HcpcsCodes
                        on refHcpcs.hcpcs_code equals hcpcs.hcp_code into hcpcCode
                        from hcpcCodes in hcpcCode.DefaultIfEmpty()

                        orderby refHcpcs.id descending
                        where refHcpcs.referral_number.Equals(referralNumber)

                        select new MedicalCode
                        {

                            CodeId = refHcpcs.id,
                            CodeType = "HCPCS",
                            Code = hcpcCodes.hcp_code,
                            DisplayDescription = hcpcCodes.hcpcs_full,
                            referralCode = true
                        }
                    )
                    .Take(15)
                    .ToList();
            }
            else if (lcnCaseNumber > 0)
            {

                MemberLcmInitial dbInit = (

                        from rptInit in _icmsContext.MemberLcmInitials
                        where rptInit.lcn_case_number.Equals(lcnCaseNumber)
                        select rptInit
                    )
                    .FirstOrDefault();

                if (dbInit != null)
                {

                    if (!string.IsNullOrEmpty(dbInit.procedure))
                    {

                        string procedureDesc = (

                                from procDesc in _icmsContext.HcpcsCodes
                                where procDesc.hcp_code.Equals(dbInit.procedure)
                                select procDesc.hcpcs_full
                            )
                            .Take(1)
                            .FirstOrDefault();

                        reportCodes = new List<MedicalCode>();
                        MedicalCode proc = new MedicalCode();

                        if (!string.IsNullOrEmpty(procedureDesc))
                        {

                            proc.CodeId = 1;
                            proc.CodeType = "HCPCS";
                            proc.Code = dbInit.procedure;
                            proc.DisplayDescription = (!string.IsNullOrEmpty(procedureDesc)) ? procedureDesc : "";

                            reportCodes.Add(proc);

                        }
                        else
                        {

                            proc.CodeId = 1;
                            proc.CodeType = "CPT";
                            proc.Code = dbInit.procedure;

                            reportCodes.Add(proc);
                        }
                    }

                }
            }

            return reportCodes;
        }


        private List<Saving> getLcmReportSavings(int compId, string referralNumber)
        {

            List<Saving> reportSavings = null;
            
            List<Saving> refSavings = null;
            List<Saving> rptSavings = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {
                refSavings = getReferralSavings(referralNumber);

                if (refSavings != null)
                {

                    reportSavings = new List<Saving>();

                    foreach(Saving referralSave in refSavings)
                    {
                        reportSavings.Add(referralSave);
                    }
                }
            }

            if (compId > 0)
            {

                rptSavings = (

                        from folSave in _icmsContext.MemberLcmFollowupSavingss
                        where folSave.lcm_followup_id.Equals(compId)
                        select new Saving
                        {

                            savingsType = "Report",
                            savingsId = folSave.member_lcm_followup_savings_id,
                            itemDescription = folSave.description,
                            savings = folSave.amount,
                        }
                    )
                    .ToList();

                if (rptSavings != null)
                {

                    if (reportSavings == null)
                    {
                        reportSavings = new List<Saving>();
                    }

                    foreach(Saving cmReportSaving in rptSavings)
                    {
                        reportSavings.Add(cmReportSaving);
                    }
                }
            }

            
            return reportSavings;
        }
        private List<Saving> getReferralSavings(string referralNumber)
        {

            List<Saving> refSavings = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {

                refSavings = (

                        from refSave in _icmsContext.rUtilizationSavingses

                        join saveUnit in _icmsContext.rSavingsUnits
                        on refSave.saving_units_id equals saveUnit.saving_units_id into saveUnits
                        from savingsUnit in saveUnits.DefaultIfEmpty()

                        where refSave.referral_number.Equals(referralNumber)
                        select new Saving
                        {

                            savingsType = "Referral",
                            savingsId = refSave.r_utilization_savings_id,
                            itemDescription = refSave.item_description,
                            quantity = refSave.quantity,
                            savingUnits = savingsUnit.units_label,
                            cost = refSave.cost,
                            negotiated = refSave.negotiated,
                            savings = refSave.cost - refSave.negotiated,
                        }
                    )
                    .ToList();
            }

            return refSavings;
        }

        public Note getLcmReportNotes(int compId)
        {
            
            Note lcmNotes = null;

            lcmNotes = (

                    from followup in _icmsContext.MemberLcmFollowups

                    join followNotes in _icmsContext.MemberLcmFollowupNoteses
                    on followup.lcm_followup_id equals followNotes.lcm_followup_id

                    where followNotes.lcm_followup_id.Equals(compId)
                    && followNotes.current_note > 0

                    orderby followNotes.creation_date descending

                    select new Note
                    {
                        currentTreatment = followNotes.current_treatments,
                        futureTreatment = followNotes.future_treatments,
                        psychoSocialSummary = followNotes.psycho_social_summary,
                        nurseSummary = followNotes.nurse_summary,
                        physicianPrognosis = followNotes.physician_prognosis,
                        previousTreatment = followNotes.previous_treatments,
                        newlyIdentified = followNotes.newly_identified_cm
                    }
                )
                .Take(1)
                .FirstOrDefault();            

            return lcmNotes;
        }

        public List<Bill> getCmPatientBills(string patId)
        {

            List<Bill> bills = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                List<Bill> dbBills = (

                        from billing in _dataStagingContext.LcmBillingWorktables
                        where billing.member_id.Equals(memberId)
                        orderby billing.record_date descending
                        select new Bill
                        {

                            billId = billing.lcm_record_id,
                            lcmInvoiceNumber = billing.LCM_Invoice_Number,
                            patientFullName = billing.patient,
                            caseOwner = billing.case_manager,
                            recordDate = billing.record_date,
                            displayRecordDate = (billing.record_date.HasValue ) ? 
                                billing.record_date.Value.ToShortDateString() + " " + billing.record_date.Value.ToShortTimeString() : 
                                "",
                            billNote = billing.notes,
                            billMinutes = billing.time_length,
                            billCode = billing.time_code,
                            employerBillingRate = (decimal)billing.lcm_rate
                        }
                    )
                    .ToList();

                if (dbBills != null)
                {

                    bills = new List<Bill>();

                    foreach (Bill addBill in dbBills)
                    {

                        if (addBill.billCode > 0)
                        {

                            BillingCodes billCodeDescription = getLcmBillCodeDescription((int)addBill.billCode);

                            addBill.billCodeDescription = billCodeDescription.billingDescription;
                            addBill.billingCode = billCodeDescription.billingCode;
                        }

                        addBill.billTotalAmount = Convert.ToDecimal(((decimal)addBill.billMinutes / 60) * addBill.employerBillingRate);
                        addBill.displayBillTotalAmount = addBill.billTotalAmount.Value.ToString("$0.00");


                        bills.Add(addBill);
                    }
                }
            }

            return bills;
        }

        public List<Bill> getCmPatientHistoricalBills(string patId)
        {

            List<Bill> bills = null;

            try
            {

                Guid memberId = Guid.Empty;

                if (Guid.TryParse(patId, out memberId))
                {

                    List<Bill> dbBills = (

                            from billing in _dataStagingContext.BillingBackups
                            where billing.member_id.Equals(memberId)
                            && billing.billing_type.Equals("LCM")
                            orderby billing.record_date descending
                            select new Bill
                            {

                                billingBackupId = billing.billing_backup_id,
                                billId = (billing.lcm_record_id.HasValue) ? (Guid)billing.lcm_record_id : Guid.Empty, 
                                memberId = billing.member_id,
                                lcmInvoiceNumber = billing.LCM_Invoice_Number,
                                patientFullName = billing.patient,
                                caseOwner = !string.IsNullOrEmpty(billing.case_manager) ? billing.case_manager : "",
                                recordDate = billing.record_date,
                                displayRecordDate = (billing.record_date.HasValue) ?
                                    billing.record_date.Value.ToShortDateString() + " " + billing.record_date.Value.ToShortTimeString() :
                                    "",
                                billNote = billing.notes,
                                billMinutes = billing.time_length,
                                billCode = billing.time_code,
                                employerBillingRate = (billing.lcm_rate.HasValue) ? (decimal)billing.lcm_rate : 0
                            }
                        )
                        .ToList();

                    if (dbBills != null)
                    {

                        bills = new List<Bill>();

                        foreach (Bill addBill in dbBills)
                        {

                            if (addBill.billCode > 0)
                            {

                                BillingCodes billCodeDescription = getLcmBillCodeDescription((int)addBill.billCode);

                                if (billCodeDescription != null)
                                {

                                    addBill.billCodeDescription = billCodeDescription.billingDescription;
                                    addBill.billingCode = billCodeDescription.billingCode;
                                }
                            }



                            double minutes = (addBill.billMinutes.HasValue) ? (double)addBill.billMinutes : 0;
                            decimal employerRate = ((decimal)addBill.employerBillingRate > 0) ? (decimal)addBill.employerBillingRate : getMemberEmployerRate((Guid)addBill.memberId);

                            addBill.billTotalAmount = Convert.ToDecimal(((decimal)minutes / 60) * employerRate);
                            addBill.displayBillTotalAmount = addBill.billTotalAmount.Value.ToString("$0.00");


                            bills.Add(addBill);
                        }
                    }
                }

                return bills;
            }
            catch(Exception ex)
            {
                return bills;
            }
        }

        private decimal getMemberEmployerRate(Guid memberId)
        {

            decimal employerRate = 0;

            employerRate = (

                    from memEnroll in _icmsContext.MemberEnrollments

                    join emply in _icmsContext.Employers
                    on memEnroll.employer_id equals emply.employer_id

                    where memEnroll.member_id.Equals(memberId)

                    select emply.lcm_billing_rate
                )
                .FirstOrDefault();

            if (employerRate == 0)
            {
                employerRate = 100;
            }

            return employerRate;
        }

        public Bill getCmPatientBill(string patId, string billId)
        {

            Bill bill = null;

            Guid lcmInvoiceId = Guid.Empty;

            if (Guid.TryParse(billId, out lcmInvoiceId))
            {

                bill = (

                        from billing in _dataStagingContext.LcmBillingWorktables
                        where billing.lcm_record_id.Equals(lcmInvoiceId)
                        select new Bill
                        {

                            billId = billing.lcm_record_id,
                            lcmInvoiceNumber = billing.LCM_Invoice_Number,
                            patientFullName = billing.patient,
                            caseOwner = billing.case_manager,
                            recordDate = billing.record_date,
                            displayRecordDate = (billing.record_date.HasValue) ?
                                billing.record_date.Value.ToShortDateString() + " " + billing.record_date.Value.ToShortTimeString() :
                                "",
                            billNote = billing.notes,
                            billMinutes = billing.time_length,
                            billCode = billing.time_code,
                            employerBillingRate = (decimal)billing.lcm_rate
                        }
                    )
                    .FirstOrDefault();

                if (bill != null)
                {

                    bill.billTotalAmount = Convert.ToDecimal(((decimal)bill.billMinutes / 60) * bill.employerBillingRate);
                    bill.displayBillTotalAmount = bill.billTotalAmount.Value.ToString("$0.00");
                }
            }

            return bill;
        }

        private BillingCodes getLcmBillCodeDescription(int billCode)
        {

            BillingCodes billingCode = null;

            billingCode = (

                    from lcmCodes in _icmsContext.LcmBillingCodes
                    where lcmCodes.billing_id.Equals(billCode)
                    select new BillingCodes
                    {
                        billingCodeId = lcmCodes.billing_id,
                        billingCode = lcmCodes.billing_code,
                        billingDescription = lcmCodes.billing_description
                    }
                )
                .FirstOrDefault();

            return billingCode;
        }


        public Note getComprehensiveReportHistoricalNote(Note lcmNote)
        {

            Note rptNote = null;

            try
            {

                if (lcmNote.lcnCaseNumber > 0 && lcmNote.lcmFollowupId > 0 && !string.IsNullOrEmpty(lcmNote.lcmNoteType))
                {

                    Note dbNote = null;

                    switch (lcmNote.lcmNoteType)
                    {

                        case "current medical status":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.current_treatments != null
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.current_treatments
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "case history":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.previous_treatments != null
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.previous_treatments
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "medications":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.future_treatments != null 
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.future_treatments
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "newly identified cm":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.newly_identified_cm_summary != null 
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.newly_identified_cm_summary
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "service authorized":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.nurse_summary != null 
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.nurse_summary
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "family dynamics":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.psycho_social_summary != null
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.psycho_social_summary
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                        case "recommendations":

                            dbNote = (

                                    from lcmNtes in _icmsContext.MemberLcmFollowups
                                    where lcmNtes.lcn_case_number.Equals(lcmNote.lcnCaseNumber)
                                    && !lcmNtes.lcm_followup_id.Equals(lcmNote.lcmFollowupId)
                                    && lcmNtes.physician_prognosis != null 
                                    orderby lcmNtes.lcm_followup_id descending
                                    select new Note
                                    {
                                        noteText = lcmNtes.physician_prognosis
                                    }
                                )
                                .Take(1)
                                .FirstOrDefault();

                            break;

                    }

                    if (dbNote != null)
                    {
                        rptNote = new Note();
                        rptNote.noteText = dbNote.noteText;
                    }
                }

                return rptNote;
            }
            catch(Exception ex)
            {
                return rptNote;
            }
        }

        public Note getComprehensiveReportNote(string noteType, string followupId)
        {

            Note rptNote = null;

            try
            {

                int memberLcmFollowupId = 0;

                if (int.TryParse(followupId, out memberLcmFollowupId) 
                    && !string.IsNullOrEmpty(noteType))
                {                    

                    Note lcmNotes = (

                            from followup in _icmsContext.MemberLcmFollowups

                            join followNotes in _icmsContext.MemberLcmFollowupNoteses
                            on followup.lcm_followup_id equals followNotes.lcm_followup_id

                            where followNotes.lcm_followup_id.Equals(memberLcmFollowupId)
                            && followNotes.current_note > 0

                            orderby followNotes.creation_date descending

                            select new Note
                            {
                                currentTreatment = followNotes.current_treatments,
                                futureTreatment = followNotes.future_treatments,
                                psychoSocialSummary = followNotes.psycho_social_summary,
                                nurseSummary = followNotes.nurse_summary,
                                physicianPrognosis = followNotes.physician_prognosis,
                                previousTreatment = followNotes.previous_treatments,
                                newlyIdentified = followNotes.newly_identified_cm
                            }
                        )
                        .Take(1)
                        .FirstOrDefault();

                    if (lcmNotes != null)
                    {

                        Note dbNote = new Note();

                        switch (noteType)
                        {

                            case "current medical status":
                                dbNote.noteText = lcmNotes.currentTreatment;
                                break;

                            case "case history":
                                dbNote.noteText = lcmNotes.previousTreatment;                                       
                                break;

                            case "medications":
                                dbNote.noteText = lcmNotes.futureTreatment;                                        
                                break;

                            case "newly identified cm":
                                dbNote.noteText = lcmNotes.newlyIdentified;
                                break;

                            case "service authorized":
                                dbNote.noteText = lcmNotes.nurseSummary;                                        
                                break;

                            case "family dynamics":
                                dbNote.noteText = lcmNotes.psychoSocialSummary;
                                break;

                            case "recommendations":
                                dbNote.noteText = lcmNotes.physicianPrognosis;
                                break;

                        }

                        if (dbNote != null)
                        {
                            rptNote = new Note();
                            rptNote.noteText = dbNote.noteText;
                        }
                    }
                }

                return rptNote;
            }
            catch (Exception ex)
            {
                return rptNote;
            }
        }



        public List<Case> flipCmCaseReports(Case report)
        {

            List<Case> caseOwnerCases = null;

            if (report.lcmReports != null && report.lcmReports.Count > 0)
            {

                foreach (LcmReports rpt in report.lcmReports)
                {

                    if (!rpt.memberId.Equals(Guid.Empty))
                    {

                        rpt.lcnCaseNumber = getCmCaseLcnCaseNumber((Guid)rpt.memberId);

                        if (rpt.lcnCaseNumber > 0)
                        {
                            rpt.lcmFollowupId = getLcmFollowupId(rpt.lcnCaseNumber);
                        }

                        rpt.lcmOpenDate = report.lcmOpenDate;
                        rpt.lcmCloseDate = report.lcmCloseDate;
                        rpt.useHistoricalNotes = report.useHistoricalNotes;
                        rpt.usr = report.usr;

                        if (rpt.lcmFollowupId > 0)
                        {
                            LcmReports finalizedRpt = rpt;

                            if (finalizeComprehensiveReport(ref finalizedRpt))
                            {
                                caseOwnerCases = getCaseOwnerCases(report.usr.ToString());
                            }
                        } 
                        else if (rpt.lcnCaseNumber > 0)
                        {

                            if (finalizeInitialReport(rpt))
                            {
                                caseOwnerCases = getCaseOwnerCases(report.usr.ToString());
                            }
                        }
                    }
                }
            }

            return caseOwnerCases;
        }

        private int getCmCaseLcnCaseNumber(Guid memberId)
        {
            
            int lcnCaseNumber = 0;

            lcnCaseNumber = (

                    from initial in _icmsContext.MemberLcmInitials
                    where initial.member_id.Equals(memberId)
                    orderby initial.lcn_case_number descending
                    select initial.lcn_case_number
                )
                .Take(1)
                .FirstOrDefault();

            return lcnCaseNumber;
        }

        private int getLcmFollowupId(int lcnCaseNumber)
        {

            int lcmFollowupId = 0;

            lcmFollowupId = (

                    from followup in _icmsContext.MemberLcmFollowups
                    where followup.lcn_case_number.Equals(lcnCaseNumber)
                    orderby followup.lcm_followup_id descending
                    select followup.lcm_followup_id
                )
                .Take(1)
                .FirstOrDefault();

            return lcmFollowupId;
        }

        private bool finalizeInitialReport(LcmReports report)
        {

            if (closeInitialReport(report))
            {

                if (createNewComprehensiveReport(ref report))
                {
                    return true;
                }
            }

            return false;
        }

        private bool finalizeComprehensiveReport(ref LcmReports report)
        {

            if (closeComprehensiveReport(report))
            {

                if (createNewComprehensiveReport(ref report))
                {
                    return true;
                }
            }           

            return false;
        }

        private bool closeInitialReport(LcmReports report)
        {

            MemberLcmInitial dbInitial = (

                    from memInitial in _icmsContext.MemberLcmInitials
                    where memInitial.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memInitial
                )
                .FirstOrDefault();

            if (dbInitial != null && !report.lcmCloseDate.Equals(DateTime.MinValue))
            {

                dbInitial.lcm_close_date = report.lcmCloseDate;
                dbInitial.last_update_date = DateTime.Now;

                if (!report.usr.Equals(Guid.Empty))
                {
                    dbInitial.last_update_user_id = report.usr;
                }

                _icmsContext.MemberLcmInitials.Update(dbInitial);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool closeComprehensiveReport(LcmReports report)
        {

            MemberLcmFollowup dbFollowUp = (

                    from memFollow in _icmsContext.MemberLcmFollowups
                    where memFollow.lcm_followup_id.Equals(report.lcmFollowupId)
                    select memFollow
                )
                .FirstOrDefault();

            if (dbFollowUp != null && !report.lcmCloseDate.Equals(DateTime.MinValue))
            {

                dbFollowUp.lcm_close_date = report.lcmCloseDate;
                dbFollowUp.last_update_date = DateTime.Now;

                if (!report.usr.Equals(Guid.Empty))
                {
                    dbFollowUp.last_update_user_id = report.usr;
                }

                _icmsContext.MemberLcmFollowups.Update(dbFollowUp);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool createNewComprehensiveReport(ref LcmReports report)
        {

            int newFollowUpId = insertComprehensiveReport(report);

            if (newFollowUpId > 0)
            {

                report.newFollowupId = newFollowUpId;

                updateBillableNotesWithReport(report, newFollowUpId);

                if (report.useHistoricalNotes)
                {
                    updateNewComprehensiveReportWithHistoricalNotes(report, newFollowUpId);
                }
                else
                {
                    updateNewComprehensiveReportWithBillableNotes(report, newFollowUpId);
                }

                return true;
            }

            return false;
        }

        private int insertComprehensiveReport(LcmReports report)
        {

            MemberLcmFollowup newFollowUp = new MemberLcmFollowup();

            newFollowUp.lcn_case_number = report.lcnCaseNumber;
            newFollowUp.lcm_followup_date = (!report.lcmOpenDate.Equals(DateTime.MinValue)) ? (DateTime)report.lcmOpenDate : DateTime.Now;
            newFollowUp.in_qa = 0;
            newFollowUp.document_approved = 0;

            _icmsContext.MemberLcmFollowups.Add(newFollowUp);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return newFollowUp.lcm_followup_id;
            }

            return 0;
        }

        private void updateBillableNotesWithReport(LcmReports report, int newFollowUpId)
        {

            updateBillableMemberNotesWithReport(report, newFollowUpId);
            updateBillableUtilizationNotesWithReport(report, newFollowUpId);
        }

        private void updateBillableMemberNotesWithReport(LcmReports report, int newFollowUpId)
        {
            try
            {
                List<MemberNotes> memNotes = (

                        from memberNotes in _icmsContext.MemberNotes
                        where memberNotes.member_id.Equals(report.memberId)
                        && (!memberNotes.note_billed.HasValue || memberNotes.note_billed.Equals(0))
                        && (!memberNotes.date_lcm_report_generated.HasValue)
                        && memberNotes.billing_id > 0
                        select memberNotes
                    )
                    .ToList();

                if (memNotes != null && memNotes.Count > 0)
                {

                    DateTime dteNow = DateTime.Now;

                    foreach (MemberNotes note in memNotes)
                    {

                        note.date_lcm_report_generated = dteNow;
                        note.lcn_case_number = report.lcnCaseNumber;
                        note.lcm_followup_id = newFollowUpId;

                        _icmsContext.MemberNotes.Update(note);
                    }

                    _icmsContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void updateBillableUtilizationNotesWithReport(LcmReports report, int newFollowUpId)
        {
            try
            {
                List<rUtilizationDaysNotes> utilNotes = (

                        from memberNotes in _icmsContext.rUtilizationDaysNoteses
                        where memberNotes.member_id.Equals(report.memberId)
                        && (!memberNotes.note_billed.HasValue || memberNotes.note_billed.Equals(0))
                        && (!memberNotes.date_lcm_report_generated.HasValue)
                        && memberNotes.billing_id > 0
                        select memberNotes
                    )
                    .ToList();

                if (utilNotes != null && utilNotes.Count > 0)
                {

                    DateTime dteNow = DateTime.Now;

                    foreach (rUtilizationDaysNotes note in utilNotes)
                    {

                        note.date_lcm_report_generated = dteNow;
                        note.lcn_case_number = report.lcnCaseNumber;
                        note.lcm_followup_id = newFollowUpId;

                        _icmsContext.rUtilizationDaysNoteses.Update(note);
                    }

                    _icmsContext.SaveChanges();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void updateNewComprehensiveReportWithHistoricalNotes(LcmReports report, int newFollowUpId)
        {

            LcmReports rptNotes = new LcmReports();

            MemberLcmFollowup dbHistoricalFollowUp = (

                    from memFollowUp in _icmsContext.MemberLcmFollowups
                    where memFollowUp.lcn_case_number.Equals(report.lcnCaseNumber)
                    && memFollowUp.lcm_followup_id.Equals(report.lcmFollowupId)
                    select memFollowUp
                )
                .FirstOrDefault();

            if (dbHistoricalFollowUp != null)
            {

                rptNotes.previousTreatments = dbHistoricalFollowUp.previous_treatments;
                rptNotes.futureTreatments = dbHistoricalFollowUp.future_treatments;
                rptNotes.psychoSocialSummary = dbHistoricalFollowUp.psycho_social_summary;

                MemberLcmFollowup dbNewFollowUp = (

                        from newFollowUp in _icmsContext.MemberLcmFollowups
                        where newFollowUp.lcm_followup_id.Equals(newFollowUpId)
                        select newFollowUp
                    )
                    .FirstOrDefault();

                if (dbNewFollowUp != null)
                {

                    dbNewFollowUp.previous_treatments = rptNotes.previousTreatments;
                    dbNewFollowUp.future_treatments = rptNotes.futureTreatments;
                    dbNewFollowUp.psycho_social_summary = rptNotes.psychoSocialSummary;

                    _icmsContext.MemberLcmFollowups.Update(dbNewFollowUp);
                    _icmsContext.SaveChanges();
                }
            }                
        }

        private void updateNewComprehensiveReportWithBillableNotes(LcmReports report, int newFollowUpId)
        {

        }



        public Case addCmCaseReportReferral(Case report)
        {

            Case cmReport = null;

            MemberLcmInitial dbInit = (

                    from memLcmInit in _icmsContext.MemberLcmInitials
                    where memLcmInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memLcmInit
                )
                .FirstOrDefault();

            if (dbInit != null)
            {

                dbInit.referral_number = report.referralNumber;

                _icmsContext.MemberLcmInitials.Update(dbInit);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    switch (report.reportType)
                    {

                        case "Comprehensive":
                            cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                            break;

                        case "Initial":
                            break;
                    }
                }
            }

            return cmReport;
        }
        public Case removeCmCaseReportReferral(Case report)
        {

            Case cmReport = null;

            MemberLcmInitial dbInit = (

                    from memLcmInit in _icmsContext.MemberLcmInitials
                    where memLcmInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memLcmInit
                )
                .FirstOrDefault();

            if (dbInit != null)
            {

                dbInit.referral_number = "";

                _icmsContext.MemberLcmInitials.Update(dbInit);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    switch(report.reportType)
                    {

                        case "Comprehensive":                    
                            cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                            break;

                        case "Initial":
                            break;
                    }
                }
            }

            return cmReport;
        }

        public Case addCmCaseReportFacility(LcmReports report)
        {

            Case cmReport = null;

            switch (report.reportType)
            {

                case "Comprehensive":

                    MemberLcmFollowup dbFoll = (

                            from memFol in _icmsContext.MemberLcmFollowups
                            where memFol.lcm_followup_id.Equals(report.lcmFollowupId)
                            select memFol
                        )
                        .FirstOrDefault();

                    if (dbFoll != null)
                    {

                        dbFoll.facility_id = report.facilityId;

                        _icmsContext.MemberLcmFollowups.Update(dbFoll);
                        int resultFoll = _icmsContext.SaveChanges();

                        if (resultFoll > 0)
                        {                    
                            cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                        }
                    }                        

                    break;

                case "Initial":

                    MemberLcmInitial dbInit = (

                            from memLcmInit in _icmsContext.MemberLcmInitials
                            where memLcmInit.lcn_case_number.Equals(report.lcnCaseNumber)
                            select memLcmInit
                        )
                        .FirstOrDefault();

                    if (dbInit != null)
                    {

                        dbInit.facility_id = (int)report.facilityId;

                        _icmsContext.MemberLcmInitials.Update(dbInit);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {
                            //cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                        }
                    }

                    break;
            }






            return cmReport;
        }
        public Case removeCmCaseReportFacility(LcmReports report)
        {

            Case cmReport = null;

            switch (report.reportType)
            {

                case "Comprehensive":

                    MemberLcmFollowup dbFoll = (

                            from memFol in _icmsContext.MemberLcmFollowups
                            where memFol.lcm_followup_id.Equals(report.lcmFollowupId)
                            select memFol
                        )
                        .FirstOrDefault();

                    if (dbFoll != null)
                    {

                        dbFoll.facility_id = 0;

                        _icmsContext.MemberLcmFollowups.Update(dbFoll);
                        int resultFoll = _icmsContext.SaveChanges();

                        if (resultFoll > 0)
                        {
                            cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                        }
                    }
                    
                    break;

                case "Initial":

                    MemberLcmInitial dbInit = (

                            from memLcmInit in _icmsContext.MemberLcmInitials
                            where memLcmInit.lcn_case_number.Equals(report.lcnCaseNumber)
                            select memLcmInit
                        )
                        .FirstOrDefault();

                    if (dbInit != null)
                    {

                        dbInit.facility_id = 0;

                        _icmsContext.MemberLcmInitials.Update(dbInit);
                        int result = _icmsContext.SaveChanges();

                        if (result > 0)
                        {
                            //cmReport = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
                        }
                    }
                    break;
            }

            return cmReport;
        }


        public Case addCmCaseReportCode(LcmReports report)
        {

            Case cmCase = null;

            if (report.codes != null && report.codes.Count > 0)
            {

                if (report.codes[0].referralCode && !string.IsNullOrEmpty(report.codes[0].referralNumber))
                {
                    cmCase = addReferralCode(report);                    
                }
                else
                {
                    cmCase = addReportCode(report);
                }
            }            

            return cmCase;
        }
        private Case addReferralCode(LcmReports report)
        {
            Case cmCase = null;

            Utilization utilCode = new Utilization();
            utilCode.memberId = report.memberId;
            utilCode.referralNumber = report.codes[0].referralNumber;
            utilCode.codeType = report.codes[0].CodeType;
            utilCode.removeCode = false;

            cmCase = updateReferralCode(report, utilCode);

            return cmCase;
        }
        private Case addReportCode(LcmReports report)
        {
            
            Case cmCase = null;

            switch (report.reportType)
            {

                case "Initial":
                    cmCase = updateInitialReportCodes(report);
                    break;

                case "Comprehensive":
                    cmCase = updateComprehensiveCodes(report);
                    break;
            }

            return cmCase;
        }
        private Case updateInitialReportCodes(LcmReports report)
        {

            Case cmCase = null;

            if (report.lcnCaseNumber > 0 && 
                (report.codes != null && report.codes.Count > 0 && !string.IsNullOrEmpty(report.codes[0].Code)) &&
                !string.IsNullOrEmpty(report.codes[0].CodeType))
            {

                MemberLcmInitial dbInit = (

                        from memInit in _icmsContext.MemberLcmInitials
                        where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                        select memInit
                    )
                    .FirstOrDefault();

                if (dbInit != null)
                {

                    switch(report.codes[0].CodeType)
                    {
                        case "ICD10":
                            cmCase = updateInitialReportCodesDiagnosis(report, dbInit);
                            break;
                        case "CPT":
                            cmCase = updateInitialReportCodesProcedure(report, dbInit);
                            break;
                        case "HCPCS":
                            cmCase = updateInitialReportCodesProcedure(report, dbInit);
                            break;
                    }                    
                }
            }

            return cmCase;
        }
        private Case updateInitialReportCodesDiagnosis(LcmReports report, MemberLcmInitial dbInit)
        {

            Case cmCase = null;

            int result = 0;

            if (string.IsNullOrEmpty(dbInit.primary_diagnosis))
            {
                dbInit.primary_diagnosis = report.codes[0].Code;
            }
            else if (string.IsNullOrEmpty(dbInit.secondary_diagnosis))
            {
                dbInit.secondary_diagnosis = report.codes[0].Code;
            }
            else if (string.IsNullOrEmpty(dbInit.other_diagnosis))
            {
                dbInit.other_diagnosis = report.codes[0].Code;
            }

            _icmsContext.MemberLcmInitials.Update(dbInit);
            result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                cmCase.lcmReports = new List<LcmReports>();

                LcmReports cmReport = new LcmReports();

                cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                   report.lcmFollowupId,
                                                   report.lcnCaseNumber,
                                                   report.codes[0].referralNumber);

                cmCase.lcmReports.Add(cmReport);
            }

            return cmCase;
        }
        private Case updateInitialReportCodesProcedure(LcmReports report, MemberLcmInitial dbInit)
        {

            Case cmCase = null;

            int result = 0;

            if (string.IsNullOrEmpty(dbInit.procedure))
            {
                dbInit.procedure = report.codes[0].Code;
            }
            
            _icmsContext.MemberLcmInitials.Update(dbInit);
            result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                cmCase.lcmReports = new List<LcmReports>();

                LcmReports cmReport = new LcmReports();

                cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                   report.lcmFollowupId,
                                                   report.lcnCaseNumber,
                                                   report.codes[0].referralNumber);

                cmCase.lcmReports.Add(cmReport);
            }

            return cmCase;
        }
        private Case updateComprehensiveCodes(LcmReports report)
        {

            Case cmCase = null;

            if (report.lcmFollowupId > 0 &&
                (report.codes != null && report.codes.Count > 0 && !string.IsNullOrEmpty(report.codes[0].Code)) &&
                !string.IsNullOrEmpty(report.codes[0].CodeType))
            {

                MemberLcmFollowup dbFollow = (

                        from memFollow in _icmsContext.MemberLcmFollowups
                        where memFollow.lcm_followup_id.Equals(report.lcmFollowupId)
                        select memFollow
                    )
                    .FirstOrDefault();

                if (dbFollow != null)
                {

                    switch (report.codes[0].CodeType)
                    {
                        case "ICD10":
                            cmCase = updateComprehensiveReportCodesDiagnosis(report, dbFollow);
                            break;
                        case "CPT":
                            cmCase = updateComprehensiveReportCodesProcedure(report, dbFollow);
                            break;
                        case "HCPCS":
                            cmCase = updateComprehensiveReportCodesProcedure(report, dbFollow);
                            break;
                    }                    
                }
            }

            return cmCase;
        }
        private Case updateComprehensiveReportCodesDiagnosis(LcmReports report, MemberLcmFollowup dbFollow)
        {

            Case cmCase = null;

            int result = 0;

            if (string.IsNullOrEmpty(dbFollow.primary_dx))
            {
                dbFollow.primary_dx = report.codes[0].Code;
            }
            else if (string.IsNullOrEmpty(dbFollow.secondary_dx))
            {
                dbFollow.secondary_dx = report.codes[0].Code;
            }
            else if (string.IsNullOrEmpty(dbFollow.other_dx))
            {
                dbFollow.other_dx = report.codes[0].Code;
            }

            _icmsContext.MemberLcmFollowups.Update(dbFollow);
            result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                cmCase.lcmReports = new List<LcmReports>();

                LcmReports cmReport = new LcmReports();

                cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                   report.lcmFollowupId,
                                                   report.lcnCaseNumber,
                                                   report.codes[0].referralNumber);

                cmCase.lcmReports.Add(cmReport);
            }

            return cmCase;
        }
        private Case updateComprehensiveReportCodesProcedure(LcmReports report, MemberLcmFollowup dbFollow)
        {

            Case cmCase = null;

            int result = 0;

            if (string.IsNullOrEmpty(dbFollow.Procedure))
            {
                dbFollow.Procedure = report.codes[0].Code;
            }

            _icmsContext.MemberLcmFollowups.Update(dbFollow);
            result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                cmCase.lcmReports = new List<LcmReports>();

                LcmReports cmReport = new LcmReports();

                cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                   report.lcmFollowupId,
                                                   report.lcnCaseNumber,
                                                   report.codes[0].referralNumber);

                cmCase.lcmReports.Add(cmReport);
            }

            return cmCase;
        }
        public Case removeCmCaseReportCode(LcmReports report)
        {

            Case cmCase = null;

            if (report.codes != null && report.codes.Count > 0)
            {

                if (report.codes[0].referralCode && !string.IsNullOrEmpty(report.codes[0].referralNumber))
                {
                    cmCase = removeReferralCode(report);                    
                }
                else
                {
                    cmCase = removeReportCode(report);
                }
            }

            return cmCase;
        }
        private Case removeReferralCode(LcmReports report)
        {

            Case cmCase = null;

            Utilization utilCode = new Utilization();
            utilCode.memberId = report.memberId;
            utilCode.referralNumber = report.codes[0].referralNumber;
            utilCode.codeType = report.codes[0].CodeType;
            utilCode.removeCode = true;

            cmCase = updateReferralCode(report, utilCode);            

            return cmCase;
        }
        private Case removeReportCode(LcmReports report)
        {

            Case cmCase = null;

            switch (report.reportType)
            {

                case "Initial":
                    cmCase = removeInitialReportCodes(report);
                    break;

                case "Comprehensive":
                    cmCase = removeComprehensiveCodes(report);
                    break;
            }

            return cmCase;
        }
        private Case removeInitialReportCodes(LcmReports report)
        {

            Case cmCase = null;

            if (report.lcnCaseNumber > 0 &&
                            (report.codes != null && report.codes.Count > 0 && !string.IsNullOrEmpty(report.codes[0].Code)) &&
                            !string.IsNullOrEmpty(report.codes[0].CodeType))
            {

                switch (report.codes[0].CodeType)
                {
                    case "ICD10":
                        cmCase = removeInitialReportCodesDiagnosis(report);
                        break;
                    case "CPT":
                        cmCase = removeInitialReportCodesProcedure(report);
                        break;
                    case "HCPCS":
                        cmCase = removeInitialReportCodesProcedure(report);
                        break;
                }
            }

            return cmCase;
        }
        private Case removeInitialReportCodesDiagnosis(LcmReports report)
        {

            Case cmCase = null;

            int result = 0;

            MemberLcmInitial dbInit = (

                    from memInit in _icmsContext.MemberLcmInitials
                    where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    && (memInit.primary_diagnosis.Equals(report.codes[0].Code) ||
                        memInit.secondary_diagnosis.Equals(report.codes[0].Code) ||
                        memInit.other_diagnosis.Equals(report.codes[0].Code))
                    select memInit
                )
                .FirstOrDefault();

            if (dbInit != null)
            {

                if (dbInit.primary_diagnosis.Equals(report.codes[0].Code))
                {
                    dbInit.primary_diagnosis = null;
                } 
                else if (dbInit.secondary_diagnosis.Equals(report.codes[0].Code))
                {
                    dbInit.secondary_diagnosis = null;
                } 
                else if (dbInit.other_diagnosis.Equals(report.codes[0].Code))
                {
                    dbInit.other_diagnosis = null;
                }

                _icmsContext.MemberLcmInitials.Update(dbInit);
                result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    cmCase.lcmReports = new List<LcmReports>();

                    LcmReports cmReport = new LcmReports();

                    cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                       report.lcmFollowupId,
                                                       report.lcnCaseNumber,
                                                       report.codes[0].referralNumber);

                    cmCase.lcmReports.Add(cmReport);
                }
            }

            return cmCase;
        }
        private Case removeInitialReportCodesProcedure(LcmReports report)
        {

            Case cmCase = null;

            int result = 0;

            MemberLcmInitial dbInit = (

                    from memInit in _icmsContext.MemberLcmInitials
                    where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    && memInit.procedure.Equals(report.codes[0].Code)
                    select memInit
                )
                .FirstOrDefault();

            if (dbInit != null)
            {


                dbInit.procedure = null;

                _icmsContext.MemberLcmInitials.Update(dbInit);
                result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    cmCase.lcmReports = new List<LcmReports>();

                    LcmReports cmReport = new LcmReports();

                    cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                       report.lcmFollowupId,
                                                       report.lcnCaseNumber,
                                                       report.codes[0].referralNumber);

                    cmCase.lcmReports.Add(cmReport);
                }
            }

            return cmCase;
        }
        private Case removeComprehensiveCodes(LcmReports report)
        {

            Case cmCase = null;

            if (report.lcmFollowupId > 0 &&
                (report.codes != null && report.codes.Count > 0 && !string.IsNullOrEmpty(report.codes[0].Code)) &&
                !string.IsNullOrEmpty(report.codes[0].CodeType))
            {

                switch (report.codes[0].CodeType)
                {
                    case "ICD10":
                        cmCase = removeComprehensiveReportCodesDiagnosis(report);
                        break;
                    case "CPT":
                        cmCase = removeComprehensiveReportCodesProcedure(report);
                        break;
                    case "HCPCS":
                        cmCase = removeComprehensiveReportCodesProcedure(report);
                        break;
                }
            }

            return cmCase;
        }
        private Case removeComprehensiveReportCodesDiagnosis(LcmReports report)
        {

            Case cmCase = null;

            int result = 0;

            MemberLcmFollowup dbFollow = (

                    from memFollow in _icmsContext.MemberLcmFollowups
                    where memFollow.lcm_followup_id.Equals(report.lcmFollowupId)
                    && (memFollow.primary_dx.Equals(report.codes[0].Code) ||
                        memFollow.secondary_dx.Equals(report.codes[0].Code) ||
                        memFollow.other_dx.Equals(report.codes[0].Code))
                    select memFollow
                )
                .FirstOrDefault();

            if (dbFollow != null)
            {

                if (dbFollow.primary_dx.Equals(report.codes[0].Code))
                {
                    dbFollow.primary_dx = null;
                }
                else if (dbFollow.secondary_dx.Equals(report.codes[0].Code))
                {
                    dbFollow.secondary_dx = null;
                }
                else if (dbFollow.other_dx.Equals(report.codes[0].Code))
                {
                    dbFollow.other_dx = null;
                }

                _icmsContext.MemberLcmFollowups.Update(dbFollow);
                result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    cmCase.lcmReports = new List<LcmReports>();

                    LcmReports cmReport = new LcmReports();

                    cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                       report.lcmFollowupId,
                                                       report.lcnCaseNumber,
                                                       report.codes[0].referralNumber);

                    cmCase.lcmReports.Add(cmReport);
                }
            }

            return cmCase;
        }
        private Case removeComprehensiveReportCodesProcedure(LcmReports report)
        {

            Case cmCase = null;

            int result = 0;

            MemberLcmFollowup dbFollow = (

                    from memFollow in _icmsContext.MemberLcmFollowups
                    where memFollow.lcm_followup_id.Equals(report.lcmFollowupId)
                    && memFollow.Procedure.Equals(report.codes[0].Code)
                    select memFollow
                )
                .FirstOrDefault();

            if (dbFollow != null)
            {


                dbFollow.Procedure = null;

                _icmsContext.MemberLcmFollowups.Update(dbFollow);
                result = _icmsContext.SaveChanges();

                if (result > 0)
                {

                    cmCase.lcmReports = new List<LcmReports>();

                    LcmReports cmReport = new LcmReports();

                    cmReport.codes = getLcmReportCodes(report.memberId.ToString(),
                                                       report.lcmFollowupId,
                                                       report.lcnCaseNumber,
                                                       report.codes[0].referralNumber);

                    cmCase.lcmReports.Add(cmReport);
                }
            }

            return cmCase;
        }
        private Case updateReferralCode(LcmReports report, Utilization utilCode)
        {
            Case cmCase = null;

            UtilizationManagementService utilServ = new UtilizationManagementService(_icmsContext, _aspNetContext, _dataStagingContext);

            switch (utilCode.codeType)
            {
                case "ICD10":
                    utilCode.diagnosisCodes = new List<MedicalCode>();

                    MedicalCode diag = new MedicalCode();
                    diag.CodeId = report.codes[0].CodeId;
                    utilCode.diagnosisCodes.Add(diag);

                    break;

                case "CPT":
                    utilCode.cptCodes = new List<MedicalCode>();

                    MedicalCode cpt = new MedicalCode();
                    cpt.CodeId = report.codes[0].CodeId;
                    utilCode.cptCodes.Add(cpt);

                    break;

                case "HCPCS":
                    utilCode.hcpcsCodes = new List<MedicalCode>();

                    MedicalCode hcpcs = new MedicalCode();
                    hcpcs.CodeId = report.codes[0].CodeId;
                    utilCode.hcpcsCodes.Add(hcpcs);

                    break;
            }

            Utilization util = utilServ.updateCodesUm(utilCode);

            if (util != null)
            {

                cmCase = new Case();
                cmCase.lcmReports = new List<LcmReports>();

                LcmReports cmReport = new LcmReports();
                cmReport.codes = new List<MedicalCode>();

                if (util.diagnosisCodes != null)
                {

                    foreach (MedicalCode diag in util.diagnosisCodes)
                    {
                        cmReport.codes.Add(diag);
                    }
                }

                if (util.cptCodes != null)
                {

                    foreach (MedicalCode cpt in util.cptCodes)
                    {
                        cmReport.codes.Add(cpt);
                    }
                }

                if (util.hcpcsCodes != null)
                {

                    foreach (MedicalCode hcpc in util.hcpcsCodes)
                    {
                        cmReport.codes.Add(hcpc);
                    }
                }

                cmCase.lcmReports.Add(cmReport);

            }

            return cmCase;
        }

        public Case updateCmCaseReportOriginal(LcmReports report)
        {

            Case cmCase = null;

            switch (report.reportType)
            {

                case "Initial":
                    cmCase = updateInitialReportOriginal(report);
                    break;

                case "Comprehensive":
                    cmCase = updateComprehensiveOriginal(report);
                    break;
            }            

            return cmCase;
        }
        private Case updateInitialReportOriginal(LcmReports report)
        {

            Case cmCase = null;

            bool initUpdated = updateInitialOriginal(report);

            if (initUpdated)
            {
                cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }
        private Case updateComprehensiveOriginal(LcmReports report)
        {

            Case cmCase = null;

            bool initUpdated = updateInitialOriginal(report);
            bool followUpdated = updateFollowUpOriginal(report);
           
            if (initUpdated || followUpdated)
            {
                cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }
        private bool updateInitialOriginal(LcmReports report)
        {

            int initResult = 0;

            MemberLcmInitial dbMemInit = (

                    from memInit in _icmsContext.MemberLcmInitials
                    where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memInit
                )
                .FirstOrDefault();

            if (dbMemInit != null)
            {

                dbMemInit.lcm_open_date = report.lcmOpenDate;
                dbMemInit.senttoadmin = report.senttoadmin;
                dbMemInit.senttorein = report.senttorein;

                _icmsContext.MemberLcmInitials.Update(dbMemInit);
                initResult = _icmsContext.SaveChanges();
            }
            
            if (initResult > 0)
            {
                return true;
            }

            return false;
        }
        private bool updateFollowUpOriginal(LcmReports report)
        {

            int followResult = 0;

            MemberLcmFollowup dbMemFoll = (

                    from memFoll in _icmsContext.MemberLcmFollowups
                    where memFoll.lcm_followup_id.Equals(report.lcmFollowupId)
                    select memFoll
                )
                .FirstOrDefault();

            if (dbMemFoll != null)
            {

                dbMemFoll.staging = report.staging;
                dbMemFoll.staging_status = report.stagingStatus;
                dbMemFoll.acuity = report.acuity;
                dbMemFoll.acuity_changed = report.acuityChanged;
                dbMemFoll.acuity_date = report.acuityDate;
                dbMemFoll.prognosis_report = report.prognosis;
                dbMemFoll.plan_of_care = report.planOfCare;

                _icmsContext.MemberLcmFollowups.Update(dbMemFoll);
                followResult = _icmsContext.SaveChanges();
            }

            if (followResult > 0)
            {
                return true;
            }

            return false;
        }


        public Case updateCmCaseReportUpdated(LcmReports report)
        {

            Case cmCase = null;

            switch (report.reportType)
            {

                case "Initial":
                    cmCase = updateInitialReportUpdated(report);
                    break;

                case "Comprehensive":
                    cmCase = updateComprehensiveUpdated(report);
                    break;
            }

            return cmCase;
        }
        private Case updateInitialReportUpdated(LcmReports report)
        {

            Case cmCase = null;

            bool initUpdated = updateInitialUpdated(report);

            if (initUpdated)
            {
                cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }
        private Case updateComprehensiveUpdated(LcmReports report)
        {

            Case cmCase = null;

            bool followUpdated = updateFollowUpUpdated(report);

            if (followUpdated)
            {
                cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }
        private bool updateInitialUpdated(LcmReports report)
        {

            int initResult = 0;

            MemberLcmInitial dbMemInit = (

                    from memInit in _icmsContext.MemberLcmInitials
                    where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memInit
                )
                .FirstOrDefault();

            if (dbMemInit != null)
            {

                dbMemInit.cancer_related = report.cancerRelated;
                dbMemInit.hospitalized = report.hospitalized;
                dbMemInit.hospital_five_days = report.hospitalFiveDays;
                dbMemInit.facility_type = report.facilityType;

                _icmsContext.MemberLcmInitials.Update(dbMemInit);
                initResult = _icmsContext.SaveChanges();
            }

            if (initResult > 0)
            {
                return true;
            }

            return false;
        }
        private bool updateFollowUpUpdated(LcmReports report)
        {

            int followResult = 0;

            MemberLcmFollowup dbMemFoll = (

                    from memFoll in _icmsContext.MemberLcmFollowups
                    where memFoll.lcm_followup_id.Equals(report.lcmFollowupId)
                    select memFoll
                )
                .FirstOrDefault();

            if (dbMemFoll != null)
            {
                dbMemFoll.cancer_related = report.cancerRelated;
                dbMemFoll.hospitalized = report.hospitalized;
                dbMemFoll.hospital_five_days = report.hospitalFiveDays;
                dbMemFoll.facility_type = report.facilityType;

                _icmsContext.MemberLcmFollowups.Update(dbMemFoll);
                followResult = _icmsContext.SaveChanges();
            }

            if (followResult > 0)
            {
                return true;
            }

            return false;
        }


        public Case updateCmCaseReportAdditional(LcmReports report)
        {

            Case cmCase = null;

            switch (report.reportType)
            {

                case "Initial":
                    cmCase = updateInitialReportAdditional(report);
                    break;

                case "Comprehensive":
                    cmCase = updateComprehensiveAdditional(report);
                    break;
            }

            return cmCase;
        }
        private Case updateInitialReportAdditional(LcmReports report)
        {

            Case cmCase = null;

            bool initUpdated = updateInitialAdditional(report);

            if (initUpdated)
            {
                //cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }
        private Case updateComprehensiveAdditional(LcmReports report)
        {

            Case cmCase = null;

            bool followUpdated = updateFollowUpAdditional(report);

            if (followUpdated)
            {
                cmCase = getCmPatientComphrehensiveReport(report.memberId.ToString(), report.lcmFollowupId);
            }

            return cmCase;
        }      
        private bool updateInitialAdditional(LcmReports report)
        {

            int initResult = 0;

            MemberLcmInitial dbMemInit = (

                    from memInit in _icmsContext.MemberLcmInitials
                    where memInit.lcn_case_number.Equals(report.lcnCaseNumber)
                    select memInit
                )
                .FirstOrDefault();

            if (dbMemInit != null)
            {

                dbMemInit.system_user_id = report.completedByUser;

                dbMemInit.next_report_date = report.nextReportDate;
                dbMemInit.lcm_close_date = report.lcmCloseDate;

                _icmsContext.MemberLcmInitials.Update(dbMemInit);
                initResult = _icmsContext.SaveChanges();
            }

            if (initResult > 0)
            {
                return true;
            }

            return false;
        }
        private bool updateFollowUpAdditional(LcmReports report)
        {

            int followResult = 0;

            MemberLcmFollowup dbMemFoll = (

                    from memFoll in _icmsContext.MemberLcmFollowups
                    where memFoll.lcm_followup_id.Equals(report.lcmFollowupId)
                    select memFoll
                )
                .FirstOrDefault();

            if (dbMemFoll != null)
            {

                dbMemFoll.system_user_id = report.completedByUser;

                dbMemFoll.lcm_followup_date = report.lcmFinalizedDate;
                dbMemFoll.lcm_close_date = report.lcmCloseDate;
                dbMemFoll.next_report_date = report.nextReportDate;

                _icmsContext.MemberLcmFollowups.Update(dbMemFoll);
                followResult = _icmsContext.SaveChanges();
            }

            if (followResult > 0)
            {
                return true;
            }

            return false;
        }


        public Case updateCmCaseReportNotes(LcmReports report)
        {

            Case cmCase = null;

            switch (report.reportType)
            {
                case "Comprehensive":
                    cmCase = updateComprehensiveNotes(report);
                    break;
            }

            return cmCase;
        }

        private Case updateComprehensiveNotes(LcmReports report)
        {
            Case ccmCase = null;

            NoteService noteServ = new NoteService(_icmsContext, _aspNetContext, _dataStagingContext);
            Note ccmRptNotes = noteServ.addCcmNote(report.reportNotes);

            if (ccmRptNotes != null)
            {
                ccmCase = new Case();
                ccmCase.lcmReports = new List<LcmReports>();

                LcmReports rpt = new LcmReports();
                rpt.lcnCaseNumber = report.lcnCaseNumber;
                rpt.lcmFollowupId = report.lcmFollowupId;
                rpt.reportNotes = ccmRptNotes;

                ccmCase.lcmReports.Add(rpt);
            }

            return ccmCase;
        }


        public Case createCmCaseReportComprehensive(LcmReports report)
        {
            Case cmCase = null;

            if (finalizeComprehensiveReport(ref report))
            {
                cmCase = new Case();
                cmCase.lcmFollowupId = report.newFollowupId;

                cmCase.lcmReports = new List<LcmReports>();

                LcmReports rpt = new LcmReports();
                rpt.lcnCaseNumber = report.lcnCaseNumber;
                rpt.lcmFollowupId = report.newFollowupId;

                cmCase.lcmReports.Add(rpt);
            }

            return cmCase;
        }

    }
}

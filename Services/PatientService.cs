using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class PatientService : IPatient
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _emrContext;

        public PatientService(IcmsContext icmsContext, AspNetContext emrContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _emrContext = emrContext ?? throw new ArgumentNullException(nameof(emrContext));
        }

        public List<Patient> searchPatients(Patient search)
        {
            List<Patient> returnFoundPatients = new List<Patient>();
            List<Member> matchingSearchMembers = null;

            bool searchLastName = false;
            bool searchFirstName = false;
            bool searchId = false;
            bool searchDob = false;
            DateTime dob = DateTime.MinValue;
            bool searchSsn = false;
            bool searchAuthNumber = false;

            if (!string.IsNullOrEmpty(search.lastName))
            {
                searchLastName = true;
            }

            if (!string.IsNullOrEmpty(search.firstName))
            {
                searchFirstName = true;
            }

            if (!string.IsNullOrEmpty(search.insuranceMemberId))
            {
                searchId = true;
            }

            if (!string.IsNullOrEmpty(search.dateOfBirthDisplay))
            {
                if (DateTime.TryParse(search.dateOfBirthDisplay, out dob))
                {
                    searchDob = true;
                }
            }

            if (!string.IsNullOrEmpty(search.ssn))
            {
                searchSsn = true;
            }

            if (search.referrals != null && 
                search.referrals.Count > 0 && 
                !string.IsNullOrEmpty(search.referrals[0].authNumber))
            {
                searchAuthNumber = true;
            }


            //get list of members matching the search parameters
            matchingSearchMembers = getPatientUsingParams(searchLastName, searchFirstName, searchId, searchSsn, searchDob, searchAuthNumber, search);


            if (matchingSearchMembers != null && matchingSearchMembers.Count > 0)
            {

                //create a list of memberIds from the list of matching members
                List<Guid> listFoundMemberIdsToSearch = new List<Guid>();

                foreach (Member rtPat in matchingSearchMembers)
                {

                    if (!rtPat.member_id.Equals(Guid.Empty))
                    {
                        listFoundMemberIdsToSearch.Add(rtPat.member_id);
                    }                  
                }

                //get list of memberEnrollment records based on the list of memberIds
                List<MemberEnrollment> enrollment = _icmsContext.MemberEnrollments.Where(item => 
                                                           listFoundMemberIdsToSearch.Contains(item.member_id)).ToList();


                returnFoundPatients = (
                                    from pat in matchingSearchMembers

                                    //join memberEnroll in _icmsContext.MemberEnrollments
                                    //on pat.member_id equals memberEnroll.member_id 

                                    //join employer in _icmsContext.Employers
                                    //on memberEnroll.employer_id equals employer.employer_id into employers
                                    //from memberEmployer in employers.DefaultIfEmpty()

                                    join ethnic in _icmsContext.MemberEthnicity
                                    on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                    from memeth in memberethnic.DefaultIfEmpty()

                                    join meminsr in _icmsContext.MemberInsurances
                                    on pat.member_id equals meminsr.member_id into insurance
                                    from patInsur in insurance.DefaultIfEmpty()

                                    select new Patient
                                    {
                                        PatientId = pat.member_id,
                                        FullName = pat.member_first_name +
                                                ((!string.IsNullOrEmpty(pat.member_middle_name)) ? " " + pat.member_middle_name + " " : " ") +
                                                pat.member_last_name,
                                        firstName = pat.member_first_name,
                                        lastName = pat.member_last_name,
                                        middleName = (string.IsNullOrEmpty(pat.member_middle_name)) ? pat.member_middle_name : "",
                                        dateOfBirth = pat.member_birth,
                                        dateOfBirthDisplay = (!pat.member_birth.Equals(DateTime.MinValue)) ? pat.member_birth?.ToString("d") : "",
                                        ssn = pat.member_ssn,
                                        gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                        emailAddress = pat.member_email,
                                        ethnicity = memeth?.ethnicity ?? String.Empty,                                        
                                        InsuranceName = (patInsur?.insurance_name != null) ? patInsur.insurance_name : "",
                                        //insuranceMemberId = (memberEnroll.egp_member_id != null) ?
                                        //                        memberEnroll.egp_member_id : 
                                        //                        (memberEnroll.claims_enrollment_id != null) ? 
                                        //                        memberEnroll.claims_enrollment_id : "",
                                        //employerName = (memberEmployer != null) ? 
                                        //                    (memberEmployer.employer_name != null) ? memberEmployer.employer_name : "" : ""memberEmployer.employer_name
                                    }
                                 )
                                 .ToList();


                if (enrollment != null && returnFoundPatients != null)
                {
                    foreach (Patient rtPat in returnFoundPatients)
                    {
                        foreach (MemberEnrollment rtPatEnroll in enrollment)
                        {
                            if (rtPat.PatientId == rtPatEnroll.member_id)
                            {
                                if (rtPatEnroll.egp_member_id != null)
                                {
                                    rtPat.insuranceMemberId = rtPatEnroll.egp_member_id;
                                }
                                else
                                {
                                    rtPat.insuranceMemberId = rtPatEnroll.claims_enrollment_id;
                                }

                                string employeName = getPatientEmployerName(rtPatEnroll.member_id);

                                if (employeName != null)
                                {
                                    rtPat.employerName = employeName;
                                }
                            }                            
                        }
                    }
                }
            }

            return returnFoundPatients;
        }

        private List<Member> getPatientUsingLastFirstSsnDobAuthNum(string lastName, string firstName, string ssn, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use ssn
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstSsnDob(string lastName, string firstName, string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstSsnAuthNumber(string lastName, string firstName, string ssn, string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use ssn
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstSsn(string lastName, string firstName, string ssn)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstDobAuthNumber(string lastName, string firstName, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstDob(string lastName, string firstName, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirstAuth(string lastName, string firstName, string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use first name
            members = (
                        from pats in _icmsContext.Patients

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastFirst(string lastName, string firstName)
        {
            List<Member> members = null;

            //use last name
            //use first name
            members = (
                        from pats in _icmsContext.Patients
                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdSsnDobAuthNumber(string lastName, string insuranceMemberId, string ssn, 
                                                                           string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use insurance id
            //use ssn
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdSsnDob(string lastName, string insuranceMemberId, string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use insurance id
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdSsnAuthNumber(string lastName, string insuranceMemberId, string ssn,
                                                                        string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use insurance id
            //use ssn
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdSsn(string lastName, string insuranceMemberId, string ssn)
        {
            List<Member> members = null;

            //use last name
            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdDobAuthNumber(string lastName, string insuranceMemberId, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use insurance id
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdDob(string lastName, string insuranceMemberId, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceIdAuthNumber(string lastName, string insuranceMemberId, string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use insurance id
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                        )
                        .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastInsuranceId(string lastName, string insuranceMemberId)
        {
            List<Member> members = null;

            //use last name
            //use insurance id
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                        )
                        .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastSsnDobAuthNumber(string lastName, string ssn, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use ssn
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastSsnDob(string lastName, string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastSsnAuthNumber(string lastName, string ssn, string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use ssn
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastSsn(string lastName, string ssn)
        {
            List<Member> members = null;

            //use last name
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastDobAuthNumber(string lastName, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastDob(string lastName, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingLastAuthNumber(string lastName, string authNumber)
        {
            List<Member> members = null;

            if (lastName.Length <= 3)
            {

                //use last name
                members = (
                            from pats in _icmsContext.Patients

                            join patRefs in _icmsContext.MemberReferrals
                            on pats.member_id equals patRefs.member_id into patReferral
                            from patReferrals in patReferral.DefaultIfEmpty()

                            where pats.member_last_name.StartsWith(lastName)
                            && pats.member_active_flag.Equals(true)
                            && patReferrals.auth_number.Equals(authNumber)
                            orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                            select pats
                           )
                           .Take(150)
                           .ToList();

            }
            else
            {

                //use last name
                members = (
                            from pats in _icmsContext.Patients

                            join patRefs in _icmsContext.MemberReferrals
                            on pats.member_id equals patRefs.member_id into patReferral
                            from patReferrals in patReferral.DefaultIfEmpty()

                            where pats.member_last_name.StartsWith(lastName)
                            && pats.member_active_flag.Equals(true)
                            && patReferrals.auth_number.Equals(authNumber)
                            orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                            select pats
                           )
                           .ToList();

            }

            return members;
        }

        public List<Member> getPatientUsingLast(string lastName)
        {
            List<Member> members = null;

            if (lastName.Length <= 3)
            {

                //use last name
                members = (
                            from pats in _icmsContext.Patients
                            where pats.member_last_name.StartsWith(lastName)
                            && pats.member_active_flag.Equals(true)
                            orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                            select pats
                           )
                           .Take(150)
                           .ToList();

            }
            else
            {

                //use last name
                members = (
                            from pats in _icmsContext.Patients
                            where pats.member_last_name.StartsWith(lastName)
                            && pats.member_active_flag.Equals(true)
                            orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                            select pats
                           )
                           .ToList();

            }

            return members;
        }

        public List<Member> getPatientUsingInsuranceIdFirstSsnDobAuthNumber(string firstName, string insuranceMemberId, string ssn, string birth,
                                                                            string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use first name
            //use insurance id
            //use ssn
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientUsingInsuranceIdFirstSsnDob(string firstName, string insuranceMemberId, string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use first name
            //use insurance id
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingInsuranceIdFirstSsnAuthNumber(string firstName, string insuranceMemberId, string ssn, string authNumber)
        {
            List<Member> members = null;

            //use first name
            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientUsingInsuranceIdFirstSsn(string firstName, string insuranceMemberId, string ssn)
        {
            List<Member> members = null;

            //use first name
            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingInsuranceIdFirstDobAuthNumber(string firstName, string insuranceMemberId, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use first name
            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientUsingInsuranceIdFirstDob(string firstName, string insuranceMemberId, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use first name
            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientUsingInsuranceIdFirstAuthNumber(string insuranceMemberId, string firstName, string authNumber)
        {
            List<Member> members = null;

            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                              memenroll.claims_enrollment_id.StartsWith(insuranceMemberId)) &&
                              pats.member_first_name.StartsWith(firstName)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientUsingInsuranceIdFirst(string insuranceMemberId, string firstName)
        {
            List<Member> members = null;

            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                              memenroll.claims_enrollment_id.StartsWith(insuranceMemberId)) &&
                              pats.member_first_name.StartsWith(firstName)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingInsuranceIdSsnDobAuthNumber(string insuranceMemberId, string ssn, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use insurance id
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientsUsingInsuranceIdSsnDob(string insuranceMemberId, string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use insurance id
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingInsuranceIdSsnAuthNumber(string insuranceMemberId, string ssn, string authNumber)
        {
            List<Member> members = null;

            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientsUsingInsuranceIdSsn(string insuranceMemberId, string ssn)
        {
            List<Member> members = null;

            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingInsuranceIdDobAuthNumber(string insuranceMemberId, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingInsuranceIdDob(string insuranceMemberId, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingInsuranceIdAuthNumber(string insuranceMemberId, string authNumber)
        {
            List<Member> members = null;

            //use insurance id
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientsUsingInsuranceId(string insuranceMemberId)
        {
            List<Member> members = null;

            //use insurance id
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                               memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingSsnDobAuthNumber(string ssn, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }
        public List<Member> getPatientsUsingSsnDob(string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingSsnAuthNumber(string ssn, string authNumber)
        {
            List<Member> members = null;

            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingSsn(string ssn)
        {
            List<Member> members = null;

            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingDobAuthNumber(string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && patReferrals.auth_number.Equals(authNumber)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .Take(100000)
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingDob(string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .Take(100000)
                       .ToList();

            return members;
        }

        public List<Member> getPatientsUsingAuthNumber(string authNumber)
        {
            List<Member> members = null;

            //use auth_number
            members = (
                        from pats in _icmsContext.Patients

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id 

                        where (patRefs.auth_number.StartsWith(authNumber) || patRefs.referral_number.StartsWith(authNumber))
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        public Patient searchPatient(string patientId)
        {
            Patient patientsFound = new Patient();

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {

                Patient dbPatient = new Patient();

                dbPatient = (
                                 from pat in _icmsContext.Patients

                                 join ethnic in _icmsContext.MemberEthnicity
                                 on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                 from memeth in memberethnic.DefaultIfEmpty()

                                 join insur in _icmsContext.MemberInsurances
                                 on pat.member_id equals insur.member_id into memberInsur
                                 from memInsur in memberInsur.DefaultIfEmpty()

                                 where pat.member_id.Equals(memberId)

                                 orderby pat.member_last_name, pat.member_first_name, pat.member_birth

                                 select new Patient
                                 {
                                     PatientId = pat.member_id,
                                     FullName = pat.member_first_name +
                                             ((!string.IsNullOrEmpty(pat.member_middle_name)) ? " " + pat.member_middle_name + " " : " ") +
                                             pat.member_last_name,
                                     firstName = pat.member_first_name,
                                     lastName = pat.member_last_name,
                                     middleName = (!string.IsNullOrEmpty(pat.member_middle_name)) ? pat.member_middle_name : "",
                                     dateOfBirth = (pat.member_birth.HasValue) ? pat.member_birth : DateTime.MinValue,
                                     dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "",
                                     age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0,
                                     ssn = pat.member_ssn,
                                     gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                     emailAddress = pat.member_email,
                                     ethnicity = memeth.ethnicity ?? String.Empty,
                                     insuranceId = (memInsur != null) ? memInsur.member_insurance_id : 0,
                                     insuranceMemberId = (memInsur != null) ? memInsur.insurance_id : "",
                                     insuranceGroupNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                                     insurancePlanNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                                     insuranceRelationshipId = (memInsur != null) ? memInsur.insurance_relationship_id : 0,
                                     insuranceSubscriberFirstName = (memInsur != null) ? memInsur.subscriber_first_name : "",
                                     insuranceSubscriberLastName = (memInsur != null) ? memInsur.subscriber_last_name : "",
                                     InsuranceName = (memInsur != null) ? memInsur.insurance_name : "",
                                     medicaidNumber = (pat.member_medicaid_num != null) ? pat.member_medicaid_num : "",
                                     medicareNumber = (pat.member_medicare_num != null) ? pat.member_medicare_num : "",
                                     maritalStatusId = pat.hospital_marital_status_types_id,
                                     primaryLanguageId = pat.languages_id,
                                 })
                                 .FirstOrDefault();

                if (dbPatient != null)
                {
                    dbPatient.allergies = getPatientAllergies((Guid)dbPatient.PatientId);
                    dbPatient.dnr = getPatientAdvancedDirectives((Guid)dbPatient.PatientId);
                    dbPatient.addresses = GetPatientAddress(dbPatient.PatientId.ToString(), true);
                    dbPatient.homePhoneNumber = GetPatientHomePhoneNumber(dbPatient.PatientId.ToString(), true);
                    dbPatient.pastAdmissions = getPatientPastAdmissions((Guid)dbPatient.PatientId, 10);
                    dbPatient.ancestry = GetPatientAncestryUsingId(dbPatient.PatientId.ToString());
                    //dbPatient.isolation = getPatientIsolation((Guid)dbPatient.PatientId, true);

                    patientsFound = dbPatient;
                }

            }

            return patientsFound;
        }

        public Patient getPatientDemographics(string patientId)
        {
            Patient patientsFound = new Patient();

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {

                Patient dbPatient = new Patient();

                dbPatient = (
                                 from pat in _icmsContext.Patients

                                 join memEnroll in _icmsContext.MemberEnrollments
                                 on pat.member_id equals memEnroll.member_id into memEnrolls
                                 from memberEnroll in memEnrolls.DefaultIfEmpty()

                                 join employs in _icmsContext.Employers
                                 on memberEnroll.employer_id equals employs.employer_id into employer
                                 from employ in employer.DefaultIfEmpty()

                                 join ethnic in _icmsContext.MemberEthnicity
                                 on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                 from memeth in memberethnic.DefaultIfEmpty()

                                 join insur in _icmsContext.MemberInsurances
                                 on pat.member_id equals insur.member_id into memberInsur
                                 from memInsur in memberInsur.DefaultIfEmpty()

                                 join memProg in _icmsContext.MemberPrograms
                                 on pat.member_id equals memProg.member_id into memberProgs
                                 from memPrograns in memberProgs.DefaultIfEmpty()

                                 where pat.member_id.Equals(memberId)

                                 orderby pat.member_last_name, pat.member_first_name, pat.member_birth

                                 select new Patient
                                 {

                                     PatientId = pat.member_id,
                                     FullName = pat.member_first_name +
                                             ((!string.IsNullOrEmpty(pat.member_middle_name)) ? " " + pat.member_middle_name + " " : " ") +
                                             pat.member_last_name,
                                     firstName = pat.member_first_name,
                                     lastName = pat.member_last_name,
                                     middleName = (!string.IsNullOrEmpty(pat.member_middle_name)) ? pat.member_middle_name : "",
                                     dateOfBirth = (pat.member_birth.HasValue) ? pat.member_birth : DateTime.MinValue,
                                     dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "",
                                     age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0,
                                     ssn = pat.member_ssn,
                                     gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                     emailAddress = pat.member_email,
                                     ethnicity = memeth.ethnicity ?? String.Empty,
                                     claimsEnrollmentId = memberEnroll.claims_enrollment_id,
                                     egpMemberId = (memberEnroll.egp_member_id != null) ? memberEnroll.egp_member_id : memberEnroll.claims_enrollment_id,
                                     selfPay = memInsur.self_pay,
                                     unInsured = memInsur.uninsured,
                                     insuranceId = (memInsur != null) ? memInsur.member_insurance_id : 0,
                                     insuranceMemberId = (memInsur != null) ? memInsur.insurance_id : "",
                                     insuranceGroupNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                                     insurancePlanNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                                     insuranceRelationshipId = (memInsur != null) ? memInsur.insurance_relationship_id : 0,
                                     insuranceSubscriberFirstName = (memInsur != null) ? memInsur.subscriber_first_name : "",
                                     insuranceSubscriberLastName = (memInsur != null) ? memInsur.subscriber_last_name : "",
                                     InsuranceName = (memInsur != null) ? memInsur.insurance_name : "",
                                     medicaidNumber = (pat.member_medicaid_num != null) ? pat.member_medicaid_num : "",
                                     medicareNumber = (pat.member_medicare_num != null) ? pat.member_medicare_num : "",
                                     maritalStatusId = pat.hospital_marital_status_types_id,
                                     primaryLanguageId = pat.languages_id,
                                     employerId = memberEnroll.employer_id,
                                     employerName = employ.employer_name,
                                     effectiveDate = memberEnroll.member_effective_date,
                                     disenrollDate = memberEnroll.member_disenroll_date,
                                     inLcm = pat.member_in_lcm,
                                     optOutLcm = (memPrograns.lcm_optout.HasValue) ? memPrograns.lcm_optout : false,
                                     optOutLcmDate = memPrograns.lcm_optout_date,
                                     newlyIdentifiedCaseStatusIid = pat.newly_identified_cm_member_case_status_id,
                                     newlyIdentifiedDateOfReferral = pat.newly_identified_cm_member_date_of_referral,
                                     newlyIdentifiedMethodOfIdentification = pat.newly_identified_cm_member_method_of_identification
                                 })
                                 .FirstOrDefault();

                if (dbPatient != null)
                {
                    dbPatient.allergies = getPatientAllergies((Guid)dbPatient.PatientId);
                    dbPatient.dnr = getPatientAdvancedDirectives((Guid)dbPatient.PatientId);
                    dbPatient.addresses = GetPatientAddress(dbPatient.PatientId.ToString(), false);
                    dbPatient.contactNumbers = GetPatientContactNumbers(dbPatient.PatientId.ToString());
                    dbPatient.pastAdmissions = getPatientPastAdmissions((Guid)dbPatient.PatientId, 10);
                    dbPatient.ancestry = GetPatientAncestryUsingId(dbPatient.PatientId.ToString());
                    dbPatient.hospitals = getPatientHospitalList((Guid)dbPatient.PatientId);
                    dbPatient.caseOwners = getPatientCaseOwnerList((Guid)dbPatient.PatientId);
                    //dbPatient.isolation = getPatientIsolation((Guid)dbPatient.PatientId, true);

                    patientsFound = dbPatient;
                }

            }

            return patientsFound;
        }

        public Patient searchPatientAge(string patientId)
        {
            Patient patientsFound = new Patient();

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {

                Patient dbPatient = new Patient();

                dbPatient = (
                                 from pat in _icmsContext.Patients

                                 join ethnic in _icmsContext.MemberEthnicity on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                 from memeth in memberethnic.DefaultIfEmpty()

                                 where pat.member_id.Equals(memberId)

                                 orderby pat.member_last_name, pat.member_first_name, pat.member_birth

                                 select new Patient
                                 {
                                     age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0,
                                 })
                                 .FirstOrDefault();

                if (dbPatient != null)
                {
                    patientsFound = dbPatient;
                }

            }

            return patientsFound;
        }

        public IEnumerable<Member> GetPatientsUsingFirstLastDob(string first, string last, string dob)
        {
            IEnumerable<Member> patients = Enumerable.Empty<Member>();
            DateTime memberBirth = DateTime.MinValue;

            if (!string.IsNullOrEmpty(dob) && DateTime.TryParse(dob, out memberBirth))
            {
                patients = _icmsContext.Patients
                            .Where(pat => pat.member_first_name.StartsWith(first)
                                          && pat.member_last_name.StartsWith(last)
                                          && pat.member_birth.Equals(memberBirth))
                            .ToList()
                            .Take(150)
                            .OrderBy(pat => pat.member_last_name)
                            .ThenBy(pat => pat.member_first_name)
                            .ThenBy(pat => pat.member_birth);
            }
            else
            {
                patients = _icmsContext.Patients
                            .Where(pat => pat.member_first_name.StartsWith(first)
                                   && pat.member_last_name.StartsWith(last))
                            .ToList()
                            .Take(150)
                            .OrderBy(pat => pat.member_last_name)
                            .ThenBy(pat => pat.member_first_name)
                            .ThenBy(pat => pat.member_birth);
            }

            return patients;
        }

        public Patient GetPatientsUsingId(string id)
        {
            Patient patient = new Patient();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                patient = (from pat in _icmsContext.Patients

                           join memEnroll in _icmsContext.MemberEnrollments
                           on pat.member_id equals memEnroll.member_id into memEnrolls
                           from memberEnroll in memEnrolls.DefaultIfEmpty()

                           join employs in _icmsContext.Employers
                           on memberEnroll.employer_id equals employs.employer_id into employer
                           from employ in employer.DefaultIfEmpty()

                           join ethnic in _icmsContext.MemberEthnicity
                           on pat.member_id equals ethnic.member_id into ethnic
                           from memberethnic in ethnic.DefaultIfEmpty()

                           join insur in _icmsContext.MemberInsurances
                           on pat.member_id equals insur.member_id into memberInsur
                           from memInsur in memberInsur.DefaultIfEmpty()

                           join memProg in _icmsContext.MemberPrograms
                           on pat.member_id equals memProg.member_id into memberProgs
                           from memPrograns in memberProgs.DefaultIfEmpty()

                           where pat.member_id.Equals(guidMemId)
                           select new Patient
                           {
                               PatientId = pat.member_id,
                               firstName = pat.member_first_name,
                               lastName = pat.member_last_name,
                               FullName = pat.member_first_name + " " + pat.member_last_name,
                               middleName = pat.member_middle_name,
                               dateOfBirth = pat.member_birth,
                               dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "",
                               ssn = pat.member_ssn,
                               gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                               emailAddress = pat.member_email,
                               ethnicity = memberethnic.ethnicity,
                               medicaidNumber = (pat.member_medicaid_num != null) ? pat.member_medicaid_num : "",
                               medicareNumber = (pat.member_medicare_num != null) ? pat.member_medicare_num : "",
                               egpMemberId = (memberEnroll.egp_member_id != null) ? memberEnroll.egp_member_id : memberEnroll.claims_enrollment_id,

                               claimsEnrollmentId = memberEnroll.claims_enrollment_id,
                               selfPay = memInsur.self_pay,
                               unInsured = memInsur.uninsured,
                               insuranceId = (memInsur != null) ? memInsur.member_insurance_id : 0,
                               insuranceMemberId = (memInsur != null) ? memInsur.insurance_id : "",
                               insuranceGroupNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                               insurancePlanNumber = (memInsur != null) ? memInsur.insurance_plan_number : "",
                               insuranceRelationshipId = (memInsur != null) ? memInsur.insurance_relationship_id : 0,
                               insuranceSubscriberFirstName = (memInsur != null) ? memInsur.subscriber_first_name : "",
                               insuranceSubscriberLastName = (memInsur != null) ? memInsur.subscriber_last_name : "",
                               InsuranceName = (memInsur != null) ? memInsur.insurance_name : "",


                               maritalStatusId = pat.hospital_marital_status_types_id,
                               primaryLanguageId = pat.languages_id,
                               employerId = memberEnroll.employer_id,
                               employerName = employ.employer_name,

                               effectiveDate = memberEnroll.member_effective_date,
                               disenrollDate = memberEnroll.member_disenroll_date,
                               inLcm = pat.member_in_lcm,
                               optOutLcm = (memPrograns.lcm_optout.HasValue) ? memPrograns.lcm_optout : false,
                               optOutLcmDate = memPrograns.lcm_optout_date,
                               newlyIdentifiedCaseStatusIid = pat.newly_identified_cm_member_case_status_id,
                               newlyIdentifiedDateOfReferral = pat.newly_identified_cm_member_date_of_referral,
                               newlyIdentifiedMethodOfIdentification = pat.newly_identified_cm_member_method_of_identification
                           }).SingleOrDefault();
            }

            if (patient != null)
            {
                patient.addresses = GetPatientAddress(id, false);
                patient.homePhoneNumber = GetPatientHomePhoneNumber(id, true);
                patient.eveningPhoneNumber = GetPatientEveningPhoneNumber(id, true);
                patient.ancestry = GetPatientAncestryUsingId(id);
            }


            return patient;
        }

        public Patient getAdmissionPatientsUsingId(Guid patientId)
        {
            Patient patient = null;

            patient = (
                        from pat in _icmsContext.Patients

                        join ethnic in _icmsContext.MemberEthnicity
                        on pat.member_id equals ethnic.member_id into ethnic
                        from memberethnic in ethnic.DefaultIfEmpty()

                        join memhltref in _icmsContext.MemberHealthPlanReferences
                        on pat.member_id equals memhltref.member_id into memhltrefs
                        from memberHealthRef in memhltrefs.DefaultIfEmpty()

                        where pat.member_id.Equals(patientId)
                        select new Patient
                        {
                            PatientId = pat.member_id,
                            firstName = pat.member_first_name,
                            lastName = pat.member_last_name,
                            middleName = pat.member_middle_name,
                            FullName = pat.member_first_name + ((!string.IsNullOrEmpty(pat.member_middle_name)) ? " " + pat.member_middle_name + " " : " ") + pat.member_last_name,
                            dateOfBirth = pat.member_birth,
                            dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "N/A",
                            age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0,
                            ssn = pat.member_ssn,
                            gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                            emailAddress = pat.member_email,
                            ethnicity = memberethnic.ethnicity,
                            mrn = memberHealthRef.mrn
                        }
                      )
                      .SingleOrDefault();

            if (patient != null)
            {
                patient.addresses = GetPatientAddress(patientId.ToString(), false);
                patient.homePhoneNumber = GetPatientHomePhoneNumber(patientId.ToString(), true);
            }

            return patient;
        }

        public Patient GetPatientEnrollmentUsingId(string id)
        {
            Patient enrollment = new Patient();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                enrollment = (

                        from memEnroll in _icmsContext.MemberEnrollments
                        where memEnroll.member_id.Equals(guidMemId)
                        select new Patient
                        {
                            PatientId = memEnroll.member_id,
                            claimsEnrollmentId = memEnroll.claims_enrollment_id,
                            egpMemberId = (memEnroll.egp_member_id != null) ? memEnroll.egp_member_id : memEnroll.claims_enrollment_id,
                        })
                        .FirstOrDefault();
            }

            return enrollment;
        }

        public Patient GetDbmsMember(string id)
        {
            Patient patient = new Patient();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                patient = (from pat in _icmsContext.Patients

                           join memEnroll in _icmsContext.MemberEnrollments
                           on pat.member_id equals memEnroll.member_id into memEnroll
                           from memEnrollment in memEnroll.DefaultIfEmpty()

                           join emply in _icmsContext.Employers
                           on memEnrollment.employer_id equals emply.employer_id into emply
                           from employer in emply.DefaultIfEmpty()

                           join tpaemply in _icmsContext.TpaEmployers
                           on memEnrollment.employer_id equals tpaemply.employer_id into tpaemply
                           from tpaEmployer in tpaemply.DefaultIfEmpty()

                           join tpa in _icmsContext.Tpas
                           on tpaEmployer.tpa_id equals tpa.tpa_id into tpa
                           from tpaEmply in tpa.DefaultIfEmpty()

                           where pat.member_id.Equals(guidMemId)
                           select new Patient
                           {
                               PatientId = pat.member_id,
                               firstName = pat.member_first_name,
                               lastName = pat.member_last_name,
                               middleName = pat.member_middle_name,
                               dateOfBirth = pat.member_birth,
                               ssn = pat.member_ssn,
                               gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                               emailAddress = pat.member_email,
                               employerId = memEnrollment.employer_id,
                               employerName = employer.employer_name,
                               employerLcmRate = employer.lcm_billing_rate,
                               tpaId = tpaEmployer.tpa_id,
                               tpaName = tpaEmply.tpa_name
                           }).SingleOrDefault();
            }


            return patient;
        }

        public List<Patient> getPatientsFromList(List<Member> patientList)
        {
            List<Patient> returnPatients = null;

            if (patientList != null && patientList.Count > 0) returnPatients = new List<Patient>();

            foreach (Member searchPatient in patientList)
            {
                Patient dbPatient = GetPatientsUsingId(searchPatient.member_id.ToString());

                if (dbPatient != null)
                {
                    returnPatients.Add(dbPatient);
                }
            }

            return returnPatients;
        }

        public Doctor getPatientGeneralProvider(Guid patientId)
        {
            Doctor generalProvider = null;

            generalProvider = (
                                from memPcp in _icmsContext.MemberPcps

                                join pcp in _icmsContext.Pcps
                                on memPcp.pcp_id equals pcp.pcp_id into provider
                                from pcps in provider.DefaultIfEmpty()

                                    //PROVIDER_ADDRESS
                                join pcpaddr in _icmsContext.PcpAddresses
                                on pcps.pcp_id equals pcpaddr.pcp_id into providerAddr
                                from pcpaddrs in providerAddr.DefaultIfEmpty()

                                    //PROVIDER_ADDRESS_CONTACT  provider_address_id - provider_contact_id
                                join pcpaddrcontact in _icmsContext.PcpAddressContacts
                                on pcpaddrs.provider_address_id equals pcpaddrcontact.provider_address_id into providerAddrContact
                                from pcpaddrcontacts in providerAddrContact.DefaultIfEmpty()

                                    //PROVIDER_CONTACT  provider_contact_id
                                join pcpcontact in _icmsContext.PcpContacts
                                on pcpaddrcontacts.provider_contact_id equals pcpcontact.provider_contact_id into providerContact
                                from pcpcontacts in providerContact.DefaultIfEmpty()

                                    //PROVIDER_CONTACT_PHONE  provider_contact_id - provider_phone_id
                                join pcpcontactphn in _icmsContext.PcpContactPhones
                                on pcpaddrcontacts.provider_contact_id equals pcpcontactphn.provider_contact_id into providerContactPhone
                                from pcpContactPhns in providerContactPhone.DefaultIfEmpty()

                                join pcpphn in _icmsContext.PcpPhoneNumbers
                                on pcpContactPhns.provider_phone_id equals pcpphn.provider_phone_id into providerPhone
                                from pcpphns in providerPhone.DefaultIfEmpty()

                                where memPcp.member_id.Equals(patientId)
                                select new Doctor
                                {
                                    pcpId = pcps.pcp_id,
                                    firstName = pcps.provider_first_name,
                                    lastName = pcps.provider_last_name,
                                    phoneNumber = pcpphns.phone_number,
                                    fullName = pcps.provider_first_name + " " + pcps.provider_last_name
                                }
                              )
                              .FirstOrDefault();

            return generalProvider;
        }

        public MedicalHistory getPatientMedicalHistory(Guid patientId)
        {
            MedicalHistory patientMedicalHistory = null;

            patientMedicalHistory = (
                                        from medhist in _icmsContext.MemberMedicalHistorys
                                        where medhist.member_id.Equals(patientId)
                                        select new MedicalHistory
                                        {
                                            historyId = medhist.member_medical_history_id,
                                            history = medhist.medical_history,
                                            isFamilyHistory = false
                                        }
                                    )
                                    .FirstOrDefault();

            return patientMedicalHistory;
        }

        public MedicalHistory getPatientFamilyMedicalHistory(Guid patientId)
        {
            MedicalHistory patientMedicalHistory = null;

            patientMedicalHistory = (
                                        from famMedHist in _icmsContext.MemberFamilyMedicalHistorys
                                        where famMedHist.member_id.Equals(patientId)
                                        select new MedicalHistory
                                        {
                                            historyId = famMedHist.member_family_medical_history_id,
                                            history = famMedHist.family_history,
                                            isFamilyHistory = true
                                        }
                                    )
                                    .FirstOrDefault();

            return patientMedicalHistory;
        }

        public NextOfKin getPatientNextOfKin(Guid patientId)
        {
            NextOfKin nextOfKin = null;

            nextOfKin = (
                            from nxtKin in _icmsContext.MemberNextOfKins

                            join relations in _icmsContext.SimsErRelationships
                            on nxtKin.sims_er_relationship_id equals relations.sims_er_relationship_id into relships
                            from simsRelation in relships.DefaultIfEmpty()

                            where nxtKin.member_id.Equals(patientId)
                            select new NextOfKin
                            {
                                nextOfKinId = nxtKin.member_next_of_kin_id,
                                firstName = nxtKin.first_name,
                                lastName = nxtKin.last_name,
                                relationshipId = nxtKin.sims_er_relationship_id,
                                relationship = simsRelation.name,
                                phoneNumber = nxtKin.phone_number,
                                fullName = nxtKin.first_name + " " + nxtKin.last_name
                            }
                        )
                        .FirstOrDefault();

            return nextOfKin;
        }

        public List<HospitalRace> GetPatientAncestryUsingId(string id)
        {
            List<HospitalRace> ancestry = null;

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                ancestry = (from memberrace in _icmsContext.MemberRaces

                            join hosrace in _icmsContext.HospitalRaces
                            on memberrace.hospital_race_ID equals hosrace.hospital_race_ID into hosrace
                            from hospitalrace in hosrace.DefaultIfEmpty()

                            where memberrace.member_id.Equals(guidMemId)
                            select new HospitalRace
                            {
                                hospital_race_ID = memberrace.hospital_race_ID,
                                race_name = hospitalrace.race_name
                            }).ToList();
            }


            return ancestry;
        }

        public List<Hospital> getPatientHospitalList(Guid patientId)
        {

            List<Hospital> patientHospitals = null;

            patientHospitals = (

                        from memhosref in _icmsContext.MemberHospitalReferences

                        join hosp in _icmsContext.Hospitals
                        on memhosref.hospital_id equals hosp.hospital_id

                        where memhosref.member_id.Equals(patientId)
                        select hosp
                    )
                    .Distinct()
                    .ToList();

            return patientHospitals;
        }

        public List<IcmsUser> getPatientCaseOwnerList(Guid patientId)
        {

            List<IcmsUser> patientCaseOwners = null;

            patientCaseOwners = (

                        from caseOwnrs in _icmsContext.CaseOwners

                        join sysUsr in _icmsContext.SystemUsers
                        on caseOwnrs.system_user_id equals sysUsr.system_user_id

                        join caseTyp in _icmsContext.CaseTypes 
                        on caseOwnrs.case_type_code equals caseTyp.case_type_code into caseTyps
                        from caseTypes in caseTyps.DefaultIfEmpty()

                        where caseOwnrs.member_id.Equals(patientId)
                        select new IcmsUser
                        {
                            UserId = caseOwnrs.system_user_id,
                            caseOwnerId = caseOwnrs.case_owner_id,
                            FirstName = sysUsr.system_user_first_name,
                            LastName = sysUsr.system_user_last_name,
                            FullName = sysUsr.system_user_first_name + " " + sysUsr.system_user_last_name,
                            caseTypeCode = caseOwnrs.case_type_code,
                            caseTypeDescr = caseTypes.case_type_descr,
                            caseAssignedDate = caseOwnrs.assigned_date,
                            caseTerminationDate = caseOwnrs.discharge_date
                        }
                    )
                    .Distinct()
                    .OrderByDescending(assignDte => assignDte.caseAssignedDate)
                    .ToList();

            return patientCaseOwners;
        }

        public List<MemberAddress> GetPatientAddress(string id, bool returnOneAddress)
        {
            List<MemberAddress> addresses = null;

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                addresses = (
                    
                             from memadd in _icmsContext.MemberAddresses
                             where memadd.member_id.Equals(guidMemId)
                             //&& (memadd.is_alternate.Equals(false) || memadd.is_alternate.Equals(null))
                             orderby memadd.member_address_id descending
                             select memadd
                           )
                           .ToList();

                if (addresses != null && addresses.Count > 0 && returnOneAddress)
                {
                    addresses.Take(1);
                }
            }

            return addresses;
        }

        public Patient getPatientAddressByAddressId(int addressId)
        {

            Patient pat = null;            

            if (addressId > 0)
            {

                MemberAddress patientAddress = null;

                patientAddress = (

                             from memadd in _icmsContext.MemberAddresses
                             where memadd.member_address_id.Equals(addressId)
                             select memadd
                           )
                           .FirstOrDefault();

                if (patientAddress != null)
                {

                    pat = new Patient();
                    pat.addresses = new List<MemberAddress>();
                    pat.addresses.Add(patientAddress);
                }
            }

            return pat;
        }

        public List<PhoneNumber> GetPatientContactNumbers(string id)
        {
            List<PhoneNumber> numbers = null;

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                numbers = (
                    
                    from memphone in _icmsContext.MemberPhoneNumbers
                    join phonetype in _icmsContext.PhoneTypes
                    on memphone.phone_type_id equals phonetype.phone_type_id
                    where memphone.member_id.Equals(guidMemId)
                    orderby memphone.phone_type_id, memphone.member_phone_id descending
                    select new PhoneNumber
                    {
                        phoneId = memphone.member_phone_id,
                        PatientId = memphone.member_id,
                        Number = memphone.phone_number,
                        Type = phonetype.label
                    }
                )
                .ToList();
            }


            return numbers;
        }

        public PhoneNumber GetPatientHomePhoneNumber(string id, bool returnOneNumber)
        {
            PhoneNumber homeNumber = new PhoneNumber();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                homeNumber = (from memphone in _icmsContext.MemberPhoneNumbers
                              join phonetype in _icmsContext.PhoneTypes
                              on memphone.phone_type_id equals phonetype.phone_type_id
                              where memphone.member_id.Equals(guidMemId)
                              && memphone.phone_type_id.Equals(1)
                              orderby memphone.phone_type_id, memphone.member_phone_id descending
                              select new PhoneNumber
                              {
                                  PatientId = memphone.member_id,
                                  Number = memphone.phone_number,
                                  Type = phonetype.label
                              })
                                 .Take(1)
                                 .SingleOrDefault();
            }


            return homeNumber;
        }

        public PhoneNumber GetPatientEveningPhoneNumber(string id, bool returnOneNumber)
        {
            PhoneNumber homeNumber = new PhoneNumber();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                homeNumber = (from memphone in _icmsContext.MemberPhoneNumbers
                              join phonetype in _icmsContext.PhoneTypes
                              on memphone.phone_type_id equals phonetype.phone_type_id
                              where memphone.member_id.Equals(guidMemId)
                              && memphone.phone_type_id.Equals(2)
                              orderby memphone.phone_type_id, memphone.member_phone_id descending
                              select new PhoneNumber
                              {
                                  PatientId = memphone.member_id,
                                  Number = memphone.phone_number,
                                  Type = phonetype.label
                              })
                                 .Take(1)
                                 .SingleOrDefault();
            }


            return homeNumber;
        }

        public Isolation getPatientIsolation(Guid id, bool returnOneIsolation)
        {
            Isolation isolation = null;

            return isolation;
        }

        public Patient GetPatientInsuranceUsingId(string id)
        {
            Patient insurance = new Patient();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                insurance = (from memIns in _icmsContext.MemberInsurances
                             join insRel in _icmsContext.InsuranceRelationships
                             on memIns.insurance_relationship_id equals insRel.insurance_relationship_id
                             where memIns.member_id.Equals(guidMemId)
                             orderby memIns.self_pay, memIns.is_medicaid, memIns.is_medicare
                             select new Patient
                             {
                                 PatientId = memIns.member_id,
                                 insuranceId = memIns.member_insurance_id,
                                 InsuranceName = memIns.insurance_name,
                                 selfPay = memIns.self_pay,
                                 unInsured = memIns.uninsured,
                                 isMedicaid = memIns.is_medicaid,
                                 isMedicare = memIns.is_medicare,
                                 insuranceMemberId = memIns.insurance_id,
                                 insuranceSubscriberFirstName = memIns.subscriber_first_name,
                                 insuranceSubscriberLastName = memIns.subscriber_last_name,
                                 insuranceRelationshipId = memIns.insurance_relationship_id,
                                 insuranceRelationshipToPatient = insRel.relationship_name
                             })
                                 .Take(1)
                                 .SingleOrDefault();
            }

            return insurance;
        }

        public Note GetPatientMemberNotes(string id, string date)
        {
            List<Note> memberNote = new List<Note>();


            Guid memberId = Guid.Empty;
            DateTime recordDate = DateTime.MinValue;

            if (Guid.TryParse(id, out memberId) && DateTime.TryParse(date, out recordDate))
            {
                memberNote = (
                                from note in _icmsContext.MemberNotes
                                where note.member_id.Equals(memberId)
                                && note.record_date.Equals(recordDate.ToLongDateString())
                                select new Note
                                {
                                    noteText = note.evaluation_text,
                                    noteSequenceNumber = note.record_seq_num
                                }
                                )
                                .OrderBy(seq => seq.noteSequenceNumber)
                                .ToList();
            }


            Note returnNote = new Note();


            if (memberNote.Count > 0)
            {
                foreach (var nte in memberNote)
                {
                    returnNote.noteText += nte.noteText;
                }


                returnNote.memberId = memberId;
                returnNote.recordDate = recordDate;
            }


            return returnNote;
        }

        public Allergy getPatientAllergies(Guid memberId)
        {
            Allergy allergies = null;

            Allergy inpatientAllergies = getLastAdmissionAllergyData(memberId);

            if (inpatientAllergies != null)
            {
                allergies = inpatientAllergies;
                return allergies;
            }

            Allergy memberAllergies = getMemberAllergies(memberId);

            if (memberAllergies != null)
            {
                allergies = memberAllergies;
                return allergies;
            }


            return allergies;

        }

        private Allergy getLastAdmissionAllergyData(Guid memberId)
        {
            Allergy allergies = null;

            HospitalInpatientAdmissionAllergies inptAllergies = null;

            inptAllergies = (
                                from inpt in _icmsContext.HospitalInpatientAdmissions

                                join hospAlrg in _icmsContext.HospitalInpatientAdmissionAllergys
                                on inpt.hospital_inpatient_admission_id equals hospAlrg.hospital_inpatient_admission_id

                                where inpt.member_id.Equals(memberId)

                                orderby inpt.registered_date descending

                                select hospAlrg
                            )
                            .Take(1)
                            .FirstOrDefault();

            if (inptAllergies != null)
            {
                allergies = new Allergy();

                allergies.allergyId = inptAllergies.hospital_inpatient_admission_allergies_id;
                allergies.echinaceaAllergy = (bool)inptAllergies.echinacea;
                allergies.ephedraAllergy = (bool)inptAllergies.ephedra;
                allergies.garlicAllergy = (bool)inptAllergies.garlic;
                allergies.gingkoBilobaAllergy = (bool)inptAllergies.gingko_biloba;
                allergies.ginkgoAllergy = (bool)inptAllergies.ginkgo;
                allergies.ginsengAllergy = (bool)inptAllergies.ginseng;
                allergies.kavaAllergy = (bool)inptAllergies.kava;
                allergies.latexAllergy = (bool)inptAllergies.latex_allergy;
                allergies.stJohnsWortAllergy = (bool)inptAllergies.st_johns_wort;
                allergies.valerianAllergy = (bool)inptAllergies.valerian;
                allergies.valerianRootAllergy = (bool)inptAllergies.valerian_root;
                allergies.viteAllergy = (bool)inptAllergies.vite;

                allergies.medicationAllergy = inptAllergies.medication_allergy;
                allergies.otherAllergies = inptAllergies.other_allergy;
            }

            return allergies;

        }

        private Allergy getMemberAllergies(Guid memberId)

        {
            Allergy allergies = null;

            //HospitalInpatientAdmissionAllergies memberAllergy = null;

            //memberAllergy = (
            //                    from inpt in _icmsContext.HospitalInpatientAdmissions

            //                    join hospAlrg in _icmsContext.HospitalInpatientAdmissionAllergys
            //                    on inpt.hospital_inpatient_admission_id equals hospAlrg.hospital_inpatient_admission_id

            //                    where inpt.member_id.Equals(memberId)

            //                    orderby inpt.registered_date descending

            //                    select hospAlrg
            //                )
            //                .Take(1)
            //                .FirstOrDefault();

            //if (memberAllergy != null)
            //{
            //    allergies = new Allergy();
            //    allergies.echinaceaAllergy = (bool)memberAllergy.echinacea;
            //    allergies.ephedraAllergy = (bool)memberAllergy.ephedra;
            //    allergies.garlicAllergy = (bool)memberAllergy.garlic;
            //    allergies.gingkoBilobaAllergy = (bool)memberAllergy.gingko_biloba;
            //    allergies.ginkgoAllergy = (bool)memberAllergy.ginkgo;
            //    allergies.ginsengAllergy = (bool)memberAllergy.ginseng;
            //    allergies.kavaAllergy = (bool)memberAllergy.kava;
            //    allergies.latexAllergy = (bool)memberAllergy.latex_allergy;
            //    allergies.medicationAllergy = memberAllergy.medication_allergy;
            //    allergies.otherAllergies = memberAllergy.other_allergy;
            //    allergies.stJohnsWortAllergy = (bool)memberAllergy.st_johns_wort;
            //    allergies.valerianAllergy = (bool)memberAllergy.valerian;
            //    allergies.valerianRootAllergy = (bool)memberAllergy.valerian_root;
            //    allergies.viteAllergy = (bool)memberAllergy.vite;
            //}

            return allergies;

        }

        public AdvancedDirective getPatientAdvancedDirectives(Guid patientId)
        {
            AdvancedDirective directives = null;

            try
            {

                tblAdvanceDirectives advDirects = (
                                                    from dircts in _emrContext.AdvancedDirectives
                                                    where dircts.member_id.Equals(patientId)
                                                    select dircts
                                                  )
                                                  .FirstOrDefault();

                if (advDirects != null)
                {
                    directives = new AdvancedDirective();
                    directives.advanceDirectiveId = advDirects.entryId;
                    directives.hasMedicalDeclarationStatements = advDirects.Declaration;
                    directives.DoNotResuscitatePatient = advDirects.DNR;
                    directives.hasPowerOfAttorneyDocument = advDirects.PowerOfAttourney;
                }

                return directives;

            }
            catch (Exception ex)
            {
                return directives;
            }
        }

        public List<Admission> getPatientPastAdmissions(Guid patientId, int numberOfAdmitsToReturn)
        {
            List<Admission> pastAdmissions = null;

            pastAdmissions = (
                                from hospadmits in _icmsContext.HospitalInpatientAdmissions
                                where hospadmits.member_id.Equals(patientId)

                                join admitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                                on hospadmits.hospital_inpatient_admission_id equals admitreason.hospital_inpatient_admission_id into reasons
                                from hospadmitreasons in reasons.DefaultIfEmpty()

                                select new Admission
                                {
                                    admitDate = hospadmits.registered_date,
                                    displayAdmitDate = (hospadmits.registered_date.HasValue) ? hospadmits.registered_date.Value.ToShortDateString() : "",
                                    dischargeDate = hospadmits.discharged_date,
                                    displayDischargeDate = (hospadmits.discharged_date.HasValue) ? hospadmits.discharged_date.Value.ToShortDateString() : "",
                                    registration_number = hospadmits.registration_number,
                                    reasonForVisit = hospadmitreasons.reason_for_visit
                                }
                              )
                              .ToList();

            if (numberOfAdmitsToReturn > 0)
            {
                pastAdmissions = pastAdmissions.Take(numberOfAdmitsToReturn).ToList();
            }

            return pastAdmissions;
        }

        public Patient addPatientForInpatient(Patient patient)
        {
            Patient returnPatient = null;

            try
            {

                DateTime dob = DateTime.MinValue;

                if (!string.IsNullOrEmpty(patient.firstName) &&
                    !string.IsNullOrEmpty(patient.lastName) &&
                    (!string.IsNullOrEmpty(patient.dateOfBirthDisplay) && DateTime.TryParse(patient.dateOfBirthDisplay, out dob)))
                {

                    Member dbPatient = null;

                    if (!string.IsNullOrEmpty(patient.ssn))
                    {

                        dbPatient = (
                                        from pat in _icmsContext.Patients
                                        where pat.member_first_name.Equals(patient.firstName)
                                        && pat.member_last_name.Equals(patient.lastName)
                                        && pat.member_birth.Equals(dob)
                                        && pat.member_ssn.Equals(patient.ssn)
                                        select pat
                                    )
                                    .FirstOrDefault();
                    }
                    else
                    {

                        dbPatient = (
                                        from pat in _icmsContext.Patients
                                        where pat.member_first_name.Equals(patient.firstName)
                                        && pat.member_last_name.Equals(patient.lastName)
                                        && pat.member_birth.Equals(dob)
                                        select pat
                                    )
                                    .FirstOrDefault();
                    }

                    if (dbPatient == null)
                    {
                        dbPatient = new Member();
                        dbPatient.member_first_name = patient.firstName;
                        dbPatient.member_last_name = patient.lastName;
                        dbPatient.member_birth = dob;
                        dbPatient.member_ssn = patient.ssn;
                        dbPatient.gender_code = patient.gender;
                        dbPatient.member_email = patient.emailAddress;

                        _icmsContext.Patients.Add(dbPatient);
                        int results = _icmsContext.SaveChanges();

                        if (results > 0)
                        {
                            patient.PatientId = dbPatient.member_id;

                            addPatientEnrollmentForInpatient(patient);
                            addPatientAddressForInpatient(patient);
                            addPatientPhoneForInpatient(patient);
                            addPatientHealthPlanReferenceForInpatient(patient);
                            addPatientNextOfKinForInpatient(patient);

                            returnPatient = getPatientDemographics(patient.PatientId.ToString());
                        }
                    }
                }

                return returnPatient;

            }
            catch(Exception ex)
            {
                return returnPatient;
            }
        }

        public PatientAsset getPatientDashboardAssets()
        {

            PatientAsset patAsset = null;

            StandardService standServ = new StandardService(_icmsContext, _emrContext);

            List<HospitalMaritalStatusTypes> maritalStatus = standServ.getMaritalStatuses();
            List<Employer> employers = standServ.getDbmsEmployers();
            List<State> states = standServ.GetStatesList();
            List<PhoneType> phoneTypes = standServ.getPhoneTypes();
            List<HospitalRace> races = standServ.getHospitalRaces();
            List<Hospital> hospitals = standServ.getHospitals();
            List<Languages> languages = standServ.getLanguages();
            List<NewlyIdentifiedCmMemberCaseStatus> cmCaseStatus = standServ.getCmCaseStatus();
            List<IcmsUser> caseOwners = standServ.getCmCaseOwners();
            List<CaseType> caseTypes = standServ.getCmCaseTypes();
            

            patAsset = new PatientAsset();
            patAsset.maritalStatuses = maritalStatus;
            patAsset.employers = employers;
            patAsset.states = states;
            patAsset.phoneTypes = phoneTypes;
            patAsset.races = races;
            patAsset.hospitals = hospitals;
            patAsset.languages = languages;
            patAsset.cmCaseStatus = cmCaseStatus;
            patAsset.caseOwners = caseOwners;
            patAsset.caseTypes = caseTypes;

            return patAsset;
        }       

        private string getPatientEmployerName(Guid memberId)
        {

            string employerName = (

                    from memEnroll in _icmsContext.MemberEnrollments

                    join employ in _icmsContext.Employers
                    on memEnroll.employer_id equals employ.employer_id

                    where memEnroll.member_id.Equals(memberId)

                    select employ.employer_name
                )
                .FirstOrDefault();

            return employerName;
        }







        private void addPatientEnrollmentForInpatient(Patient patient)
        {
            MemberEnrollment patEnroll = null;

            if (!patient.PatientId.Equals(Guid.Empty))
            {

                patEnroll = (
                                from memEnroll in _icmsContext.MemberEnrollments
                                where memEnroll.member_id.Equals(patient.PatientId)
                                select memEnroll
                            )
                            .FirstOrDefault();

                if (patEnroll == null)
                {
                    patEnroll = new MemberEnrollment();
                    patEnroll.member_id = (Guid)patient.PatientId;
                    patEnroll.claims_system_id = 8;
                    patEnroll.claims_id = DateTime.Now.ToShortDateString() + "_" + patient.lastName;
                    patEnroll.member_effective_date = DateTime.Now;
                    patEnroll.client_id = 705;
                    patEnroll.client_bu_id = 1050;
                    patEnroll.manual_entry_flag = false;
                    patEnroll.user_updated = patient.usr;
                    patEnroll.date_updated = DateTime.Now;

                    _icmsContext.MemberEnrollments.Add(patEnroll);
                    int result = _icmsContext.SaveChanges();
                }
            }
        }

        private void addPatientAddressForInpatient(Patient patient)
        {
            if (patient.addresses != null && patient.addresses.ToList().Count > 0)
            {
                if (!patient.PatientId.Equals(Guid.Empty))
                {
                
                    MemberAddress patAddr = null;

                    patAddr = (
                                    from memAddrs in _icmsContext.MemberAddresses
                                    where memAddrs.member_id.Equals(patient.PatientId)
                                    select memAddrs
                              )
                              .FirstOrDefault();

                    if (patAddr == null)
                    {
                        MemberAddress patientAddress = patient.addresses.First();

                        patAddr = new MemberAddress();
                        patAddr.member_id = (Guid)patient.PatientId;
                        patAddr.address_line1 = patientAddress.address_line1;
                        patAddr.address_line2 = patientAddress.address_line2;
                        patAddr.zip_code = patientAddress.zip_code;
                        patAddr.address_type_id = 1;

                        _icmsContext.MemberAddresses.Add(patAddr);
                        int result = _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addPatientPhoneForInpatient(Patient patient)
        {
            if (patient.homePhoneNumber.Number != null)
            {
                if (!patient.PatientId.Equals(Guid.Empty))
                {

                    MemberPhone patPhn = null;

                    patPhn = (
                                    from memPhn in _icmsContext.MemberPhoneNumbers
                                    where memPhn.member_id.Equals(patient.PatientId)
                                    select memPhn
                              )
                              .FirstOrDefault();

                    if (patPhn == null)
                    {
                        patPhn = new MemberPhone();
                        patPhn.member_id = (Guid)patient.PatientId;
                        patPhn.phone_type_id = 2;
                        patPhn.phone_number = patient.homePhoneNumber.Number;

                        _icmsContext.MemberPhoneNumbers.Add(patPhn);
                        int result = _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addPatientHealthPlanReferenceForInpatient(Patient patient)
        {
            if (!string.IsNullOrEmpty(patient.mrn) || !string.IsNullOrEmpty(patient.hospitalNo))
            {
                if (!patient.PatientId.Equals(Guid.Empty))
                {

                    MemberHealthPlanReference patHlthPlnRef = null;

                    patHlthPlnRef = (
                                        from memHltPlnRef in _icmsContext.MemberHealthPlanReferences
                                        where memHltPlnRef.member_id.Equals(patient.PatientId)
                                        select memHltPlnRef
                                    )
                                    .FirstOrDefault();

                    if (patHlthPlnRef == null)
                    {
                        patHlthPlnRef = new MemberHealthPlanReference();
                        patHlthPlnRef.member_id = (Guid)patient.PatientId;
                        patHlthPlnRef.mrn = patient.mrn;
                        patHlthPlnRef.hospital_number = patient.hospitalNo;

                        _icmsContext.MemberHealthPlanReferences.Add(patHlthPlnRef);
                        int result = _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addPatientNextOfKinForInpatient(Patient patient)
        {
            if (patient.nextOfKin != null)
            {

                MemberNextOfKin nxtOfKin = null;

                if (!patient.PatientId.Equals(Guid.Empty))
                {

                    nxtOfKin = (
                                    from nxtKin in _icmsContext.MemberNextOfKins
                                    where nxtKin.member_id.Equals(patient.PatientId)
                                    select nxtKin
                               )
                               .FirstOrDefault();

                    if (nxtOfKin == null)
                    {
                        nxtOfKin = new MemberNextOfKin();
                        nxtOfKin.member_id = (Guid)patient.PatientId;
                        nxtOfKin.last_name = patient.nextOfKin.lastName;
                        nxtOfKin.first_name = patient.nextOfKin.firstName;
                        nxtOfKin.sims_er_relationship_id = patient.nextOfKin.relationshipId;
                        nxtOfKin.phone_number = patient.nextOfKin.phoneNumber;

                        _icmsContext.MemberNextOfKins.Add(nxtOfKin);
                        int result = _icmsContext.SaveChanges();
                    }
                }
            }
        }

        
        public List<Patient> patientBasicExists(Patient patient)
        {

            List<Patient> returnPatients = null;

            try
            {

                DateTime dob = DateTime.MinValue;

                if (!string.IsNullOrEmpty(patient.firstName) &&
                    !string.IsNullOrEmpty(patient.lastName) &&
                    (!string.IsNullOrEmpty(patient.dateOfBirthDisplay) && DateTime.TryParse(patient.dateOfBirthDisplay, out dob)))
                {

                    List<Member> dbPatients = null;

                    string ssn = (string.IsNullOrEmpty(patient.ssn)) ? null : patient.ssn;

                    if (!string.IsNullOrEmpty(ssn))
                    {

                        dbPatients = (
                                        from pat in _icmsContext.Patients
                                        where pat.member_first_name.Equals(patient.firstName)
                                        && pat.member_last_name.Equals(patient.lastName)
                                        && pat.member_birth.Equals(dob)
                                        && pat.member_ssn.Equals(ssn)
                                        select pat
                                    )
                                    .ToList();
                    }
                    else
                    {

                        dbPatients = (
                                        from pat in _icmsContext.Patients
                                        where pat.member_first_name.Equals(patient.firstName)
                                        && pat.member_last_name.Equals(patient.lastName)
                                        && pat.member_birth.Equals(dob)
                                        select pat
                                    )
                                    .ToList();
                    }


                    if (dbPatients != null && dbPatients.Count > 0)
                    {

                        returnPatients = new List<Patient>();

                        foreach (Member matchingPatient in dbPatients)
                        {

                            Patient addPatient = getPatientDemographics(matchingPatient.member_id.ToString());
                            returnPatients.Add(addPatient);
                        }
                    }
                }


                return returnPatients;

            }
            catch (Exception ex)
            {
                return returnPatients;
            }
        }


        public Patient addPatientBasic(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                DateTime dob = DateTime.MinValue;

                if (!string.IsNullOrEmpty(addPatient.firstName) &&
                    !string.IsNullOrEmpty(addPatient.lastName) &&
                    (!string.IsNullOrEmpty(addPatient.dateOfBirthDisplay) && DateTime.TryParse(addPatient.dateOfBirthDisplay, out dob)))
                {

                    Member dbPatient = null;

                    string ssn = (string.IsNullOrEmpty(addPatient.ssn)) ? generateSsn() : addPatient.ssn;

                    if (!string.IsNullOrEmpty(ssn))
                    {

                        dbPatient = (
                                        from pat in _icmsContext.Patients
                                        where pat.member_first_name.Equals(addPatient.firstName)
                                        && pat.member_last_name.Equals(addPatient.lastName)
                                        && pat.member_birth.Equals(dob)
                                        && pat.member_ssn.Equals(ssn)
                                        select pat
                                    )
                                    .FirstOrDefault();


                        if (dbPatient == null)
                        {

                            dbPatient = new Member();
                            dbPatient.member_first_name = addPatient.firstName;
                            dbPatient.member_last_name = addPatient.lastName;
                            dbPatient.member_birth = dob;
                            dbPatient.member_ssn = ssn;

                            _icmsContext.Patients.Add(dbPatient);
                            int results = _icmsContext.SaveChanges();

                            if (results > 0)
                            {
                                addPatient.PatientId = dbPatient.member_id;

                                addPatientEnrollmentForInpatient(addPatient);

                                returnPatient = getPatientDemographics(addPatient.PatientId.ToString());
                            }
                        }
                    }
                }

                    

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }


        public Patient updatePatientInfo(Patient updatePatient)
        {

            Patient returnPatient = null;

            try
            {

                DateTime dob = DateTime.MinValue;

                if (!updatePatient.PatientId.Equals(Guid.Empty) && 
                    !updatePatient.usr.Equals(Guid.Empty) &&
                    (!string.IsNullOrEmpty(updatePatient.dateOfBirthDisplay) && DateTime.TryParse(updatePatient.dateOfBirthDisplay, out dob)))
                {

                    Member dbPatient = null;

                    dbPatient = (
                                    from pat in _icmsContext.Patients
                                    where pat.member_id.Equals(updatePatient.PatientId)
                                    select pat
                                )
                                .FirstOrDefault();


                    if (dbPatient != null)
                    {

                        dbPatient.member_first_name = updatePatient.firstName;
                        dbPatient.member_middle_name = updatePatient.middleName;
                        dbPatient.member_last_name = updatePatient.lastName;
                        dbPatient.member_birth = dob;
                        dbPatient.member_ssn = updatePatient.ssn;
                        dbPatient.gender_code = (!string.IsNullOrEmpty(updatePatient.gender)) ? updatePatient.gender : "";
                        dbPatient.member_medicare_num = updatePatient.medicareNumber;
                        dbPatient.member_medicaid_num = updatePatient.medicaidNumber;
                        dbPatient.hospital_marital_status_types_id = updatePatient.maritalStatusId;
                        dbPatient.languages_id = updatePatient.primaryLanguageId;

                        _icmsContext.Patients.Update(dbPatient);
                        int results = _icmsContext.SaveChanges();

                        if (results > 0)
                        {
                            updatePatientRaces(updatePatient);
                            returnPatient = getPatientDemographics(updatePatient.PatientId.ToString());
                        }
                    }
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient updatePatientEmail(Patient updatePatient)
        {

            Patient returnPatient = null;

            try
            {

                if (!updatePatient.PatientId.Equals(Guid.Empty) &&
                    !updatePatient.usr.Equals(Guid.Empty))
                {

                    Member dbPatient = null;

                    dbPatient = (
                                    from pat in _icmsContext.Patients
                                    where pat.member_id.Equals(updatePatient.PatientId)
                                    select pat
                                )
                                .FirstOrDefault();


                    if (dbPatient != null)
                    {

                        dbPatient.member_email = updatePatient.emailAddress;

                        _icmsContext.Patients.Update(dbPatient);
                        int results = _icmsContext.SaveChanges();

                        if (results > 0)
                        {
                            returnPatient =  getPatientDemographics(updatePatient.PatientId.ToString());
                        }
                    }
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }


        public Patient addPatientAddress(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                DateTime dob = DateTime.MinValue;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty))
                {

                    List<MemberAddress> dbAddresses = null;

                    bool add = true;

                    dbAddresses = (
                                
                                    from patAddr in _icmsContext.MemberAddresses
                                    where patAddr.member_id.Equals(addPatient.PatientId)
                                    select patAddr
                                )
                                .ToList();


                    if (dbAddresses != null)
                    {
                       
                        foreach(MemberAddress patientAddress in dbAddresses)
                        {

                            if (patientAddress.address_line1.Trim().ToLower().Equals(addPatient.addresses[0].address_line1.Trim().ToLower()))
                            {
                                if (patientAddress.address_line2.Trim().ToLower().Equals(addPatient.addresses[0].address_line2.Trim().ToLower()))
                                {
                                    if (patientAddress.city.Trim().ToLower().Equals(addPatient.addresses[0].city.Trim().ToLower()))
                                    {
                                        if (patientAddress.state_abbrev.Trim().ToLower().Equals(addPatient.addresses[0].state_abbrev.Trim().ToLower()))
                                        {
                                            if (patientAddress.zip_code.Trim().ToLower().Equals(addPatient.addresses[0].zip_code.Trim().ToLower()))
                                            {
                                                add = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (add)
                    {

                        MemberAddress addAddress = new MemberAddress();
                        addAddress.member_id = (Guid)addPatient.PatientId;
                        addAddress.address_line1 = addPatient.addresses[0].address_line1;
                        addAddress.address_line2 = addPatient.addresses[0].address_line2;
                        addAddress.address_type_id = 1;
                        addAddress.city = addPatient.addresses[0].city;
                        addAddress.state_abbrev = addPatient.addresses[0].state_abbrev;
                        addAddress.zip_code = addPatient.addresses[0].zip_code;
                        addAddress.is_alternate = addPatient.addresses[0].is_alternate;

                        _icmsContext.MemberAddresses.Add(addAddress);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.addresses = GetPatientAddress(addPatient.PatientId.ToString(), false);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient removePatientAddress(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty) &&
                    (addPatient.addresses != null && addPatient.addresses[0].member_address_id > 0))
                {

                    MemberAddress dbAddress = null;

                    dbAddress = (

                                    from patAddr in _icmsContext.MemberAddresses
                                    where patAddr.member_address_id.Equals(addPatient.addresses[0].member_address_id)
                                    && patAddr.member_id.Equals(addPatient.PatientId)
                                    select patAddr
                                )
                                .FirstOrDefault();


                    if (dbAddress != null)
                    {

                        _icmsContext.MemberAddresses.Remove(dbAddress);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.addresses = GetPatientAddress(addPatient.PatientId.ToString(), false);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient updatePatientAddress(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                if (addPatient.addresses != null && 
                    addPatient.addresses[0].member_address_id > 0)
                {

                    MemberAddress dbAddress = null;

                    dbAddress = (

                                    from patAddr in _icmsContext.MemberAddresses
                                    where patAddr.member_address_id.Equals(addPatient.addresses[0].member_address_id)
                                    select patAddr
                                )
                                .FirstOrDefault();


                    if (dbAddress != null)
                    {

                        dbAddress.address_line1 = addPatient.addresses[0].address_line1;
                        dbAddress.address_line2 = addPatient.addresses[0].address_line2;
                        dbAddress.address_type_id = 1;
                        dbAddress.city = addPatient.addresses[0].city;
                        dbAddress.state_abbrev = addPatient.addresses[0].state_abbrev;
                        dbAddress.zip_code = addPatient.addresses[0].zip_code;
                        dbAddress.is_alternate = addPatient.addresses[0].is_alternate;

                        _icmsContext.MemberAddresses.Update(dbAddress);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.addresses = GetPatientAddress(addPatient.PatientId.ToString(), false);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }



        public Patient addPatientPhone(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                DateTime dob = DateTime.MinValue;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty))
                {

                    List<MemberPhone> dbPhones = null;

                    bool add = true;

                    dbPhones = (

                                    from patPhn in _icmsContext.MemberPhoneNumbers
                                    where patPhn.member_id.Equals(addPatient.PatientId)
                                    select patPhn
                                )
                                .ToList();


                    if (dbPhones != null)
                    {

                        foreach (MemberPhone patientPhone in dbPhones)
                        {

                            if (patientPhone.phone_number.Trim().ToLower().Equals(addPatient.contactNumbers[0].Number.Trim().ToLower()))
                            {
                                if (patientPhone.phone_type_id.Equals(addPatient.contactNumbers[0].phoneTypeId))
                                {
                                    add = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (add)
                    {

                        MemberPhone addPhone = new MemberPhone();
                        addPhone.member_id = (Guid)addPatient.PatientId;
                        addPhone.phone_number = addPatient.contactNumbers[0].Number;
                        addPhone.phone_type_id = addPatient.contactNumbers[0].phoneTypeId;

                        _icmsContext.MemberPhoneNumbers.Add(addPhone);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.contactNumbers = GetPatientContactNumbers(addPatient.PatientId.ToString());
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient removePatientPhone(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty) &&
                    (addPatient.contactNumbers != null && addPatient.contactNumbers[0].phoneId > 0))
                {

                    MemberPhone dbPhone = null;

                    dbPhone = (

                                    from patAddr in _icmsContext.MemberPhoneNumbers
                                    where patAddr.member_phone_id.Equals(addPatient.contactNumbers[0].phoneId)
                                    && patAddr.member_id.Equals(addPatient.PatientId)
                                    select patAddr
                                )
                                .FirstOrDefault();


                    if (dbPhone != null)
                    {

                        _icmsContext.MemberPhoneNumbers.Remove(dbPhone);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.contactNumbers = GetPatientContactNumbers(addPatient.PatientId.ToString());
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }



        public Patient updatePatientEmployer(Patient updatePatient)
        {

            Patient returnPatient = null;

            try
            {

                if (!updatePatient.PatientId.Equals(Guid.Empty) &&
                    !updatePatient.usr.Equals(Guid.Empty))
                {

                    MemberEnrollment dbMemEnroll = null;

                    dbMemEnroll = (
                                    from memEnroll in _icmsContext.MemberEnrollments
                                    where memEnroll.member_id.Equals(updatePatient.PatientId)
                                    select memEnroll
                                )
                                .FirstOrDefault();


                    if (dbMemEnroll != null)
                    {

                        dbMemEnroll.employer_id = updatePatient.employerId;
                        dbMemEnroll.member_effective_date = updatePatient.effectiveDate;
                        dbMemEnroll.member_disenroll_date = updatePatient.disenrollDate;

                        _icmsContext.MemberEnrollments.Update(dbMemEnroll);
                        int results = _icmsContext.SaveChanges();

                        if (results > 0)
                        {
                            returnPatient = getPatientDemographics(updatePatient.PatientId.ToString());
                        }
                    }
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }



        public Patient updatePatientInsurance(Patient updatePatient)
        {

            Patient returnPatient = null;

            try
            {

                if (!updatePatient.PatientId.Equals(Guid.Empty) &&
                    !updatePatient.usr.Equals(Guid.Empty))
                {

                    Patient patientInsurance = saveInsurance(updatePatient);
                    Patient patientEnroll = saveEgpMemberId(updatePatient);

                    if (patientInsurance != null || patientEnroll != null)
                    {
                        returnPatient = setPatientInsurance(patientInsurance, patientEnroll);                        
                    }
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }
        private Patient saveInsurance(Patient updatePatient)
        {

            Patient patientInsurance = null;

            MemberInsurance dbInsurance = null;

            dbInsurance = (
                            from memInsur in _icmsContext.MemberInsurances
                            where memInsur.member_id.Equals(updatePatient.PatientId)
                            select memInsur
                        )
                        .FirstOrDefault();


            bool getInsurance = false;

            if (dbInsurance != null)
            {
                getInsurance = updateInsurance(dbInsurance, updatePatient);
            } 
            else
            {
                getInsurance = addInsurance(updatePatient);
            }

            if (getInsurance)
            {
                patientInsurance = GetPatientInsuranceUsingId(updatePatient.PatientId.ToString());
            }

            return patientInsurance;
        }
        private bool addInsurance(Patient updatePatient)
        {

            MemberInsurance addInsurance = new MemberInsurance();

            addInsurance.member_id = updatePatient.PatientId;
            addInsurance.self_pay = updatePatient.selfPay;
            addInsurance.uninsured = updatePatient.unInsured;
            addInsurance.creation_date = updatePatient.creationDate;
            addInsurance.creation_user_id = updatePatient.usr;

            _icmsContext.MemberInsurances.Add(addInsurance);
            int results = _icmsContext.SaveChanges();

            if (results > 0)
            {
                return true;
            }

            return false;
        }
        private bool updateInsurance(MemberInsurance dbInsurance, Patient updatePatient)
        {

            dbInsurance.self_pay = updatePatient.selfPay;
            dbInsurance.uninsured = updatePatient.unInsured;

            _icmsContext.MemberInsurances.Update(dbInsurance);
            int results = _icmsContext.SaveChanges();

            if (results > 0)
            {
                return true;
            }

            return false;
        }        
        private Patient saveEgpMemberId(Patient updatePatient)
        {

            Patient patientInsurance = null;

            MemberEnrollment dbEnroll = null;

            dbEnroll = (
                            from memEnroll in _icmsContext.MemberEnrollments
                            where memEnroll.member_id.Equals(updatePatient.PatientId)
                            select memEnroll
                        )
                        .FirstOrDefault();


            if (dbEnroll != null)
            {

                dbEnroll.egp_member_id = updatePatient.egpMemberId;

                _icmsContext.MemberEnrollments.Update(dbEnroll);
                int results = _icmsContext.SaveChanges();

                if (results > 0)
                {
                    patientInsurance = GetPatientEnrollmentUsingId(updatePatient.PatientId.ToString());
                }
            }

            return patientInsurance;
        }
        private Patient setPatientInsurance(Patient patientInsurance, Patient patientEnroll)
        {

            Patient returnPatient = new Patient();

            if (patientInsurance != null)
            {

                returnPatient.PatientId = patientInsurance.PatientId;
                returnPatient.insuranceId = patientInsurance.insuranceId;
                returnPatient.InsuranceName = patientInsurance.InsuranceName;
                returnPatient.selfPay = patientInsurance.selfPay;
                returnPatient.unInsured = patientInsurance.unInsured;
                returnPatient.isMedicaid = patientInsurance.isMedicaid;
                returnPatient.isMedicare = patientInsurance.isMedicare;
                returnPatient.insuranceMemberId = patientInsurance.insuranceMemberId;
                returnPatient.insuranceSubscriberFirstName = patientInsurance.insuranceSubscriberFirstName;
                returnPatient.insuranceSubscriberLastName = patientInsurance.insuranceSubscriberLastName;
                returnPatient.insuranceRelationshipId = patientInsurance.insuranceRelationshipId;
                returnPatient.insuranceRelationshipToPatient = patientInsurance.insuranceRelationshipToPatient;
            }

            if (patientEnroll != null)
            {

                if (returnPatient.PatientId.Equals(Guid.Empty))
                {
                    returnPatient.PatientId = patientEnroll.PatientId;
                }

                returnPatient.egpMemberId = patientEnroll.egpMemberId;
                returnPatient.claimsEnrollmentId = patientEnroll.claimsEnrollmentId;
            }

            return returnPatient;
        }



        public Patient addPatientHospital(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                DateTime dtNow = DateTime.Now;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty))
                {

                    List<MemberHospitalReference> dbMemHospRef = null;

                    bool add = true;

                    dbMemHospRef = (

                                    from patHosp in _icmsContext.MemberHospitalReferences
                                    where patHosp.member_id.Equals(addPatient.PatientId)
                                    select patHosp
                                )
                                .ToList();


                    if (dbMemHospRef != null)
                    {

                        foreach (MemberHospitalReference patientPhone in dbMemHospRef)
                        {

                            if (patientPhone.hospital_id.Equals(addPatient.hospitals[0].hospital_id))
                            {
                                add = false;
                                break;
                            }
                        }
                    }

                    if (add)
                    {

                        MemberHospitalReference addMemHospRef = new MemberHospitalReference();
                        addMemHospRef.member_id = (Guid)addPatient.PatientId;
                        addMemHospRef.hospital_id = addPatient.hospitals[0].hospital_id;
                        addMemHospRef.creation_date = dtNow;
                        addMemHospRef.creation_user_id = addPatient.usr;

                        _icmsContext.MemberHospitalReferences.Add(addMemHospRef);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.hospitals = getPatientHospitalList((Guid)addPatient.PatientId);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient removePatientHospital(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty) &&
                    (addPatient.hospitals != null && addPatient.hospitals[0].hospital_id > 0))
                {

                    MemberHospitalReference dbMemHospRef = null;

                    dbMemHospRef = (

                                    from patAddr in _icmsContext.MemberHospitalReferences
                                    where patAddr.hospital_id.Equals(addPatient.hospitals[0].hospital_id)
                                    && patAddr.member_id.Equals(addPatient.PatientId)
                                    select patAddr
                                )
                                .FirstOrDefault();


                    if (dbMemHospRef != null)
                    {

                        _icmsContext.MemberHospitalReferences.Remove(dbMemHospRef);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.hospitals = getPatientHospitalList((Guid)addPatient.PatientId);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }



        public Patient updatePatientCmIdentification(Patient updatePatient)
        {

            Patient returnPatient = null;

            try
            {

                if (!updatePatient.PatientId.Equals(Guid.Empty) &&
                    !updatePatient.usr.Equals(Guid.Empty))
                {

                    Member dbPatient = null;

                    dbPatient = (
                                    from pat in _icmsContext.Patients
                                    where pat.member_id.Equals(updatePatient.PatientId)
                                    select pat
                                )
                                .FirstOrDefault();


                    if (dbPatient != null)
                    {

                        dbPatient.member_in_lcm = (bool)updatePatient.inLcm;
                        dbPatient.newly_identified_cm_member_case_status_id = updatePatient.newlyIdentifiedCaseStatusIid;
                        dbPatient.newly_identified_cm_member_date_of_referral = updatePatient.newlyIdentifiedDateOfReferral;
                        dbPatient.newly_identified_cm_member_method_of_identification = updatePatient.newlyIdentifiedMethodOfIdentification;

                        _icmsContext.Patients.Update(dbPatient);
                        int results = _icmsContext.SaveChanges();

                        if (results > 0)
                        {
                            
                            updatePatientCmProgram(updatePatient);

                            returnPatient = getPatientDemographics(updatePatient.PatientId.ToString());
                        }
                    }
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }



        public Patient addPatientCaseOwner(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                DateTime dtNow = DateTime.Now;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty))
                {

                    List<CaseOwner> dbCaseOwners = null;

                    bool add = true;

                    dbCaseOwners = (

                                    from caseOwnr in _icmsContext.CaseOwners
                                    where caseOwnr.member_id.Equals(addPatient.PatientId)
                                    && !caseOwnr.discharge_date.HasValue
                                    select caseOwnr
                                )
                                .ToList();


                    if (dbCaseOwners != null)
                    {

                        foreach (CaseOwner patientCaseOwner in dbCaseOwners)
                        {

                            if (patientCaseOwner.system_user_id.Equals(addPatient.caseOwners[0].UserId))
                            {
                                add = false;
                                break;
                            }
                        }
                    }

                    if (add)
                    {


                        string caseTypeCode = getCaseOwnerCaseTypeCode(addPatient);

                        CaseOwner addCaseOwner = new CaseOwner();
                        addCaseOwner.member_id = (Guid)addPatient.PatientId;
                        addCaseOwner.system_user_id = addPatient.caseOwners[0].UserId;
                        addCaseOwner.assigned_date = dtNow;
                        addCaseOwner.case_type_code = caseTypeCode;

                        _icmsContext.CaseOwners.Add(addCaseOwner);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.caseOwners = getPatientCaseOwnerList((Guid)addPatient.PatientId);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }

        public Patient removePatientCaseOwner(Patient addPatient)
        {

            Patient returnPatient = null;

            try
            {

                int result = 0;

                if (!addPatient.PatientId.Equals(Guid.Empty) &&
                    !addPatient.usr.Equals(Guid.Empty) &&
                    (addPatient.caseOwners != null && addPatient.caseOwners[0].caseOwnerId > 0))
                {

                    CaseOwner dbCaseOwner = null;

                    dbCaseOwner = (

                                    from patAddr in _icmsContext.CaseOwners
                                    where patAddr.case_owner_id.Equals(addPatient.caseOwners[0].caseOwnerId)
                                    select patAddr
                                )
                                .FirstOrDefault();


                    if (dbCaseOwner != null)
                    {
                        
                        dbCaseOwner.discharge_date = DateTime.Now;

                        _icmsContext.CaseOwners.Update(dbCaseOwner);
                        result = _icmsContext.SaveChanges();
                    }
                }

                if (result > 0)
                {

                    returnPatient = new Patient();
                    returnPatient.caseOwners = getPatientCaseOwnerList((Guid)addPatient.PatientId);
                }

                return returnPatient;

            }
            catch (Exception ex)
            {
                return returnPatient;
            }
        }





        private void updatePatientCmProgram(Patient updatePatient)
        {

            MemberProgram dbPatPrograms = null;

            dbPatPrograms = (
                            from patProgs in _icmsContext.MemberPrograms
                            where patProgs.member_id.Equals(updatePatient.PatientId)
                            select patProgs
                        )
                        .FirstOrDefault();


            if (dbPatPrograms != null)
            {

                dbPatPrograms.lcm_optout = (bool)updatePatient.optOutLcm;
                dbPatPrograms.lcm_optout_date = updatePatient.optOutLcmDate;

                _icmsContext.MemberPrograms.Update(dbPatPrograms);
                _icmsContext.SaveChanges();
            } 
            else
            {

                MemberProgram newProgram = new MemberProgram();
                newProgram.member_id = (Guid)updatePatient.PatientId;
                newProgram.lcm_optout = updatePatient.optOutLcm;
                newProgram.lcm_optout_date = updatePatient.optOutLcmDate;

                _icmsContext.MemberPrograms.Add(newProgram);
                _icmsContext.SaveChanges();
            }
        }

        private void updatePatientRaces(Patient updatePatient)
        {

            try
            {

                List<MemberRace> dbPatientRaces = null;

                dbPatientRaces = (

                        from memberRace in _icmsContext.MemberRaces
                        where memberRace.member_id.Equals(updatePatient.PatientId)
                        select memberRace
                    )
                    .ToList();

                if (dbPatientRaces != null && dbPatientRaces.Count > 0)
                {
                    updateExisitingRaces(dbPatientRaces, updatePatient);                    
                } 
                else
                {
                    updateNewRaces(updatePatient);
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void updateExisitingRaces(List<MemberRace> dbPatientRaces, Patient updatePatient)
        {


            foreach (MemberRace memRace in dbPatientRaces)
            {

                bool remove = true;
                int removeRaceId = 0;

                if (updatePatient.ancestry != null && updatePatient.ancestry.Count > 0)
                {

                    foreach (HospitalRace race in updatePatient.ancestry)
                    {

                        if (memRace.hospital_race_ID.Equals(race.hospital_race_ID))
                        {
                            remove = false;
                        }

                        removeRaceId = race.hospital_race_ID;
                    }

                    if (remove && removeRaceId > 0)
                    {

                        _icmsContext.MemberRaces.Remove(memRace);
                        _icmsContext.SaveChanges();
                    }
                } else
                {

                    _icmsContext.MemberRaces.Remove(memRace);
                    _icmsContext.SaveChanges();
                }
            }

            updateNewRaces(updatePatient);
        }

        private void updateNewRaces(Patient updatePatient)
        {            

            if (updatePatient.ancestry != null && updatePatient.ancestry.Count > 0)
            {

                foreach (HospitalRace race in updatePatient.ancestry)
                {

                    MemberRace raceInDb = (

                            from memRace in _icmsContext.MemberRaces
                            where memRace.member_id.Equals(updatePatient.PatientId)
                            && memRace.hospital_race_ID.Equals(race.hospital_race_ID)
                            select memRace
                           )
                           .FirstOrDefault();

                    if (raceInDb == null)
                    {

                        MemberRace addRace = new MemberRace();
                        addRace.member_id = (Guid)updatePatient.PatientId;
                        addRace.hospital_race_ID = race.hospital_race_ID;
                        addRace.creation_date = DateTime.Now;
                        addRace.creation_user_id = updatePatient.usr;

                        _icmsContext.MemberRaces.Add(addRace);
                        _icmsContext.SaveChanges();
                    }
                }
            }
        }




        private string getCaseOwnerCaseTypeCode(Patient addPatient)
        {

            string caseTypeCode = "";

            if (addPatient.caseOwners[0] != null && addPatient.caseOwners[0].caseTypeId > 0)
            {

                caseTypeCode = (

                        from castyp in _icmsContext.CaseTypes
                        where castyp.case_type_id.Equals(addPatient.caseOwners[0].caseTypeId)
                        select castyp.case_type_code
                    )
                    .FirstOrDefault();
            }

            return caseTypeCode;
        }

        private List<Member> getPatientUsingParams(bool searchLastName, bool searchFirstName, bool searchId, bool searchSsn, bool searchDob,
                                                   bool searchAuth, Patient patient)
        {
            List<Member> members = null;

            if (searchLastName)
            {
                if (searchFirstName)
                {
                    if (searchId)
                    {
                        if (searchSsn)
                        {
                            if (searchDob)
                            {
                                if (searchAuth)
                                {
                                    members = getPatientUsingLastFirstInsuranceIdSsnDobAuthNum(patient.lastName, patient.firstName, 
                                                                                patient.insuranceMemberId, patient.ssn, patient.dateOfBirthDisplay,
                                                                                patient.referrals[0].authNumber);
                                }
                                else
                                {
                                    members = getPatientUsingLastFirstInsuranceIdSsnDob(patient.lastName, patient.firstName, patient.insuranceMemberId, patient.ssn, patient.dateOfBirthDisplay);
                                }
                            }
                            else
                            {
                                if (searchAuth)
                                {
                                    members = getPatientUsingLastFirstInsuranceIdSsnAuthNum(patient.lastName, patient.firstName, 
                                                                        patient.insuranceMemberId, patient.ssn, patient.referrals[0].authNumber);
                                }
                                else
                                {
                                    members = getPatientUsingLastFirstInsuranceIdSsn(patient.lastName, patient.firstName, patient.insuranceMemberId, patient.ssn);
                                }
                            }
                        }
                        else if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastFirstInsuranceIdDobAuth(patient.lastName, patient.firstName, patient.insuranceMemberId, 
                                                                                     patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastFirstInsuranceIdDob(patient.lastName, patient.firstName, patient.insuranceMemberId, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastFirstInsuranceIdAuthNum(patient.lastName, patient.firstName, patient.insuranceMemberId,
                                                                                    patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastFirstInsuranceId(patient.lastName, patient.firstName, patient.insuranceMemberId);
                            }
                        }
                    }
                    else
                    {
                        if (searchSsn)
                        {
                            if (searchDob)
                            {
                                if (searchAuth)
                                {
                                    members = getPatientUsingLastFirstSsnDobAuthNum(patient.lastName, patient.firstName, 
                                                                                    patient.ssn, patient.dateOfBirthDisplay,
                                                                                    patient.referrals[0].authNumber);
                                }
                                else
                                {
                                    members = getPatientUsingLastFirstSsnDob(patient.lastName, patient.firstName, patient.ssn, patient.dateOfBirthDisplay);
                                }
                            }
                            else
                            {
                                if (searchAuth)
                                {
                                    members = getPatientUsingLastFirstSsnAuthNumber(patient.lastName, patient.firstName, patient.ssn,
                                                                                    patient.referrals[0].authNumber);
                                }
                                else
                                {
                                    members = getPatientUsingLastFirstSsn(patient.lastName, patient.firstName, patient.ssn);
                                }
                            }
                        }
                        else if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastFirstDobAuthNumber(patient.lastName, patient.firstName, patient.dateOfBirthDisplay,
                                                                                patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastFirstDob(patient.lastName, patient.firstName, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastFirstAuth(patient.lastName, patient.firstName, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastFirst(patient.lastName, patient.firstName);
                            }
                        }
                    }
                }
                else if (searchId)
                {
                    if (searchSsn)
                    {
                        if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastInsuranceIdSsnDobAuthNumber(patient.lastName, patient.insuranceMemberId, patient.ssn,
                                                                                         patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastInsuranceIdSsnDob(patient.lastName, patient.insuranceMemberId, patient.ssn, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastInsuranceIdSsnAuthNumber(patient.lastName, patient.insuranceMemberId, patient.ssn,
                                                                                patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastInsuranceIdSsn(patient.lastName, patient.insuranceMemberId, patient.ssn);
                            }
                        }
                    }
                    else if (searchDob)
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingLastInsuranceIdDobAuthNumber(patient.lastName, patient.insuranceMemberId, 
                                                                                  patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingLastInsuranceIdDob(patient.lastName, patient.insuranceMemberId, patient.dateOfBirthDisplay);
                        }
                    }
                    else
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingLastInsuranceIdAuthNumber(patient.lastName, patient.insuranceMemberId, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingLastInsuranceId(patient.lastName, patient.insuranceMemberId);
                        }
                    }
                }
                else
                {
                    if (searchSsn)
                    {
                        if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastSsnDobAuthNumber(patient.lastName, patient.ssn, patient.dateOfBirthDisplay,
                                                                        patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastSsnDob(patient.lastName, patient.ssn, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingLastSsnAuthNumber(patient.lastName, patient.ssn, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingLastSsn(patient.lastName, patient.ssn);
                            }
                        }
                    }
                    else if (searchDob)
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingLastDobAuthNumber(patient.lastName, patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingLastDob(patient.lastName, patient.dateOfBirthDisplay);
                        }
                    }
                    else
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingLastAuthNumber(patient.lastName, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingLast(patient.lastName);
                        }
                    }
                }

            }
            else if (searchId)
            {
                if (searchFirstName)
                {
                    if (searchSsn)
                    {
                        if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingInsuranceIdFirstSsnDobAuthNumber(patient.insuranceMemberId, patient.firstName, patient.ssn, 
                                                                                          patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingInsuranceIdFirstSsnDob(patient.insuranceMemberId, patient.firstName, patient.ssn, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientUsingInsuranceIdFirstSsnAuthNumber(patient.insuranceMemberId, patient.firstName, patient.ssn,
                                                                                       patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientUsingInsuranceIdFirstSsn(patient.insuranceMemberId, patient.firstName, patient.ssn);
                            }
                        }
                    }
                    else if (searchDob)
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingInsuranceIdFirstDobAuthNumber(patient.insuranceMemberId, patient.firstName, 
                                                                                   patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingInsuranceIdFirstDob(patient.insuranceMemberId, patient.firstName, patient.dateOfBirthDisplay);
                        }
                    }
                    else
                    {
                        if (searchAuth)
                        {
                            members = getPatientUsingInsuranceIdFirstAuthNumber(patient.insuranceMemberId, patient.firstName, 
                                                                                patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientUsingInsuranceIdFirst(patient.insuranceMemberId, patient.firstName);
                        }
                    }
                }
                else
                {
                    if (searchSsn)
                    {
                        if (searchDob)
                        {
                            if (searchAuth)
                            {
                                members = getPatientsUsingInsuranceIdSsnDobAuthNumber(patient.insuranceMemberId, patient.ssn, patient.dateOfBirthDisplay,
                                                                            patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientsUsingInsuranceIdSsnDob(patient.insuranceMemberId, patient.ssn, patient.dateOfBirthDisplay);
                            }
                        }
                        else
                        {
                            if (searchAuth)
                            {
                                members = getPatientsUsingInsuranceIdSsnAuthNumber(patient.insuranceMemberId, patient.ssn, patient.referrals[0].authNumber);
                            }
                            else
                            {
                                members = getPatientsUsingInsuranceIdSsn(patient.insuranceMemberId, patient.ssn);
                            }
                        }
                    }
                    else if (searchDob)
                    {
                        if (searchAuth)
                        {
                            members = getPatientsUsingInsuranceIdDobAuthNumber(patient.insuranceMemberId, patient.dateOfBirthDisplay, 
                                                                               patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientsUsingInsuranceIdDob(patient.insuranceMemberId, patient.dateOfBirthDisplay);
                        }
                    }
                    else
                    {
                        if (searchAuth)
                        {
                            members = getPatientsUsingInsuranceIdAuthNumber(patient.insuranceMemberId, patient.referrals[0].authNumber);
                        }
                        else
                        {
                            members = getPatientsUsingInsuranceId(patient.insuranceMemberId);
                        }
                    }
                }

            }
            else if (searchSsn)
            {
                if (searchDob)
                {
                    if (searchAuth)
                    {
                        members = getPatientsUsingSsnDobAuthNumber(patient.ssn, patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                    }
                    else
                    {
                        members = getPatientsUsingSsnDob(patient.ssn, patient.dateOfBirthDisplay);
                    }
                }
                else
                {
                    if (searchAuth)
                    {
                        members = getPatientsUsingSsnAuthNumber(patient.ssn, patient.referrals[0].authNumber);
                    }
                    else
                    {
                        members = getPatientsUsingSsn(patient.ssn);
                    }
                }
            }
            else if (searchDob)
            {
                if (searchAuth)
                {
                    members = getPatientsUsingDobAuthNumber(patient.dateOfBirthDisplay, patient.referrals[0].authNumber);
                }
                else
                {
                    members = getPatientsUsingDob(patient.dateOfBirthDisplay);
                }
            }
            else if (searchAuth)
            {
                members = getPatientsUsingAuthNumber(patient.referrals[0].authNumber);
            }

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdSsnDobAuthNum(string lastName, string firstName, string insuranceMemberId,
                                                                              string ssn, string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use insurance id
            //use ssn
            //use dob
            //use auth
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && (patReferrals.auth_number.StartsWith(authNumber) || patReferrals.referral_number.StartsWith(authNumber))
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdSsnDob(string lastName, string firstName, string insuranceMemberId,
                                                                       string ssn, string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use insurance id
            //use ssn
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdSsnAuthNum(string lastName, string firstName, string insuranceMemberId,
                                                                           string ssn, string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use insurance id
            //use ssn
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        && (patReferrals.auth_number.StartsWith(authNumber) || patReferrals.referral_number.StartsWith(authNumber))
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdSsn(string lastName, string firstName, string insuranceMemberId,
                                                                    string ssn)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use insurance id
            //use ssn
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_ssn.StartsWith(ssn)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdDobAuth(string lastName, string firstName, string insuranceMemberId,
                                                                        string birth, string authNumber)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use insurance id
            //use dob
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        && (patReferrals.auth_number.StartsWith(authNumber) || patReferrals.referral_number.StartsWith(authNumber))
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdDob(string lastName, string firstName, string insuranceMemberId,
                                                                    string birth)
        {
            List<Member> members = null;

            DateTime dob = DateTime.Parse(birth);

            //use last name
            //use first name
            //use insurance id
            //use dob
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_birth.Equals(dob)
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower()
                        select pats
                       )
                       .ToList();

            return members;
        }

        private List<Member> getPatientUsingLastFirstInsuranceIdAuthNum(string lastName, string firstName, string insuranceMemberId,
                                                                        string authNumber)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use insurance id
            //auth number
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        join patRefs in _icmsContext.MemberReferrals
                        on pats.member_id equals patRefs.member_id into patReferral
                        from patReferrals in patReferral.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        && (patReferrals.auth_number.StartsWith(authNumber) || patReferrals.referral_number.StartsWith(authNumber))
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }
        private List<Member> getPatientUsingLastFirstInsuranceId(string lastName, string firstName, string insuranceMemberId)
        {
            List<Member> members = null;

            //use last name
            //use first name
            //use insurance id
            members = (
                        from pats in _icmsContext.Patients

                        join patenroll in _icmsContext.MemberEnrollments
                        on pats.member_id equals patenroll.member_id into patenrolls
                        from memenroll in patenrolls.DefaultIfEmpty()

                        where pats.member_last_name.StartsWith(lastName)
                        && pats.member_first_name.StartsWith(firstName)
                        && (memenroll.egp_member_id.StartsWith(insuranceMemberId) ||
                            memenroll.claims_enrollment_id.StartsWith(insuranceMemberId))
                        && pats.member_active_flag.Equals(true)
                        orderby pats.member_last_name.Trim().ToLower(), pats.member_first_name.Trim().ToLower(), pats.member_birth
                        select pats
                       )
                       .ToList();

            return members;
        }


        private string generateSsn()
        {

            string ssn = "";

            int ssnInt = 0;
            char ssnChar = char.MinValue;
            int nextSsnInt = 0;
            char nextSsnChar = char.MinValue;

            ssnInt = GetSsnInt();

            if (ssnInt > 0)
            {

                ssnChar = GetSsnChar();

                if (char.IsLetter(ssnChar))
                {
                    if (ssnInt >= 99999999)
                    {
                        nextSsnInt = 0;
                        nextSsnChar = (char)((int)ssnChar + 1);
                    }
                    else
                    {
                        nextSsnInt = ssnInt + 1;
                        nextSsnChar = ssnChar;
                    }

                    if (updateNextSsnProperty(nextSsnChar, nextSsnInt, ssnInt, ssnChar))
                    {
                        ssn = ssnChar + ssnInt.ToString("00000000");
                    }
                }
            }

            return ssn;
        }

        private int GetSsnInt()
        {
            
            int ssnInt = 0;

            string ssnValue = (

                    from sysprop in _icmsContext.SysProperties
                    where sysprop.propertyname.Equals("AutoSSN")
                    select sysprop.propertyvalue
                )
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(ssnValue))
            {
                ssnInt = Convert.ToInt32(ssnValue.Substring(ssnValue.Length - 8));
            }


            return ssnInt;
        }

        private char GetSsnChar()
        {

            char ssnChar = char.MinValue;

            string ssnValue = (

                    from sysprop in _icmsContext.SysProperties
                    where sysprop.propertyname.Equals("AutoSSN")
                    select sysprop.propertyvalue
                )
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(ssnValue))
            {
                ssnChar = Convert.ToChar(ssnValue.Substring(0, 1));
            }


            return ssnChar;
        }

        private bool updateNextSsnProperty(char nextSsnChar, int nextSsnInt, int ssnInt, char ssnChar)
        {

            bool updated = false;

            string currentValue = ssnChar.ToString() +  ssnInt.ToString("00000000");

            SysProperty dbSysProp = (

                    from sysprop in _icmsContext.SysProperties
                    where sysprop.propertyname.Equals("AutoSSN")
                    && sysprop.propertyvalue.Equals(currentValue)
                    select sysprop
                )
                .FirstOrDefault();

            if (dbSysProp != null)
            {

                dbSysProp.propertyvalue = nextSsnChar.ToString() + nextSsnInt.ToString("00000000");

                _icmsContext.SysProperties.Update(dbSysProp);
                updated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }

            return updated;
        }

    }
}

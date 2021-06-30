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

        public PatientService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
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
                            .Take(50)
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
                            .Take(50)
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

                            join ethnic in _icmsContext.MemberEthnicity
                            on pat.member_id equals ethnic.member_id into ethnic
                            from memberethnic in ethnic.DefaultIfEmpty()

                            where pat.member_id.Equals(guidMemId)
                            select new Patient
                            {
                                PatientId = pat.member_id,
                                FirstName = pat.member_first_name,
                                LastName = pat.member_last_name,
                                MiddleName = pat.member_middle_name,
                                DateOfBirth = pat.member_birth,
                                Ssn = pat.member_ssn,
                                Gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                Email = pat.member_email,
                                Ethnicity = memberethnic.ethnicity
                            }).SingleOrDefault();
            }


            return patient;
        }

        public IEnumerable<HospitalRace> GetPatientAncestryUsingId(string id)
        {
            IEnumerable<HospitalRace> ancestry = Enumerable.Empty<HospitalRace>();

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
                               race_name = hospitalrace.race_name
                           }).ToList();
            }


            return ancestry;
        }

        public IEnumerable<MemberAddress> GetPatientAddress(string id, bool returnOneAddress)
        {
            IEnumerable<MemberAddress> addresses = Enumerable.Empty<MemberAddress>();

            Guid guidMemId = Guid.Empty;

            if (Guid.TryParse(id, out guidMemId))
            {
                addresses = (from memadd in _icmsContext.MemberAddresses
                            where memadd.member_id.Equals(guidMemId) 
                            && (memadd.is_alternate.Equals(false) || memadd.is_alternate.Equals(null))
                            orderby memadd.member_address_id descending
                            select memadd)
                            .ToList();

                if (addresses.Any() && returnOneAddress)
                {
                    addresses.Take(1);
                }
            }


            return addresses;
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
                                InsuranceName = memIns.insurance_name,
                                SelfPay = memIns.self_pay,
                                IsMedicaid = memIns.is_medicaid,
                                IsMedicare = memIns.is_medicare,
                                InsuranceMemberId = memIns.insurance_id,
                                InsuranceSubscriberFirstName = memIns.subscriber_first_name,
                                InsuranceSubscriberLastName = memIns.subscriber_last_name,
                                InsuranceRelationshipId = memIns.insurance_relationship_id,
                                InsuranceRelationshipToPatient = insRel.relationship_name
                              })
                                 .Take(1)
                                 .SingleOrDefault();
            }

            return insurance;
        }
    }
}

using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Patient
    {
        public Guid? PatientId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string DateOfBirthDisplay { get; set; }

        public string Ssn { get; set; }

        public string? Gender { get; set; }

        public string? Ethnicity { get; set; }

        public string? Email { get; set; }

        public string? InsuranceName { get; set; }

        public string? InsuranceMemberId { get; set; }

        public bool? SelfPay { get; set; }

        public bool? IsMedicare { get; set; }
        public bool? IsMedicaid { get; set; }

        public string? InsuranceSubscriberFirstName { get; set; }
        public string? InsuranceSubscriberLastName { get; set; }

        public string? InsuranceRelationshipToPatient { get; set; }

        public int? InsuranceRelationshipId { get; set; }

        public IEnumerable<HospitalRace> Ancestry { get; set; }

        public IEnumerable<MemberAddress> Addresses { get; set; }

        public PhoneNumber HomePhoneNumber { get; set; }
    }
}

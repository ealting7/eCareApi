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


        public string? egpMemberId { get; set; }

        public string? claimsEnrollmentId { get; set; }


        public string FullName { get; set; }
        public string firstName { get; set; }

        public string middleName { get; set; }

        public string lastName { get; set; }

        public DateTime? dateOfBirth { get; set; }

        public string dateOfBirthDisplay { get; set; }
        public int age { get; set; }

        public string ssn { get; set; }

        public string? gender { get; set; }

        public string? ethnicity { get; set; }

        public string? emailAddress { get; set; }

        public byte? isSandsShpg { get; set; }

        public byte? isWishard { get; set; }



        public int enrollmentId { get; set; }
        public DateTime effectiveDate { get; set; }

        public DateTime? disenrollDate { get; set; }
        public int? disenrollReasonId { get; set; }



        public int? employerId { get; set; }

        public string? employerName { get; set; }

        public decimal employerLcmRate { get; set; }

        public int tpaId { get; set; }
        public string tpaName { get; set; }

        public string? InsuranceName { get; set; }

        public string? insuranceMemberId { get; set; }

        public bool? selfPay { get; set; }

        public bool? isMedicare { get; set; }
        public bool? isMedicaid { get; set; }

        public string? insuranceSubscriberFirstName { get; set; }
        public string? insuranceSubscriberLastName { get; set; }

        public string? insuranceRelationshipToPatient { get; set; }

        public int? insuranceRelationshipId { get; set; }

        public IEnumerable<HospitalRace> ancestry { get; set; }

        public IEnumerable<MemberAddress> addresses { get; set; }

        public PhoneNumber homePhoneNumber { get; set; }

        public Allergy allergies { get; set; }

        public AdvancedDirective dnr { get; set; }
    }
}

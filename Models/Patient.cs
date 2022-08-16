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

        public string mrn { get; set; }
        public string hospitalNo { get; set; }

        public string medicaidNumber { get; set; }
        public string medicareNumber { get; set; }

        public int? maritalStatusId { get; set; }

        public int? primaryLanguageId { get; set; }



        public int enrollmentId { get; set; }
        public DateTime effectiveDate { get; set; }

        public DateTime? disenrollDate { get; set; }
        public int? disenrollReasonId { get; set; }


        public Guid? caseOwnerId { get; set; }
        public string caseOwner { get; set; }



        public int? employerId { get; set; }

        public string? employerName { get; set; }

        public decimal employerLcmRate { get; set; }
        public string employerAddress { get; set; }
        public string employerCityStateZip { get; set; }

        public int tpaId { get; set; }
        public string tpaName { get; set; }

        public int insuranceId { get; set; }
        public string? InsuranceName { get; set; }

        public string? insuranceMemberId { get; set; }

        public bool? selfPay { get; set; }
        public bool? unInsured { get; set; }

        public bool? isMedicare { get; set; }
        public bool? isMedicaid { get; set; }

        public bool? inLcm { get; set; }
        public bool? optOutLcm { get; set; }
        public DateTime? optOutLcmDate { get; set; }

        public int? newlyIdentifiedCaseStatusIid { get; set; }
        public DateTime? newlyIdentifiedDateOfReferral { get; set; }
        public string newlyIdentifiedMethodOfIdentification { get; set; }


        public string? insuranceSubscriberFirstName { get; set; }
        public string? insuranceSubscriberLastName { get; set; }

        public string? insuranceRelationshipToPatient { get; set; }

        public int? insuranceRelationshipId { get; set; }

        public string insuranceGroupNumber { get; set; }
        public string insurancePlanNumber { get; set; }


        public List<HospitalRace> ancestry { get; set; }

        public List<MemberAddress> addresses { get; set; }

        public PhoneNumber homePhoneNumber { get; set; }
        public PhoneNumber eveningPhoneNumber { get; set; }
        public List<PhoneNumber> contactNumbers { get; set; }

        public Allergy allergies { get; set; }

        public AdvancedDirective dnr { get; set; }

        public Isolation isolation { get; set; }

        public PatientValuables admissionValuables {get; set;}


        public decimal? height { get; set; }
        public string displayHeight { get; set; }
        public decimal? weight { get; set; }
        public string displayWeight { get; set; }


        public Doctor generalProvider { get; set; }

        public List<Hospital> hospitals { get; set; }

        public List<Admission> pastAdmissions { get; set; }

        public Admission currentAdmission { get; set; }

        public List<Utilization> referrals { get; set; }

        public List<PatientContacts> hospitalContacts { get; set; }
        public MedicalHistory medicalHistory { get; set; }
        public MedicalHistory familyMedicalHistory { get; set; }
        public NextOfKin nextOfKin { get; set; }

        public List<IcmsUser> caseOwners { get; set; }

        public DateTime creationDate { get; set; }
        public Guid usr { get; set; }
    }
}

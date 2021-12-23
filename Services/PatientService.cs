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
            List<Patient> patientsFound = new List<Patient>();
            List<Member> members = null;

            if (!string.IsNullOrEmpty(search.lastName))
            {                

                if (!string.IsNullOrEmpty(search.firstName))
                {

                    members = (
                                from pats in _icmsContext.Patients
                                where pats.member_last_name.StartsWith(search.lastName)
                                && pats.member_first_name.StartsWith(search.firstName)
                                && pats.member_active_flag.Equals(true)
                                select pats
                               )
                               .ToList();

                } else
                {

                    members = (
                                from pats in _icmsContext.Patients
                                where pats.member_last_name.StartsWith(search.lastName)
                                && pats.member_active_flag.Equals(true)
                                select pats
                               )
                               .ToList();

                }


                if (members != null)
                {
                    if (!string.IsNullOrEmpty(search.dateOfBirthDisplay))
                    {
                        DateTime dob = DateTime.MinValue;

                        if (DateTime.TryParse(search.dateOfBirthDisplay, out dob))
                        {                            

                            members = (
                                        from pats in members
                                        where pats.member_birth.Equals(dob)
                                        select pats
                                      )
                                      .ToList(); 
                        }
                    }

                }


                if (members != null)
                {
                    if (!string.IsNullOrEmpty(search.ssn))
                    {
                        members = members.Where(mem => mem.member_ssn.Contains(search.ssn)).ToList();
                    }
                }
                
            }


            if (members != null && members.Count > 0)
            {

                List<Patient> finalPatients = null;

                finalPatients = (from pat in members

                                 join ethnic in _icmsContext.MemberEthnicity on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                 from memeth in memberethnic.DefaultIfEmpty()

                                 orderby pat.member_last_name, pat.member_first_name, pat.member_birth

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
                                     ethnicity = memeth?.ethnicity ?? String.Empty
                                 })                                 
                                 .ToList();

                if (finalPatients != null && finalPatients.Count > 1)
                {
                    patientsFound = finalPatients.OrderBy(x => x.lastName)
                                                 .ThenBy(x => x.firstName).ToList();
                                                 //.ThenBy(z => z.dateOfBirth).ToList();
                } else if (finalPatients != null)
                {
                    patientsFound = finalPatients;
                }

            }

            return patientsFound;
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

                                 join ethnic in _icmsContext.MemberEthnicity on pat.member_id equals (Guid)ethnic.member_id into memberethnic
                                 from memeth in memberethnic.DefaultIfEmpty()

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
                                     middleName = (string.IsNullOrEmpty(pat.member_middle_name)) ? pat.member_middle_name : "",
                                     dateOfBirth = (pat.member_birth.HasValue) ? pat.member_birth : DateTime.MinValue,
                                     dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "",
                                     age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0,
                                     ssn = pat.member_ssn,
                                     gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                     emailAddress = pat.member_email,
                                     ethnicity = memeth.ethnicity ?? String.Empty
                                 })
                                 .FirstOrDefault();

                if (dbPatient != null)
                {
                    dbPatient.allergies = getPatientAllergies((Guid)dbPatient.PatientId);
                    dbPatient.dnr = getPatientAdvancedDirectives((Guid)dbPatient.PatientId);

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
                                firstName = pat.member_first_name,
                                lastName = pat.member_last_name,
                                middleName = pat.member_middle_name,
                                dateOfBirth = pat.member_birth,
                                ssn = pat.member_ssn,
                                gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                                emailAddress = pat.member_email,
                                ethnicity = memberethnic.ethnicity
                            }).SingleOrDefault();
            }

            if (patient != null)
            {
                patient.addresses = GetPatientAddress(id, false);
                patient.homePhoneNumber = GetPatientHomePhoneNumber(id, true);
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

                        where pat.member_id.Equals(patientId)
                        select new Patient
                        {
                            PatientId = pat.member_id,
                            firstName = pat.member_first_name,
                            lastName = pat.member_last_name,
                            middleName = pat.member_middle_name,
                            FullName = pat.member_first_name + ((!string.IsNullOrEmpty(pat.member_middle_name)) ? " " + pat.member_middle_name  + " ": " ") + pat.member_last_name,
                            dateOfBirth = pat.member_birth,
                            dateOfBirthDisplay = (pat.member_birth.HasValue) ? pat.member_birth.Value.ToShortDateString() : "N/A",
                            age = (pat.member_birth.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(pat.member_birth)).Days / 365 : 0, 
                            ssn = pat.member_ssn,
                            gender = (!string.IsNullOrEmpty(pat.gender_code)) ? (pat.gender_code.ToLower().Equals("m")) ? "male" : "female" : "na",
                            emailAddress = pat.member_email,
                            ethnicity = memberethnic.ethnicity
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
                                selfPay = memIns.self_pay,
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
                allergies.echinaceaAllergy = (bool)inptAllergies.echinacea;
                allergies.ephedraAllergy = (bool)inptAllergies.ephedra;
                allergies.garlicAllergy = (bool)inptAllergies.garlic;
                allergies.gingkoBilobaAllergy = (bool)inptAllergies.gingko_biloba;
                allergies.ginkgoAllergy = (bool)inptAllergies.ginkgo;
                allergies.ginsengAllergy = (bool)inptAllergies.ginseng;
                allergies.kavaAllergy = (bool)inptAllergies.kava;
                allergies.latexAllergy = (bool)inptAllergies.latex_allergy;
                allergies.medicationAllergy = inptAllergies.medication_allergy;
                allergies.otherAllergies = inptAllergies.other_allergy;
                allergies.stJohnsWortAllergy = (bool)inptAllergies.st_johns_wort;
                allergies.valerianAllergy = (bool)inptAllergies.valerian;
                allergies.valerianRootAllergy = (bool)inptAllergies.valerian_root;
                allergies.viteAllergy = (bool)inptAllergies.vite;
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

    }
}

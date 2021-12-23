using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class HospitalService : IHospital
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _emrContext;

        public HospitalService(IcmsContext icmsContext, AspNetContext emrContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _emrContext = emrContext ?? throw new ArgumentNullException(nameof(emrContext));
        }



        public List<Admission> getHospitalDepartments(string hospitalId)
        {
            List<Admission> departments = null;

            int hospId = 0;

            if (int.TryParse(hospitalId, out hospId))
            {

                List<Admission> hospitalDepartments = new List<Admission>();

                hospitalDepartments = (
                                    from depts in _icmsContext.HospitalDepartments
                                    where depts.hospital_id.Equals(hospId)

                                    select new Admission
                                    {
                                        hospital_department_id = depts.hospital_department_id,
                                        departmentName = depts.hospital_department_name
                                    }
                              )
                              .ToList();

                if (hospitalDepartments != null && hospitalDepartments.Count > 0)
                {

                    departments = new List<Admission>();
                    departments = hospitalDepartments;

                    foreach (Admission dept in hospitalDepartments)
                    {
                        List<Room> rooms = null;

                        rooms = (
                                    from deptroom in _icmsContext.HospitalDepartmentRooms

                                    where deptroom.hospital_department_id.Equals(dept.hospital_department_id)
                                    select new Room
                                    {
                                        departmentRoomsId = deptroom.hospital_department_rooms_id,
                                        roomName = deptroom.name,
                                        roomOccupancy = deptroom.occupancy
                                    }
                                ).ToList();

                        if (rooms != null)
                        {
                            dept.departmentRooms = new List<Room>();
                            dept.departmentRooms = rooms;
                        }
                        
                    }

                }
            }

            return departments;
        }

        public List<Hospital> GetHospitals()
        {
            List<Hospital> hospitals = null;

            hospitals = (from hosp in _icmsContext.Hospitals
                          where (hosp.deleted.Equals(false) || (bool)hosp.deleted == null)
                          orderby hosp.name
                          select hosp)
                           .Take(50)
                           .ToList();

            return hospitals;
        }

        public IEnumerable<Hospital> GetCollectionFacilities()
        {
            IEnumerable<Hospital> facilities = Enumerable.Empty<Hospital>();

            facilities = (from hosp in _icmsContext.Hospitals
                            where hosp.specimen_collection_equipped.Equals(true)
                            && (hosp.deleted.Equals(0) || hosp.deleted == null)
                            orderby hosp.name
                            select hosp)
                           .Take(50)
                           .ToList();

            return facilities;
        }

        public IEnumerable<HospitalFacility> GetLaboratoryFacilities()
        {
            IEnumerable<HospitalFacility> facilities = Enumerable.Empty<HospitalFacility>();

            facilities = (from hosp in _icmsContext.Hospitals
                          join hospSpec in _icmsContext.HospitalSpecialtys
                          on hosp.hospital_specialty_id equals hospSpec.hospital_specialty_id
                          where hospSpec.hospital_specialty_id.Equals(2) //Laboratory
                          && (hosp.deleted.Equals(0) || hosp.deleted == null)
                          orderby hosp.name
                          select new HospitalFacility{
                              hospitalId = hosp.hospital_id,
                              hospitalName = hosp.name,
                              hospitalSpecialty = hospSpec.specialty_descr,
                              address1 = hosp.address1,
                              address2 = hosp.address2,
                              city = hosp.city,
                              zip = hosp.zip,
                              stateAbbrev = hosp.state_abbrev
                          })
                           .Take(50)
                           .ToList();

            return facilities;
        }



        public List<Admission> getHospitalInpatientAdmissions(Admission search)
        {
            List<Admission> admissions = null;

            try
            {

                admissions = (
                        from inptadmits in _icmsContext.HospitalInpatientAdmissions

                        join patient in _icmsContext.Patients
                        on inptadmits.member_id equals patient.member_id into pats
                        from inptpats in pats.DefaultIfEmpty()

                        join visitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                        on inptadmits.hospital_inpatient_admission_id equals visitreason.hospital_inpatient_admission_id into reasons
                        from reasonforvisit in reasons.DefaultIfEmpty()

                        join icd10 in _icmsContext.DiagnosisCodes10
                        on inptadmits.admission_diagnosis_code_id equals icd10.diagnosis_codes_10_id into icdcodes
                        from icd10s in icdcodes.DefaultIfEmpty()

                        join hospdeptrooms in _icmsContext.HospitalDepartmentRooms
                        on inptadmits.hospital_department_rooms_id equals hospdeptrooms.hospital_department_rooms_id into rooms
                        from hosprooms in rooms.DefaultIfEmpty()

                        where inptadmits.hospital_id.Equals(search.hospital_id)
                        && inptadmits.registered_date != null
                        && inptadmits.discharged_date == null

                        select new Admission
                        {
                            hospital_inpatient_admission_id = inptadmits.hospital_inpatient_admission_id,
                            registration_number = inptadmits.registration_number,
                            member_id = inptadmits.member_id,
                            patientName = inptpats.member_first_name + " " + inptpats.member_last_name,
                            patientFullName = inptpats.member_first_name + ((!string.IsNullOrEmpty(inptpats.member_middle_name)) ? " " + inptpats.member_middle_name + " " : " ") + inptpats.member_last_name,
                            reasonForVisit = reasonforvisit.reason_for_visit,
                            admitDate = (inptadmits.registered_date.HasValue) ? inptadmits.registered_date : DateTime.MinValue,
                            displayAdmitDate = (inptadmits.registered_date.HasValue) ? inptadmits.registered_date.Value.ToShortDateString() : "N/A",
                            lengthOfStay = (inptadmits.registered_date.HasValue) ? DateTime.Now.Subtract(Convert.ToDateTime(inptadmits.registered_date)).Days : 1,
                            admission_diagnosis_code_id = inptadmits.admission_diagnosis_code_id,
                            admitIcdCode = icd10s.diagnosis_code,
                            admitIcdCodeDescription = (icd10s.short_description != null) ? icd10s.short_description : icd10s.long_description,
                            departmentRoomName = hosprooms.name,
                            nurseName = "N/A",
                            physicianName = "N/A"
                        }
                    )
                    .Distinct()
                    .ToList();



                return admissions;

            }
            catch(Exception ex)
            {
                return admissions;
            }
        }

        public List<Admission> getPatientInpatientAdmissions(string patientId)
        {
            List<Admission> patientAdmission = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {
                patientAdmission = (
                        from inptadmits in _icmsContext.HospitalInpatientAdmissions

                        join visitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                        on inptadmits.hospital_inpatient_admission_id equals visitreason.hospital_inpatient_admission_id into reasons
                        from reasonforvisit in reasons.DefaultIfEmpty()

                        join icd10 in _icmsContext.DiagnosisCodes10
                        on inptadmits.admission_diagnosis_code_id equals icd10.diagnosis_codes_10_id into icdcodes
                        from icd10s in icdcodes.DefaultIfEmpty()

                        where inptadmits.member_id.Equals(memberId)
                        && inptadmits.registered_date != null

                        select new Admission
                        {
                            hospital_inpatient_admission_id = inptadmits.hospital_inpatient_admission_id,
                            registration_number = inptadmits.registration_number,
                            reasonForVisit = reasonforvisit.reason_for_visit,
                            admitDate = inptadmits.registered_date,
                            displayAdmitDate = (inptadmits.registered_date.HasValue) ? inptadmits.registered_date.Value.ToShortDateString() : "N/A",
                            admission_diagnosis_code_id = inptadmits.admission_diagnosis_code_id,
                            admitIcdCode = icd10s.diagnosis_code,
                            admitIcdCodeDescription = (icd10s.short_description != null) ? icd10s.short_description : icd10s.long_description
                        }
                    )
                    .Distinct()
                    .ToList();

            }

            return patientAdmission;
        }

        public Admission getInpatientAdmission(string admitId)
        {
            Admission patientAdmission = null;

            int hospitalInpatientAdmissionId = 0;

            if (int.TryParse(admitId, out hospitalInpatientAdmissionId))
            {
                patientAdmission = (
                        from inptadmits in _icmsContext.HospitalInpatientAdmissions

                        join visitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                        on inptadmits.hospital_inpatient_admission_id equals visitreason.hospital_inpatient_admission_id into reasons
                        from reasonforvisit in reasons.DefaultIfEmpty()

                        join icd10 in _icmsContext.DiagnosisCodes10
                        on inptadmits.admission_diagnosis_code_id equals icd10.diagnosis_codes_10_id into icdcodes
                        from icd10s in icdcodes.DefaultIfEmpty()

                        join deptRoom in _icmsContext.HospitalDepartmentRooms
                        on inptadmits.hospital_department_rooms_id equals deptRoom.hospital_department_rooms_id into rooms
                        from deptrooms in rooms.DefaultIfEmpty() 

                        where inptadmits.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)

                        select new Admission
                        {
                            hospital_inpatient_admission_id = inptadmits.hospital_inpatient_admission_id,
                            hospital_id = inptadmits.hospital_id,
                            hospital_department_id = inptadmits.hospital_department_id,
                            member_id = inptadmits.member_id,
                            registration_number = inptadmits.registration_number,
                            reasonForVisit = reasonforvisit.reason_for_visit,
                            admitDate = inptadmits.registered_date,
                            displayAdmitDate = (inptadmits.registered_date.HasValue) ? inptadmits.registered_date.Value.ToShortDateString() : "N/A",
                            admission_diagnosis_code_id = inptadmits.admission_diagnosis_code_id,
                            admitIcdCode = icd10s.diagnosis_code,
                            admitIcdCodeDescription = (icd10s.short_description != null) ? icd10s.short_description : icd10s.long_description,
                            hospital_department_rooms_id = inptadmits.hospital_department_rooms_id,
                            departmentRoomName = deptrooms.name
                        }
                    )
                    .FirstOrDefault();

                if (patientAdmission != null && !patientAdmission.member_id.Equals(Guid.Empty))
                {
                    patientAdmission.patient = getAdmissionPatientInfo(patientAdmission);
                    //patientAdmission.departmentRooms = getAdmissionDepartmentRooms(patientAdmission);
                }

            }

            return patientAdmission;
        }

        private Patient getAdmissionPatientInfo(Admission admit)
        {
            Patient admitPatient = null;

            PatientService patService = new PatientService(_icmsContext, _emrContext);
            Patient returnPatient = patService.getAdmissionPatientsUsingId(admit.member_id);

            if (returnPatient != null)
            {
                admitPatient = returnPatient;
                admitPatient.allergies = getPatientAllergies(admit);
                admitPatient.dnr = getPatientAdvancedDirectives(admit.member_id); 
            }

            return admitPatient;
        }

        public Admission getPatientCurrentAdmission(string patientId)
        {
            Admission patientAdmission = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {
                patientAdmission = (
                        from inptadmits in _icmsContext.HospitalInpatientAdmissions

                        join visitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                        on inptadmits.hospital_inpatient_admission_id equals visitreason.hospital_inpatient_admission_id into reasons
                        from reasonforvisit in reasons.DefaultIfEmpty()

                        join icd10 in _icmsContext.DiagnosisCodes10
                        on inptadmits.admission_diagnosis_code_id equals icd10.diagnosis_codes_10_id into icdcodes
                        from icd10s in icdcodes.DefaultIfEmpty()

                        join deptRoom in _icmsContext.HospitalDepartmentRooms
                        on inptadmits.hospital_department_rooms_id equals deptRoom.hospital_department_rooms_id into rooms
                        from deptrooms in rooms.DefaultIfEmpty()

                        where inptadmits.member_id.Equals(memberId)
                        && inptadmits.registered_date.HasValue
                        && !inptadmits.discharged_date.HasValue

                        orderby inptadmits.registered_date descending

                        select new Admission
                        {
                            hospital_inpatient_admission_id = inptadmits.hospital_inpatient_admission_id,
                            hospital_id = inptadmits.hospital_id,
                            member_id = inptadmits.member_id,
                            registration_number = inptadmits.registration_number,
                            reasonForVisit = reasonforvisit.reason_for_visit,
                            admitDate = inptadmits.registered_date,
                            displayAdmitDate = (inptadmits.registered_date.HasValue) ? inptadmits.registered_date.Value.ToShortDateString() : "N/A",
                            admission_diagnosis_code_id = inptadmits.admission_diagnosis_code_id,
                            admitIcdCode = icd10s.diagnosis_code,
                            admitIcdCodeDescription = (icd10s.short_description != null) ? icd10s.short_description : icd10s.long_description,
                            departmentRoomName = deptrooms.name
                        }
                    )
                    .Take(1)
                    .FirstOrDefault();

                if (patientAdmission != null && !patientAdmission.member_id.Equals(Guid.Empty))
                {
                    patientAdmission.patient = getAdmissionPatientInfo(patientAdmission);
                }

            }

            return patientAdmission;
        }

        private List<Room> getAdmissionDepartmentRooms(Admission admit)
        {
            List<Room> departmentRooms = null;

            if (admit.hospital_department_id > 0)
            {
                departmentRooms = (
                                    from deptrooms in _icmsContext.HospitalDepartmentRooms
                                    where deptrooms.hospital_department_id.Equals(admit.hospital_department_id)
                                    select new Room
                                    {
                                        departmentRoomsId = deptrooms.hospital_department_rooms_id,
                                        roomName = deptrooms.name
                                    }
                                  )
                                  .ToList();
            }

            return departmentRooms;
        } 



        public Allergy getPatientAllergies(Admission admit)
        {
            Allergy allergies = null;

            HospitalInpatientAdmissionAllergies inptAllergies = null;

            inptAllergies = (
                                from hospAlrg in _icmsContext.HospitalInpatientAdmissionAllergys
                                where hospAlrg.hospital_inpatient_admission_id.Equals(admit.hospital_inpatient_admission_id)
                                select hospAlrg
                            )
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
            catch(Exception ex)
            {
                return directives;
            }            
        }




        public CareplanAssessItem getCareplanAssessFormItems()
        {
            CareplanAssessItem assessItems = new CareplanAssessItem();

            getCareplanAssessGeneralItems(ref assessItems);
            getCareplanAssessVitalItems(ref assessItems);
            getCareplanAssessHeentItems(ref assessItems);
            getCareplanAssessAbdomenItems(ref assessItems);
            getCareplanAssessExtremityItems(ref assessItems);
            getCareplanDiagnosisItems(ref assessItems);
            getCareplanOutcomeItems(ref assessItems);
            getCareplanInterventionItems(ref assessItems);

            return assessItems;
        }


        private void getCareplanAssessGeneralItems(ref CareplanAssessItem assessItems)
        {
            assessItems.breathingRates = getBreathingRates();
            assessItems.chronologicalDevelopmentAppearances = getChronologicalDevelopmentAppearances();
            assessItems.breathingTypes = getBreathingTypes();
            assessItems.mentalStatuses = getMentalStatuses();
            assessItems.painLevels = getPainLevels();
            assessItems.alertnessStates = getAlertnessState();
        }

        public List<AssessItem> getBreathingRates()
        {            
            List<AssessItem> breathrates = null;

            breathrates = (
                from breaths in _icmsContext.HospitalBreathingRates
                where !breaths.deleted.HasValue || breaths.deleted.Equals(0)                
                select new AssessItem
                {
                    breathingRateId = breaths.hospital_breathing_rate_id,
                    breathingRate = breaths.breathing_rate
                }
                )
                .ToList();

            return breathrates;

        }

        public List<AssessItem> getBreathingTypes()
        {
            List<AssessItem> breathtypes = null;

            breathtypes = (
                from breaths in _icmsContext.HospitalBreathingTypes
                where !breaths.deleted.HasValue || breaths.deleted.Equals(0)
                select new AssessItem
                {
                    breathingTypeId = breaths.hospital_breathing_type_id,
                    breathingType = breaths.breathing_type
                }
                )
                .ToList();

            return breathtypes;

        }

        public List<AssessItem> getChronologicalDevelopmentAppearances()
        {
            List<AssessItem> chronologicalappearances = null;

            chronologicalappearances = (
                from chronos in _icmsContext.HospitalChronologicalDevelopmentAppearances
                where !chronos.deleted.HasValue || chronos.deleted.Equals(0)
                select new AssessItem
                {
                    chronologicalDevelopmentAppearanceId = chronos.hospital_chronological_development_appearance_id,
                    developmentAppearance = chronos.development_appearance
                }
                )
                .ToList();

            return chronologicalappearances;

        }

        public List<AssessItem> getMentalStatuses()
        {
            List<AssessItem> mentalStats = null;

            mentalStats = (
                from mental in _icmsContext.HospitalMentalStatuses
                where !mental.deleted.HasValue || mental.deleted.Equals(0)
                select new AssessItem
                {
                    mentalStatusId = mental.hospital_mental_status_id,
                    mentalStatus = mental.mental_status
                }
                )
                .ToList();

            return mentalStats;

        }

        public List<AssessItem> getPainLevels()
        {
            List<AssessItem> painLvls = null;

            painLvls = (
                from mental in _icmsContext.HospitalPainLevels
                where !mental.deleted.HasValue || mental.deleted.Equals(0)
                select new AssessItem
                {
                    painLevelId = mental.hospital_pain_level_id,
                    painLevel = mental.pain_level
                }
                )
                .ToList();

            return painLvls;

        }

        public List<AssessItem> getAlertnessState()
        {
            List<AssessItem> alerts = null;

            alerts = (
                from mental in _icmsContext.HospitalAlertnessStates
                where !mental.deleted.HasValue || mental.deleted.Equals(0)
                select new AssessItem
                {
                    alertnessStateId = mental.hospital_alertness_state_id,
                    alertnessName = mental.alertness_name
                }
                )
                .ToList();

            return alerts;

        }


        private void getCareplanAssessVitalItems(ref CareplanAssessItem assessItems)
        {
            assessItems.temperatureSites = getTemperatureSites();
            assessItems.respirationRegularity = getRespirationRegularity();
            assessItems.respirationDepth = getRespirationDepth();
            assessItems.pulsePosture = getPulsePosture();
            assessItems.pulseRhythm = getPulseRhythm();
            assessItems.pulseIntensity = getPulseIntensity();
        }

        public List<AssessItem> getTemperatureSites()
        {
            List<AssessItem> termperatureSites = null;

            termperatureSites = (
                from temp in _icmsContext.HospitalTemperatureSites
                where !temp.deleted.HasValue || temp.deleted.Equals(0)
                select new AssessItem
                {
                    temperatureSiteId = temp.hospital_temperature_site_id,
                    temperatureSite = temp.temperature_site
                }
                )
                .ToList();

            return termperatureSites;
        }
        public List<AssessItem> getRespirationRegularity()
        {
            List<AssessItem> respirationRegularities = null;


            respirationRegularities = (
                from resp in _icmsContext.HospitalRespirationRegularities
                where !resp.deleted.HasValue || resp.deleted.Equals(0)
                select new AssessItem
                {
                    respirationRegularityId = resp.hospital_respiration_regularity_id,
                    respirationRegularity = resp.respiration_regularity
                }
                )
                .ToList();

            return respirationRegularities;
        }
        public List<AssessItem> getRespirationDepth()
        {
            List<AssessItem> respirationDepth = null;

            respirationDepth = (
                from resp in _icmsContext.HospitalRespirationDepths
                where !resp.deleted.HasValue || resp.deleted.Equals(0)
                select new AssessItem
                {
                    respirationDepthId = resp.hospital_respiration_depth_id,
                    respirationDepth = resp.respiration_depth
                }
                )
                .ToList();

            return respirationDepth;
        }
        public List<AssessItem> getPulsePosture()
        {
            List<AssessItem> publsePostures = null;

            publsePostures = (
                from pulse in _icmsContext.HospitalPulsePositionForReadings
                where !pulse.deleted.HasValue || pulse.deleted.Equals(0)
                select new AssessItem
                {
                    pulsePositionForReadingId = pulse.hospital_pulse_position_for_reading_id,
                    pulsePositionForReading = pulse.position_for_reading
                }
                )
                .ToList();

            return publsePostures;
        }
        public List<AssessItem> getPulseRhythm()
        {
            List<AssessItem> pulseRhythms = null;

            pulseRhythms = (
                from pulse in _icmsContext.HospitalPulseRhythms
                where !pulse.deleted.HasValue || pulse.deleted.Equals(0)
                select new AssessItem
                {
                    pulseRhythmId= pulse.hospital_pulse_rhythm_id,
                    pulseRhythm = pulse.pulse_rhythm
                }
                )
                .ToList();

            return pulseRhythms;
        }
        public List<AssessItem> getPulseIntensity()
        {
            List<AssessItem> pulseIntensities = null;

            pulseIntensities = (
                from pulse in _icmsContext.HospitalPulseIntensities
                where !pulse.deleted.HasValue || pulse.deleted.Equals(0)
                select new AssessItem
                {
                    pulseIntensityId = pulse.hospital_pulse_intensity_id,
                    pulseIntensity = pulse.pulse_intensity
                }
                )
                .ToList();

            return pulseIntensities;
        }



        private void getCareplanAssessHeentItems(ref CareplanAssessItem assessItems)
        {
            getHeentHeadItems(ref assessItems);
        }
        private void getHeentHeadItems(ref CareplanAssessItem assessItems)
        {
            assessItems.headSkinColor = getHeadSkinColor();
            assessItems.headProportionToBody = getHeadProportionToBody();
        }

        public List<AssessItem> getHeadSkinColor()
        {
            List<AssessItem> headSkinColors = null;

            headSkinColors = (
                from skin in _icmsContext.HospitalHeadSkinColors
                where !skin.deleted.HasValue || skin.deleted.Equals(0)
                select new AssessItem
                {
                    headSkinColorId = skin.hospital_head_skin_color_id,
                    skinColor = skin.skin_color
                }
                )
                .ToList();

            return headSkinColors;
        }
        public List<AssessItem> getHeadProportionToBody()
        {
            List<AssessItem> headProportions = null;

            headProportions = (
                from propor in _icmsContext.HospitalHeadProportionToBodies
                where !propor.deleted.HasValue || propor.deleted.Equals(0)
                select new AssessItem
                {
                    headProportionToBodyId = propor.hospital_head_proportion_to_body_id,
                    proportionSize = propor.proportion_size
                }
                )
                .ToList();

            return headProportions;
        }



        public void getCareplanAssessAbdomenItems(ref CareplanAssessItem assessItems)
        {
            assessItems.abdomenAbdominalContour = getAbdomenAbdominalContour();
        }

        public List<AssessItem> getAbdomenAbdominalContour()
        {
            List<AssessItem> abContours = null;

            abContours = (
                from abs in _icmsContext.HospitalAbdominalContours
                where !abs.deleted.HasValue || abs.deleted.Equals(0)
                select new AssessItem
                {
                    abdominalContourId = abs.hospital_abdominal_contour_id,
                    abdominalContour = abs.abdominal_contour
                }
                )
                .ToList();

            return abContours;
        }



        public void getCareplanAssessExtremityItems(ref CareplanAssessItem assessItems)
        {
            getExtremitiyUpperItems(ref assessItems);
        }

        public void getExtremitiyUpperItems(ref CareplanAssessItem assessItems)
        {
            assessItems.upperPalpateArteryStrength = getUpperPalpateArteryStrength();
            assessItems.upperSqueezePushStrength = getUpperSqueezePushStrength();
        }

        public List<AssessItem> getUpperPalpateArteryStrength()
        {
            List<AssessItem> upper = null;

            upper = (
                from artery in _icmsContext.HospitalPalpateArteryStrengths
                where !artery.deleted.HasValue || artery.deleted.Equals(0)
                select new AssessItem
                {
                    upperPalpateArteryStrengthId = artery.hospital_palpate_artery_strength_id,
                    upperPalpateArteryStrength = artery.palpate_artery_strength
                }
                )
                .ToList();

            return upper;
        }

        public List<AssessItem> getUpperSqueezePushStrength()
        {
            List<AssessItem> upper = null;

            upper = (
                from squeeze in _icmsContext.HospitalSqueezePushStrengths
                where !squeeze.deleted.HasValue || squeeze.deleted.Equals(0)
                select new AssessItem
                {
                    upperSqueezePushStrengthId = squeeze.hospital_squeeze_push_strength_id,
                    upperSqueezePushStrength = squeeze.squeeze_push_strength
                }
                )
                .ToList();

            return upper;
        }




        public void getCareplanDiagnosisItems(ref CareplanAssessItem assessItems)
        {
            assessItems.diagnosisDomains = getDiagnosisDomain();
            assessItems.diagnosisClass = getDiagnosisClass();
            assessItems.diagnosisPriority = getDiagnosisPriority();
        }

        public CareplanAssessItem getCareplanDiagnosisDomainClasses(string domainId)
        {
            CareplanAssessItem returnDomainClasses = null;

            CareplanAssessItem domainClasses = new CareplanAssessItem();
            domainClasses.diagnosisClass = getDiagnosisDomainClasses(domainId);

            if (domainClasses != null)
            {
                returnDomainClasses = new CareplanAssessItem();
                returnDomainClasses.diagnosisClass = domainClasses.diagnosisClass;
            }

            return returnDomainClasses;
        }

        public List<AssessItem> getDiagnosisDomain()
        {
            List<AssessItem> domain = null;

            domain = (
                from domains in _icmsContext.HospitalNursingDiagnosisDomains
                select new AssessItem
                {
                    diagnosisDomainId = domains.hospital_nursing_diagnosis_domain_id,
                    diagnosisDomainName = domains.domain_name
                }
                )
                .ToList();

            return domain;
        }

        public List<AssessItem> getDiagnosisClass()
        {
            List<AssessItem> classes = null;

            classes = (
                from domains in _icmsContext.HospitalNursingDiagnosisClasses
                select new AssessItem
                {
                    diagnosisClassId = domains.hospital_nursing_diagnosis_class_id,
                    diagnosisClassDomainId = domains.hospital_nursing_diagnosis_domain_id,
                    diagnosisClassName = domains.class_name
                }
                )
                .ToList();

            return classes;
        }

        public List<AssessItem> getDiagnosisPriority()
        {
            List<AssessItem> priority = null;

            priority = (
                from priorities in _icmsContext.TaskPriorities
                select new AssessItem
                {
                    diagnosisPriorityId = priorities.task_priority_id,
                    diagnosisPriorityDescription = priorities.task_description
                }
                )
                .ToList();

            return priority;
        }

        public List<AssessItem> getDiagnosisDomainClasses(string domainId)
        {
            List<AssessItem> classes = null;

            int diagnosisDomainId = 0;

            if (int.TryParse(domainId, out diagnosisDomainId))
            {
                classes = (
                    from domains in _icmsContext.HospitalNursingDiagnosisClasses
                    where domains.hospital_nursing_diagnosis_domain_id.Equals(diagnosisDomainId)
                    select new AssessItem
                    {
                        diagnosisClassId = domains.hospital_nursing_diagnosis_class_id,
                        diagnosisClassDomainId = diagnosisDomainId,
                        diagnosisClassName = domains.class_name
                    }
                    )
                    .ToList();
            }

            return classes;
        }



        public void getCareplanOutcomeItems(ref CareplanAssessItem assessItems)
        {
            //HospitalCareplanGoal
            assessItems.outcomeGoalMeasurment = getOutcomeGoalMeasurement();
        }

        public List<AssessItem> getOutcomeGoalMeasurement()
        {
            List<AssessItem> goalmeasures = null;

            goalmeasures = (
                from goals in _icmsContext.HospitalCareplanGoals
                select new AssessItem
                {
                    outcomeGoalId = goals.hospital_careplan_goal_id,
                    outcomeGoalMeasure = goals.goal_measure
                }
                )
                .ToList();

            return goalmeasures;
        }


        public void getCareplanInterventionItems(ref CareplanAssessItem assessItems)
        {
            assessItems.interventionFrequency = getInterventionFrequencyAdministration();
            assessItems.interventionTypes = getInterventionTypes();
        }

        public List<AssessItem> getInterventionFrequencyAdministration()
        {
            List<AssessItem> frequency = null;

            frequency = (
                from goals in _icmsContext.HospitalMedicationFrequencyAdministrations
                select new AssessItem
                {
                    interventionFrequencyAdministrationId = goals.hospital_medication_frequency_administration_id,
                    interventionAdministrationFrequency = goals.administration_frequency
                }
                )
                .ToList();

            return frequency;
        }

        public List<AssessItem> getInterventionTypes()
        {
            List<AssessItem> interventionTypes = null;

            interventionTypes = (
                from intvtypes in _icmsContext.HospitalCareplanInterventionTypes
                select new AssessItem
                {
                    interventionTypeId = intvtypes.hospital_careplan_intervention_type_id,
                    interventionType = intvtypes.intervention_type
                }
                )
                .ToList();

            return interventionTypes;
        }




        public List<AssessItem> getCareplanAssessBasicGenerals(int inpatientAdmissionId)
        {
            List<AssessItem> generalAssesses = null;

            generalAssesses = (
                from hospInptAdmNurGen in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicGenerals

                join breathType in _icmsContext.HospitalBreathingTypes
                on hospInptAdmNurGen.hospital_breathing_type_id equals breathType.hospital_breathing_type_id into brthtypes
                from breathTypes in brthtypes.DefaultIfEmpty()

                join breathRate in _icmsContext.HospitalBreathingRates
                on hospInptAdmNurGen.hospital_breathing_rate_id equals breathRate.hospital_breathing_rate_id into brthrates
                from breathRates in brthrates.DefaultIfEmpty()

                join appearance in _icmsContext.HospitalChronologicalDevelopmentAppearances
                on hospInptAdmNurGen.hospital_chronological_development_appearance_id equals appearance.hospital_chronological_development_appearance_id into appear
                from appearances in appear.DefaultIfEmpty()

                join mentalstatus in _icmsContext.HospitalMentalStatuses
                on hospInptAdmNurGen.hospital_mental_status_id equals mentalstatus.hospital_mental_status_id into mentls
                from mentalstatuses in mentls.DefaultIfEmpty()

                join painLevel in _icmsContext.HospitalPainLevels
                on hospInptAdmNurGen.hospital_pain_level_id equals painLevel.hospital_pain_level_id into pinlvls
                from painLevels in pinlvls.DefaultIfEmpty()

                join alert in _icmsContext.HospitalAlertnessStates
                on hospInptAdmNurGen.hospital_alertness_state_id equals alert.hospital_alertness_state_id into alrts
                from alerts in alrts.DefaultIfEmpty()

                where hospInptAdmNurGen.hospital_inpatient_admission_id.Equals(inpatientAdmissionId)

                select new AssessItem
                {
                    inpatientAdmissionId = hospInptAdmNurGen.hospital_inpatient_admission_id,
                    basicGeneralAssessId = hospInptAdmNurGen.hospital_inpatient_admission_nursing_process_assessment_basic_general_id,
                    breathingRateId = hospInptAdmNurGen.hospital_breathing_rate_id,
                    breathingRate = breathTypes.breathing_type,
                    breathingTypeId = hospInptAdmNurGen.hospital_breathing_rate_id,
                    breathingType = breathRates.breathing_rate,
                    chronologicalDevelopmentAppearanceId = hospInptAdmNurGen.hospital_chronological_development_appearance_id,
                    developmentAppearance = appearances.development_appearance,
                    mentalStatusId = hospInptAdmNurGen.hospital_mental_status_id,
                    mentalStatus = mentalstatuses.mental_status,
                    painLevelId = hospInptAdmNurGen.hospital_pain_level_id,
                    painLevel = painLevels.pain_level,
                    alertnessStateId = hospInptAdmNurGen.hospital_alertness_state_id,
                    alertnessName = alerts.alertness_name,
                    answeredNameCorrectly = hospInptAdmNurGen.answered_name_correctly,
                    answeredDobCorrectly = hospInptAdmNurGen.answered_dob_correctly,
                    stateOfHealthThin = hospInptAdmNurGen.state_of_health_thin,
                    stateOfHealthCachectic = hospInptAdmNurGen.state_of_health_cachectic,
                    stateOfHealthTemporalWasting = hospInptAdmNurGen.state_of_health_temporal_wasting,
                    stateOfHealthPale = hospInptAdmNurGen.state_of_health_pale,
                    stateOfHealthDiaphoretic = hospInptAdmNurGen.state_of_health_diaphoretic,
                    stateOfHealthSignsOfPain = hospInptAdmNurGen.state_of_health_signs_of_pain,
                    complainsOfDiscomfort = hospInptAdmNurGen.complains_of_discomfort,
                    signsOfDiscomfort = hospInptAdmNurGen.signs_of_discomfort,
                    painDetails = hospInptAdmNurGen.pain_details,
                    hospitalReassessmentTimeframeId = hospInptAdmNurGen.hospital_reassessment_timeframe_id,
                    assessmentResult = hospInptAdmNurGen.assessment_result,
                    requestMedication = hospInptAdmNurGen.request_medication,
                    needsWoundCare = hospInptAdmNurGen.needs_wound_care,
                    requestLab = hospInptAdmNurGen.request_lab,
                    needsSocialServie = hospInptAdmNurGen.needs_social_servie,
                    requestTherapy = hospInptAdmNurGen.request_therapy,
                    callDr = hospInptAdmNurGen.call_dr
                }
                ).ToList();

            return generalAssesses;
        }


        public Hospital GetHospitalUsingId(int id)
        {
            Hospital facility = new Hospital();

            facility = (from hosp in _icmsContext.Hospitals
                        where hosp.hospital_id.Equals(id)
                        select hosp).FirstOrDefault();                        

            return facility;
        }


        public bool CreateInpatientAdmission(HospitalInpatientAdmission admit)
        {
            bool admitCreated = false;


            if (!admit.member_id.Equals(Guid.Empty))
            {
                if (!string.IsNullOrEmpty(admit.registration_number))
                {
                    admit.registration_number = generateRegistrationNumber();
                }


                _icmsContext.HospitalInpatientAdmissions.Add(admit);

                admitCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return admitCreated;
        }

        public bool CreateInpatientAdmissionOrder(HospitalInpatientAdmissionOrder order)
        {
            bool orderCreated = false;


            if (order.hospital_inpatient_admission_id > 0)
            {
                _icmsContext.HospitalInpatientAdmissionOrders.Add(order);

                orderCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return orderCreated;
        }

        public bool CreateInpatientAdmissionOrderLab(HospitalInpatientAdmissionOrderLab labOrder)
        {
            bool labCreated = false;


            if (labOrder.hospital_inpatient_admission_order_id > 0)
            {
                _icmsContext.HospitalInpatientAdmissionOrderLabs.Add(labOrder);

                labCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return labCreated;
        }

        public bool CreateInpatientAdmissionOrderDiagnosis(List<HospitalInpatientAdmissionOrderDiagnosis> diags)
        {
            bool diagsCreated = false;


            foreach (HospitalInpatientAdmissionOrderDiagnosis icd10 in diags)
            {
                bool added = false;


                if (icd10.hospital_inpatient_admission_id > 0 &&
                    icd10.hospital_inpatient_admission_order_id > 0 &&
                    icd10.diagnosis_codes_10_id > 0)
                {
                    _icmsContext.HospitalInpatientAdmissionDiagnoses.Add(icd10);

                    added = (_icmsContext.SaveChanges() > 0) ? true : false;
                }


                if (added && !diagsCreated)
                {
                    diagsCreated = true;
                }
            }


            return diagsCreated;
        }

        public bool CreateHospitalAppointmentSchedule(HospitalAppointmentSchedule appt)
        {
            bool apptCreated = false;

            if (!appt.member_id.Equals(Guid.Empty))
            {
                _icmsContext.HospitalAppointmentSchedules.Add(appt);

                apptCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
            }


            return apptCreated;
        }



        private string generateRegistrationNumber()
        {
            string registrationNumber = "";

            NextAdmissionId nextAdmissionId = (from admitId in _icmsContext.NextAdmissionIds
                                                 select admitId).FirstOrDefault();

            if (nextAdmissionId.next_admission_id > 0)
            {
                int nextAdmitId = (int)nextAdmissionId.next_admission_id;

                nextAdmissionId.next_admission_id = nextAdmitId + 1;

                _icmsContext.NextAdmissionIds.Attach(nextAdmissionId);
                _icmsContext.Entry(nextAdmissionId).State = EntityState.Modified;
                int updated = _icmsContext.SaveChanges();


                if (updated > 0)
                {
                    registrationNumber = "ADM" + nextAdmitId.ToString();
                }
            }


            return registrationNumber;
        }
    }
}

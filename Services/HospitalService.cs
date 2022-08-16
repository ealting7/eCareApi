using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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


        public IcmsUser getLoggedInUser(Guid userId)
        {
            IcmsUser userLoggedIn = null;

            StandardService standServ = new StandardService(_icmsContext, _emrContext);
            userLoggedIn = standServ.getAspUser(userId);

            return userLoggedIn;
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

        public List<Room> getHospitalDepartmentRooms(string departmentId)
        {
            List<Room> departmentRooms = null;
            int deptId = 0;

            if (int.TryParse(departmentId, out deptId))
            {
                departmentRooms = (
                                    from deptrooms in _icmsContext.HospitalDepartmentRooms
                                    where deptrooms.hospital_department_id.Equals(deptId)
                                    select new Room
                                    {
                                        departmentRoomsId = deptrooms.hospital_department_rooms_id,
                                        roomName = deptrooms.name,
                                        roomOccupancy = deptrooms.occupancy,
                                        roomAvailable = deptrooms.room_available,
                                    }
                                  )
                                  .ToList();
            }

            if (departmentRooms != null && departmentRooms.Count > 0)
            {
                loadRoomOccupancy(ref departmentRooms);
            }

            return departmentRooms;
        }

        private void loadRoomOccupancy(ref List<Room>departmentRooms)
        {
            foreach(Room room in departmentRooms)
            {
                List<Patient> patientsInroom = (
                                                    from roomRef in _icmsContext.HospitalDepartmentRoomsReferences

                                                    join patient in _icmsContext.Patients
                                                    on roomRef.member_id equals patient.member_id into pats
                                                    from inptpats in pats.DefaultIfEmpty()

                                                    where roomRef.hospital_department_rooms_id.Equals(room.departmentRoomsId)
                                                    select new Patient
                                                    {
                                                        PatientId = roomRef.member_id,
                                                        firstName = inptpats.member_first_name,
                                                        lastName = inptpats.member_last_name,
                                                        FullName = inptpats.member_first_name + " " + inptpats.member_last_name,
                                                        dateOfBirthDisplay = (inptpats.member_birth.HasValue) ? inptpats.member_birth.Value.ToShortDateString() : "",
                                                        dateOfBirth = inptpats.member_birth
                                                    }
                                                )
                                                .Distinct()
                                                .ToList();

                if (patientsInroom != null)
                {
                    room.patientsInRoom = patientsInroom;
                }
            }
        }

        public List<HospitalDepartment> getHospitalDepartmentsAsset(int hospitalId)
        {
            List<HospitalDepartment> hospitalDepartments = null;

            if (hospitalId > 0)
            {

                hospitalDepartments = (
                                            from depts in _icmsContext.HospitalDepartments
                                            where depts.hospital_id.Equals(hospitalId)
                                            select depts
                                      )
                                      .ToList();
                
            }

            return hospitalDepartments;
        }

        public List<Hospital> GetHospitals()
        {
            List<Hospital> hospitals = null;

            hospitals = (
                
                from hosp in _icmsContext.Hospitals
                where (hosp.deleted.Equals(false) || hosp.deleted == null)
                orderby hosp.name
                select hosp
            )
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


        public List<Specimen> getSpecimenTypes()
        {
            List<Specimen> specimenTypes = null;

            specimenTypes = (
                                from spectyp in _icmsContext.HospitalOrderSpecimenTypes
                                where (spectyp.disabled.Equals(false) || !spectyp.disabled.HasValue)
                                orderby spectyp.specimen_type
                                select new Specimen
                                {
                                    specimenTypeName = spectyp.specimen_type,
                                    specimenTypeId = spectyp.hospital_order_specimen_type_id
                                }
                            )
                            .ToList();

            return specimenTypes;
        }

        public List<Specimen> getSpecimenVolumes()
        {
            List<Specimen> volumes = null;

            volumes = (
                        from specvol in _icmsContext.HospitalMedicationStrengthUnitTypes
                        orderby specvol.strength_name
                        select new Specimen
                        {
                            specimenVolumeName = specvol.strength_abbrev,
                            specimenVolumeId = specvol.hospital_medication_strength_unit_type_id
                        }
                      )
                      .ToList();

            return volumes;
        }


        public List<ErVisit> getHospitalErVisits(string hospitalId)
        {
            List<ErVisit> erVisits = null;

            int hospId = 0;

            if (int.TryParse(hospitalId, out hospId))
            {
                erVisits = (
                                from er in _icmsContext.SimsErs

                                join erStatus in _icmsContext.SimsErStatuses
                                on er.sims_er_status_id equals erStatus.sims_er_status_id into statuses
                                from erStatuses in statuses.DefaultIfEmpty()

                                join erRoom in _icmsContext.SimsErRooms
                                on er.sims_er_room_id equals erRoom.sims_er_room_id into rooms
                                from erRooms in rooms.DefaultIfEmpty()

                                join pat in _icmsContext.Patients
                                on er.member_id equals pat.member_id into pats
                                from patients in pats.DefaultIfEmpty()

                                where er.hospital_id.Equals(hospId)
                                select new ErVisit
                                {
                                    erId = er.sims_er_id,
                                    patientId = er.member_id,
                                    erPatient = new Patient
                                    {
                                        firstName = patients.member_first_name,
                                        lastName = patients.member_last_name,
                                        FullName = patients.member_first_name + " " + patients.member_last_name
                                    },
                                    visitReason = er.reason_for_visit,
                                    ambulanceUsed = (er.ambulance_used.HasValue) ? (er.ambulance_used > 0) ? true : false : false,
                                    erStatusId = er.sims_er_status_id,
                                    erStatus = erStatuses.name,
                                    roomId = er.sims_er_room_id,
                                    roomName = erRooms.name,
                                    roomPrefix = erRooms.room_prefix,
                                    checkInDate = er.check_in_date,
                                    displayCheckInDate = (er.check_in_date.HasValue) ? er.check_in_date.Value.ToShortDateString() : "",
                                    checkOutDdate = er.check_out_date,
                                    displayCheckOutDate = (er.check_out_date.HasValue) ? er.check_out_date.Value.ToShortDateString() : ""
                                }
                           )
                           .Distinct()
                           .ToList();
            }

            return erVisits;
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

        public Admission getInpatientAdmission(Admission admit)
        {
            Admission patientAdmission = null;

            if (admit.hospital_inpatient_admission_id > 0)
            {

                patientAdmission = (
                        from inptadmits in _icmsContext.HospitalInpatientAdmissions

                        join visitreason in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                        on inptadmits.hospital_inpatient_admission_id equals visitreason.hospital_inpatient_admission_id into reasons
                        from reasonforvisit in reasons.DefaultIfEmpty()

                        join icd10 in _icmsContext.DiagnosisCodes10
                        on inptadmits.admission_diagnosis_code_id equals icd10.diagnosis_codes_10_id into icdcodes
                        from icd10s in icdcodes.DefaultIfEmpty()

                        join hosp in _icmsContext.Hospitals 
                        on inptadmits.hospital_id equals hosp.hospital_id into hospitals 
                        from hosps in hospitals.DefaultIfEmpty()

                        join dept in _icmsContext.HospitalDepartments
                        on inptadmits.hospital_department_id equals dept.hospital_department_id into departments
                        from depts in departments.DefaultIfEmpty()

                        join deptRoom in _icmsContext.HospitalDepartmentRooms
                        on inptadmits.hospital_department_rooms_id equals deptRoom.hospital_department_rooms_id into rooms
                        from deptrooms in rooms.DefaultIfEmpty() 

                        where inptadmits.hospital_inpatient_admission_id.Equals(admit.hospital_inpatient_admission_id)

                        select new Admission
                        {
                            hospital_inpatient_admission_id = inptadmits.hospital_inpatient_admission_id,
                            hospital_id = inptadmits.hospital_id,
                            hospitalName = hosps.name,
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
                            departmentName = depts.hospital_department_name,
                            departmentRoomName = deptrooms.name,
                            displayTodaysDate = DateTime.Now.DayOfWeek + " " + DateTime.Now.ToShortDateString()
                        }
                    )
                    .FirstOrDefault();

                if (patientAdmission != null && !patientAdmission.member_id.Equals(Guid.Empty))
                {
                    patientAdmission.patient = getAdmissionPatientInfo(patientAdmission);
                    patientAdmission.vitalSigns = getInpatientAdmissionVitalSigns(admit.hospital_inpatient_admission_id);
                    //patientAdmission.departmentRooms = getAdmissionDepartmentRooms(patientAdmission);

                    patientAdmission.userInpatientAdmissionDashboardDefaults = getInpatientAdmissionUserDefaults(admit);


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

                decimal height = getPatientHeight(admit.member_id);

                if (height > 0)
                {
                    admitPatient.displayHeight = convertHeightToDisplayHeight(height);
                }

                decimal weight = getPatientWeight(admit.member_id);

                if (weight > 0)
                {
                    admitPatient.displayWeight = convertWeightToDisplayWeight(weight);
                }
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

        public InpatientChart getInpatientChart(InpatientChart searchChart)
        {
            InpatientChart returnChart = null;


            if (searchChart.chartId > 0)
            {
                returnChart = (
                        from inptchrts in _icmsContext.HospitalInpatientAdmissionCharts
                        where inptchrts.hospital_inpatient_admission_chart_id.Equals(searchChart.chartId)
                        select new InpatientChart
                        {
                            chartId = inptchrts.hospital_inpatient_admission_chart_id,
                            chartName = inptchrts.chart_name,
                            chartOrder = inptchrts.display_order,
                            chartTableName = inptchrts.chart_table_name,
                            chartType = inptchrts.chart_type,
                            rationale = inptchrts.rationale
                        }
                    )
                    .Take(1)
                    .FirstOrDefault();

                if (returnChart != null)
                {

                    List<InpatientChartSource> sources = getInpatientChartSources(searchChart);
                    
                    foreach(InpatientChartSource source in sources)
                    {
                     
                        if ((!string.IsNullOrEmpty(source.loaderTableName) &&
                             !string.IsNullOrEmpty(source.loaderIdColumnName) &&
                             !string.IsNullOrEmpty(source.loaderDescriptionColumnName)))
                        {
                            source.loaderForSource = getSourceItemLoader(source);
                        }                        

                    }

                    returnChart.sources = sources;
                    returnChart.dailySources = getDailyCharts(searchChart, returnChart);

                    getInpatientDetailSourceData(searchChart, ref returnChart);

                }

            }

            return returnChart;
        }

        private List<InpatientChartSource> getDailyCharts(InpatientChart searchChart, InpatientChart chart)
        {
            List<InpatientChartSource> returnDailySources = null;            

            if (searchChart.admissionId > 0 && chart.sources != null && !string.IsNullOrEmpty(chart.chartTableName) && 
                !string.IsNullOrEmpty(searchChart.date))
            {
                switch (chart.chartTableName)
                {
                    case "MEMBER_VITALS":
                        returnDailySources = getVitalSignDailyCharts(searchChart, chart);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                        returnDailySources = getCardiacAssessmentDailyCharts(searchChart, chart);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                        returnDailySources = getNeurologicalAssessmentDailyCharts(searchChart, chart);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                        returnDailySources = getBloodGasAssessmentDailyCharts(searchChart, chart);
                        break;
                }                

            }

            return returnDailySources;
        }

        private List<InpatientChartSource> getVitalSignDailyCharts(InpatientChart searchChart, InpatientChart chart)
        {

            List<InpatientChartSource> returnDailySources = null;

            DateTime dteMeasured = DateTime.MinValue;

            if (DateTime.TryParse(searchChart.date, out dteMeasured))
            {

                //get an hourly chart/table record for the chart.chartTableName for the given day
                DateTime startDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 12:00 AM");
                DateTime endDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 11:59 PM");


                List<MemberVitals> dbVitals = (
                                                from vitals in _icmsContext.MemberVitalses
                                                where vitals.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                                                && (vitals.date_measured > startDate && vitals.date_measured < endDate)
                                                select vitals
                                               )
                                               .ToList();

                if (dbVitals != null)
                {

                    returnDailySources = new List<InpatientChartSource>();


                    foreach (MemberVitals vital in dbVitals)
                    {

                        //each source/control
                        foreach (InpatientChartSource source in chart.sources)
                        {
                            InpatientChartSource dailyChart = new InpatientChartSource();
                            dailyChart.chartTableId = vital.member_vitals_id;
                            dailyChart.hour = vital.date_measured.Hour;
                            dailyChart.minute = vital.date_measured.Minute;
                            dailyChart.date = vital.date_measured.ToShortDateString();
                            dailyChart.admissionId = (int)vital.hospital_inpatient_admission_id;

                            loadCommonDailyChartSourceItems(ref dailyChart, source);                            

                            switch (source.modelVariableName)
                            {
                                case "temperature":
                                    dailyChart.sourceDecimalValue = (vital.temperature_in_fahrenheit != null) ? (decimal)vital.temperature_in_fahrenheit : 0;
                                    break;
                                case "temperatureSiteId":
                                    dailyChart.sourceIntValue = (vital.hospital_temperature_site_id != null) ? (int)vital.hospital_temperature_site_id : 0;
                                    break;
                                case "temperatureManagement":
                                    dailyChart.sourceTextValue = vital.temperature_management;
                                    break;
                                case "heartRate":
                                    dailyChart.sourceTextValue = vital.heart_rate;
                                    break;
                                case "heartRhythm":
                                    dailyChart.sourceIntValue = (vital.hospital_pulse_rhythm_id != null) ? (int)vital.hospital_pulse_rhythm_id : 0;
                                    break;
                                case "systolicBloodPressure":
                                    dailyChart.sourceIntValue = (vital.seated_blood_pressure_systolic != null) ? (int)vital.seated_blood_pressure_systolic : 0;
                                    break;
                                case "diastolicBloodPressure":
                                    dailyChart.sourceIntValue = (vital.seated_blood_pressure_diastolic != null) ? (int)vital.seated_blood_pressure_diastolic : 0;
                                    break;
                                case "meanArterialPressure":
                                    dailyChart.sourceIntValue = (vital.mean_arterial_pressure != null) ? (int)vital.mean_arterial_pressure : 0;
                                    break;
                                case "nonInvasiveBloodPressure":
                                    dailyChart.sourceTextValue = vital.non_invasive_blood_pressure;
                                    break;
                                case "respirationRate":
                                    dailyChart.sourceIntValue = (vital.respiration_per_minute != null) ? (int)vital.respiration_per_minute : 0;
                                    break;
                                case "fi02":
                                    dailyChart.sourceIntValue = (vital.fi02 != null) ? (int)vital.fi02 : 0;
                                    break;
                                case "sp02":
                                    dailyChart.sourceTextValue = vital.sp02;
                                    break;
                                case "etco02":
                                    dailyChart.sourceTextValue = vital.etc02;
                                    break;
                            }


                            if (source.controlType.Equals("select") && !string.IsNullOrEmpty(source.loaderTableName) &&
                                !string.IsNullOrEmpty(source.loaderIdColumnName))
                            {
                                dailyChart.dailyChartSelectValue = getSelectControlDataLoaderValue(dailyChart);
                            }

                            dailyChart.highlightSource = vital.highlight;
                            dailyChart.highlightColor = vital.highlight_color;

                            dailyChart.mdtNote = getVitalSignSourceMtdNote(source, vital);
                            getVitalSignSourceHighlightAndColor(ref dailyChart, vital);

                            returnDailySources.Add(dailyChart);
                        }

                    }

                }
            }

            return returnDailySources;

        }

        private List<InpatientChartSource> getCardiacAssessmentDailyCharts(InpatientChart searchChart, InpatientChart chart)
        {

            List<InpatientChartSource> returnDailySources = null;

            DateTime dteMeasured = DateTime.MinValue;

            if (DateTime.TryParse(searchChart.date, out dteMeasured))
            {

                //get an hourly chart/table record for the chart.chartTableName for the given day
                DateTime startDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 12:00 AM");
                DateTime endDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 11:59 PM");


                List<HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac> dbCardiacAssessment = (

                                from cardiac in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs
                                where cardiac.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                                && (cardiac.creation_date > startDate && cardiac.creation_date < endDate)
                                select cardiac
                            )
                            .ToList();

                if (dbCardiacAssessment != null)
                {

                    returnDailySources = new List<InpatientChartSource>();

                    foreach (HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment in dbCardiacAssessment)
                    {

                        //each source/control
                        foreach (InpatientChartSource source in chart.sources)
                        {
                            InpatientChartSource dailyChart = new InpatientChartSource();
                            dailyChart.chartTableId = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id;
                            dailyChart.hour = assessment.date_measured.Value.Hour;
                            dailyChart.minute = assessment.date_measured.Value.Minute;

                            dailyChart.date = assessment.date_measured.Value.ToShortDateString();
                            dailyChart.admissionId = (int)assessment.hospital_inpatient_admission_id;

                            loadCommonDailyChartSourceItems(ref dailyChart, source);

                            switch (source.modelVariableName)
                            {
                                case "cardiacOutput":
                                    dailyChart.sourceTextValue = assessment.cardiac_output;
                                    break;
                                case "cardiacIndex":
                                    dailyChart.sourceTextValue = assessment.cardiac_index;
                                    break;
                                case "cardiacStrokeVolume":
                                    dailyChart.sourceTextValue = assessment.stroke_volume ;
                                    break;
                                case "cardiacStrokeVolumeIndex":
                                    dailyChart.sourceTextValue = assessment.stroke_volume_index;
                                    break;
                                case "cardiacNailClubbing":
                                    dailyChart.sourceBoolValue = (bool)assessment.nail_clubbing;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacEdemaSymptoms":
                                    dailyChart.sourceBoolValue = (bool)assessment.edema_symptoms;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacPulsesNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.pulses_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacAorticSoundNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.aortic_sound_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacPulmonicSoundNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.pulmonic_sound_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacErbsPointSoundNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.erb_point_sound_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacTricuspidSoundNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.tricuspid_sound_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacApicalPulseSoundNormal":
                                    dailyChart.sourceBoolValue = (bool)assessment.apical_pulse_sound_normal;
                                    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                    break;
                            }


                            if (source.controlType.Equals("select") && !string.IsNullOrEmpty(source.loaderTableName) &&
                                !string.IsNullOrEmpty(source.loaderIdColumnName))
                            {
                                dailyChart.dailyChartSelectValue = getSelectControlDataLoaderValue(dailyChart);
                            }


                            dailyChart.mdtNote = getCardiacSourceMtdNote(source, assessment);
                            getCardiacSourceHighlightAndColor(ref dailyChart, assessment);

                            returnDailySources.Add(dailyChart);
                        }

                    }

                }
            }

            return returnDailySources;

        }

        private List<InpatientChartSource> getNeurologicalAssessmentDailyCharts(InpatientChart searchChart, InpatientChart chart)
        {

            List<InpatientChartSource> returnDailySources = null;

            DateTime dteMeasured = DateTime.MinValue;

            if (DateTime.TryParse(searchChart.date, out dteMeasured))
            {

                //get an hourly chart/table record for the chart.chartTableName for the given day
                DateTime startDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 12:00 AM");
                DateTime endDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 11:59 PM");

                List<HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological> dbAssessments = (

                                from assess in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals
                                where assess.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                                && (assess.creation_date > startDate && assess.creation_date < endDate)
                                select assess
                            )
                            .ToList();

                if (dbAssessments != null)
                {

                    returnDailySources = new List<InpatientChartSource>();

                    foreach (HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment in dbAssessments)
                    {

                        //each source/control
                        foreach (InpatientChartSource source in chart.sources)
                        {
                            InpatientChartSource dailyChart = new InpatientChartSource();
                            dailyChart.chartTableId = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id;
                            dailyChart.hour = assessment.date_measured.Value.Hour;
                            dailyChart.minute = assessment.date_measured.Value.Minute;
                            dailyChart.date = assessment.date_measured.Value.ToShortDateString();
                            dailyChart.admissionId = (int)assessment.hospital_inpatient_admission_id;

                            loadCommonDailyChartSourceItems(ref dailyChart, source);

                            switch (source.modelVariableName)
                            {
                                case "neurologyHandSqueezeStrengthId":
                                    dailyChart.sourceIntValue = (assessment.hand_squeeze_strength_id != null) ? (int)assessment.hand_squeeze_strength_id : 0;
                                    break;

                                //case "cardiacIndex":
                                //    dailyChart.sourceTextValue = assessment.cardiac_index;
                                //    break;
                                
                                //case "cardiacApicalPulseSoundNormal":
                                //    dailyChart.sourceBoolValue = (bool)assessment.apical_pulse_sound_normal;
                                //    dailyChart.controlLabel = (dailyChart.sourceBoolValue) ? "Yes" : "No";
                                //    break;
                            }


                            if (source.controlType.Equals("select") && !string.IsNullOrEmpty(source.loaderTableName) &&
                                !string.IsNullOrEmpty(source.loaderIdColumnName))
                            {
                                dailyChart.dailyChartSelectValue = getSelectControlDataLoaderValue(dailyChart);
                            }
                            
                            dailyChart.mdtNote = getNeuroSourceMtdNote(source, assessment);
                            getNeurologicalSourceHighlightAndColor(ref dailyChart, assessment);

                            returnDailySources.Add(dailyChart);
                        }

                    }

                }
            }

            return returnDailySources;

        }

        private List<InpatientChartSource> getBloodGasAssessmentDailyCharts(InpatientChart searchChart, InpatientChart chart)
        {

            List<InpatientChartSource> returnDailySources = null;

            DateTime dteMeasured = DateTime.MinValue;

            if (DateTime.TryParse(searchChart.date, out dteMeasured))
            {

                //get an hourly chart/table record for the chart.chartTableName for the given day
                DateTime startDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 12:00 AM");
                DateTime endDate = Convert.ToDateTime(dteMeasured.ToShortDateString() + " 11:59 PM");

                List<HospitalInpatientAdmissionBloodGas> dbAssessments = (

                                from assess in _icmsContext.HospitalInpatientAdmissionBloodGases
                                where assess.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                                && (assess.creation_date > startDate && assess.creation_date < endDate)
                                select assess
                            )
                            .ToList();

                if (dbAssessments != null)
                {

                    returnDailySources = new List<InpatientChartSource>();

                    foreach (HospitalInpatientAdmissionBloodGas assessment in dbAssessments)
                    {

                        //each source/control
                        foreach (InpatientChartSource source in chart.sources)
                        {
                            InpatientChartSource dailyChart = new InpatientChartSource();
                            dailyChart.chartTableId = assessment.hosptial_inpatient_admission_blood_gas_id;
                            dailyChart.hour = assessment.date_measured.Value.Hour;
                            dailyChart.minute = assessment.date_measured.Value.Minute;
                            dailyChart.date = assessment.date_measured.Value.ToShortDateString();
                            dailyChart.admissionId = (int)assessment.hospital_inpatient_admission_id;

                            loadCommonDailyChartSourceItems(ref dailyChart, source);

                            switch (source.modelVariableName)
                            {
                                case "bloodGasPh":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.ph)) ? assessment.ph : "";
                                    break;
                                case "bloodGasPc02":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.pco2)) ? assessment.pco2 : "";
                                    break;
                                case "bloodGasP02":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.po2)) ? assessment.po2 : "";
                                    break;
                                case "bloodGasHc03":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.hco3)) ? assessment.hco3 : "";
                                    break;
                                case "bloodGasBaseExcess":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.base_excess)) ? assessment.base_excess : "";
                                    break;
                                case "bloodGasHb":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.hb)) ? assessment.hb : "";
                                    break;
                                case "bloodGas02Saturation":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.o2_saturation)) ? assessment.o2_saturation : "";
                                    break;
                                case "bloodGasSodium":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.sodium)) ? assessment.sodium : "";
                                    break;
                                case "bloodGasPotassium":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.potassium)) ? assessment.potassium : "";
                                    break;
                                case "bloodGasCalcium":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.calcium)) ? assessment.calcium : "";
                                    break;
                                case "bloodGasBloodSugar":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.blood_sugar)) ? assessment.blood_sugar : "";
                                    break;
                                case "bloodGasLactate":
                                    dailyChart.sourceTextValue = (!string.IsNullOrEmpty(assessment.lactate)) ? assessment.lactate : "";
                                    break;
                            }


                            if (source.controlType.Equals("select") && !string.IsNullOrEmpty(source.loaderTableName) &&
                                !string.IsNullOrEmpty(source.loaderIdColumnName))
                            {
                                dailyChart.dailyChartSelectValue = getSelectControlDataLoaderValue(dailyChart);
                            }

                            dailyChart.mdtNote = getBloodGasSourceMtdNote(source, assessment);
                            getBloodGasSourceHighlightAndColor(ref dailyChart, assessment);

                            returnDailySources.Add(dailyChart);
                        }

                    }

                }
            }

            return returnDailySources;

        }
        



        private Note getVitalSignSourceMtdNote(InpatientChartSource source, MemberVitals vital)
        {
            Note note = null;

            if (vital.member_vitals_id > 0)
            {

                HospitalInpatientAdmissionMdtNoteVitalSignReference vitalRef = (
                        from vtlRef in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                        where vtlRef.member_vitals_id.Equals(vital.member_vitals_id)
                        && vtlRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        && vtlRef.hospital_inpatient_admission_id.Equals(vital.hospital_inpatient_admission_id)
                        select vtlRef
                    )
                    .FirstOrDefault();

                if (vitalRef != null)
                {

                    note = (
                        from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                        join hospnotetyp in _icmsContext.HospitalNoteTypes
                        on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                        where hospInptAdmMdt.hosptial_inpatient_admission_mdt_note.Equals(vitalRef.hosptial_inpatient_admission_mdt_note)
                        select new Note
                        {
                            noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                            recordDate = hospInptAdmMdt.creation_date,
                            displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                                hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                                : "N/A",
                            caseOwnerId = hospInptAdmMdt.creation_user_id,
                            noteText = hospInptAdmMdt.hospital_note,
                            hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                            hospitalNoteTypeName = hospnotetyp.note_type_name
                        }
                    )
                    .FirstOrDefault();                 
                }

            }

            return note;
        }

        private void getVitalSignSourceHighlightAndColor(ref InpatientChartSource source, MemberVitals vital)
        {

            if (vital.member_vitals_id > 0)
            {

                int sourceId = source.sourceId;

                HospitalInpatientAdmissionMdtNoteVitalSignReference vitalRef = (
                        from vtlRef in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                        where vtlRef.member_vitals_id.Equals(vital.member_vitals_id)
                        && vtlRef.hospital_inpatient_admission_chart_source_id.Equals(sourceId)
                        && vtlRef.hospital_inpatient_admission_id.Equals(vital.hospital_inpatient_admission_id)
                        select vtlRef
                    )
                    .FirstOrDefault();

                if (vitalRef != null)
                {
                    source.highlightSource = (vitalRef.highlight != null) ? vitalRef.highlight : false;
                    source.highlightColor = (vitalRef.highlight_color!= null) ? vitalRef.highlight_color : "";
                }
            }
        }

        private Note getCardiacSourceMtdNote(InpatientChartSource source, HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment)
        {
            Note note = null;

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id > 0)
            {

                HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference cardiacRef = (
                        from crdRef in _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences
                        where crdRef.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id)
                        && crdRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        && crdRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select crdRef
                    )
                    .FirstOrDefault();

                if (cardiacRef != null)
                {

                    note = (
                        from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                        join hospnotetyp in _icmsContext.HospitalNoteTypes
                        on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                        where hospInptAdmMdt.hosptial_inpatient_admission_mdt_note.Equals(cardiacRef.hosptial_inpatient_admission_mdt_note)
                        select new Note
                        {
                            noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                            recordDate = hospInptAdmMdt.creation_date,
                            displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                                hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                                : "N/A",
                            caseOwnerId = hospInptAdmMdt.creation_user_id,
                            noteText = hospInptAdmMdt.hospital_note,
                            hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                            hospitalNoteTypeName = hospnotetyp.note_type_name
                        }
                    )
                    .FirstOrDefault();
                }

            }

            return note;
        }

        private void getCardiacSourceHighlightAndColor(ref InpatientChartSource source, HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment)
        {

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id > 0)
            {

                int sourceId = source.sourceId;

                HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference references = (
                        from highlightRef in _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences
                        where highlightRef.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id)
                        && highlightRef.hospital_inpatient_admission_chart_source_id.Equals(sourceId)
                        && highlightRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select highlightRef
                    )
                    .FirstOrDefault();

                if (references != null)
                {
                    source.highlightSource = (references.highlight != null) ? references.highlight : false;
                    source.highlightColor = (references.highlight_color != null) ? references.highlight_color : "";
                }
            }
        }

        private Note getNeuroSourceMtdNote(InpatientChartSource source, HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment)
        {
            Note note = null;

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id > 0)
            {

                HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference neuroRef = (
                        from nroRef in _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences
                        where nroRef.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id)
                        && nroRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        && nroRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select nroRef
                    )
                    .FirstOrDefault();

                if (neuroRef != null)
                {

                    note = (
                        from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                        join hospnotetyp in _icmsContext.HospitalNoteTypes
                        on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                        where hospInptAdmMdt.hosptial_inpatient_admission_mdt_note.Equals(neuroRef.hosptial_inpatient_admission_mdt_note)
                        select new Note
                        {
                            noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                            recordDate = hospInptAdmMdt.creation_date,
                            displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                                hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                                : "N/A",
                            caseOwnerId = hospInptAdmMdt.creation_user_id,
                            noteText = hospInptAdmMdt.hospital_note,
                            hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                            hospitalNoteTypeName = hospnotetyp.note_type_name
                        }
                    )
                    .FirstOrDefault();
                }

            }

            return note;
        }

        private void getNeurologicalSourceHighlightAndColor(ref InpatientChartSource source, HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment)
        {

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id > 0)
            {

                int sourceId = source.sourceId;

                HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference references = (
                        from highlightRef in _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences
                        where highlightRef.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id)
                        && highlightRef.hospital_inpatient_admission_chart_source_id.Equals(sourceId)
                        && highlightRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select highlightRef
                    )
                    .FirstOrDefault();

                if (references != null)
                {
                    source.highlightSource = (references.highlight != null) ? references.highlight : false;
                    source.highlightColor = (references.highlight_color != null) ? references.highlight_color : "";
                }
            }
        }

        private Note getBloodGasSourceMtdNote(InpatientChartSource source, HospitalInpatientAdmissionBloodGas assessment)
        {
            Note note = null;

            if (assessment.hosptial_inpatient_admission_blood_gas_id > 0)
            {

                HospitalInpatientAdmissionMdtNoteBloodGasReference bloodGasRef = (
                        from nroRef in _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences
                        where nroRef.hosptial_inpatient_admission_blood_gas_id.Equals(assessment.hosptial_inpatient_admission_blood_gas_id)
                        && nroRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        && nroRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select nroRef
                    )
                    .FirstOrDefault();

                if (bloodGasRef != null)
                {

                    note = (
                        from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                        join hospnotetyp in _icmsContext.HospitalNoteTypes
                        on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                        where hospInptAdmMdt.hosptial_inpatient_admission_mdt_note.Equals(bloodGasRef.hosptial_inpatient_admission_mdt_note)
                        select new Note
                        {
                            noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                            recordDate = hospInptAdmMdt.creation_date,
                            displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                                hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                                : "N/A",
                            caseOwnerId = hospInptAdmMdt.creation_user_id,
                            noteText = hospInptAdmMdt.hospital_note,
                            hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                            hospitalNoteTypeName = hospnotetyp.note_type_name
                        }
                    )
                    .FirstOrDefault();
                }

            }

            return note;
        }

        private void getBloodGasSourceHighlightAndColor(ref InpatientChartSource source, HospitalInpatientAdmissionBloodGas assessment)
        {

            if (assessment.hosptial_inpatient_admission_blood_gas_id > 0)
            {

                int sourceId = source.sourceId;

                HospitalInpatientAdmissionMdtNoteBloodGasReference references = (
                        from highlightRef in _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences
                        where highlightRef.hosptial_inpatient_admission_blood_gas_id.Equals(assessment.hosptial_inpatient_admission_blood_gas_id)
                        && highlightRef.hospital_inpatient_admission_chart_source_id.Equals(sourceId)
                        && highlightRef.hospital_inpatient_admission_id.Equals(assessment.hospital_inpatient_admission_id)
                        select highlightRef
                    )
                    .FirstOrDefault();

                if (references != null)
                {
                    source.highlightSource = (references.highlight != null) ? references.highlight : false;
                    source.highlightColor = (references.highlight_color != null) ? references.highlight_color : "";
                }
            }
        }




        private void loadCommonDailyChartSourceItems(ref InpatientChartSource dailyChart, InpatientChartSource source)
        {
            dailyChart.controlClass = source.controlClass;
            dailyChart.controlType = source.controlType;
            dailyChart.loaderDescriptionColumnName = source.loaderDescriptionColumnName;
            dailyChart.loaderIdColumnName = source.loaderIdColumnName;
            dailyChart.loaderTableName = source.loaderTableName;
            dailyChart.modelVariableName = source.modelVariableName;
            dailyChart.sourceId = source.sourceId;
            dailyChart.sourceName = source.sourceName;
            dailyChart.sourceNameAbbrev = source.sourceNameAbbrev;
            dailyChart.sourceOrder = source.sourceOrder;
            dailyChart.controlPattern = source.controlPattern;
            dailyChart.maxLength = source.maxLength;
            dailyChart.placeholder = source.placeholder;
            dailyChart.textareaCols = source.textareaCols;
            dailyChart.textareaRows = source.textareaRows;
            dailyChart.controlGroupName = source.controlGroupName;
            dailyChart.controlMin = source.controlMin;
            dailyChart.controlMax = source.controlMax;
            dailyChart.controlStep = source.controlStep;
            dailyChart.controlValue = source.controlValue;
            dailyChart.controlLabel = source.controlLabel;
            dailyChart.controlChecked = source.controlChecked;
        }

        private void getInpatientDetailSourceData(InpatientChart searchChart, ref InpatientChart details)
        {

            if (searchChart.admissionId > 0 && details.sources != null && !string.IsNullOrEmpty(details.chartType))
            {

                switch (details.chartType)
                {
                    case "demographicChart":
                        getInpatientDemographicDetails(searchChart, ref details);
                        break;
                    case "admissionChart":
                        getInpatientAdmissionDetails(searchChart, ref details);
                        break;
                }

            }

        }

        private void getInpatientDemographicDetails(InpatientChart searchChart, ref InpatientChart chart)
        {

            if (searchChart.admissionId > 0 && !searchChart.patientId.Equals(Guid.Empty))
            {
                PatientService patServ = new PatientService(_icmsContext, _emrContext);
                Patient inptPatient = patServ.getAdmissionPatientsUsingId((Guid)searchChart.patientId);

                if (inptPatient != null)
                {

                    PatientContacts inptContact = getAdmissionContact(searchChart);
                    Doctor generalProvider = patServ.getPatientGeneralProvider((Guid)searchChart.patientId);

                    inptPatient.medicalHistory = patServ.getPatientMedicalHistory((Guid)searchChart.patientId);
                    inptPatient.familyMedicalHistory = patServ.getPatientFamilyMedicalHistory((Guid)searchChart.patientId);
                    inptPatient.nextOfKin = patServ.getPatientNextOfKin((Guid)searchChart.patientId);

                    foreach (InpatientChartSource source in chart.sources)
                    {
                        switch (source.modelVariableName)
                        {
                            case "firstName":
                                source.sourceTextValue = (!string.IsNullOrEmpty(inptPatient.firstName)) ? inptPatient.firstName : "";
                                break;

                            case "lastName":
                                source.sourceTextValue = (!string.IsNullOrEmpty(inptPatient.lastName)) ? inptPatient.lastName : "";
                                break;

                            case "dateOfBirthDisplay":
                                source.sourceTextValue = (inptPatient.dateOfBirth.Value != null && !inptPatient.dateOfBirth.Value.Equals(DateTime.MinValue)) ? 
                                                                inptPatient.dateOfBirth.Value.ToShortDateString() : "";
                                break;

                            case "age":
                                source.sourceIntValue = inptPatient.age;
                                break;

                            case "mrn":
                                source.sourceTextValue = (!string.IsNullOrEmpty(inptPatient.mrn)) ? inptPatient.mrn : "";
                                break;

                            case "addresses.address_line1":
                                if (inptPatient.addresses != null && inptPatient.addresses.Any())
                                {
                                    foreach(MemberAddress addr in inptPatient.addresses)
                                    {
                                        source.sourceTextValue = addr.address_line1;
                                        source.sourceTextValue += addr.address_line2;
                                        break;
                                    }
                                }

                                break;

                            case "addresses.city":
                                if (inptPatient.addresses != null && inptPatient.addresses.Any())
                                {
                                    foreach (MemberAddress addr in inptPatient.addresses)
                                    {
                                        source.sourceTextValue = addr.city;
                                        break;
                                    }
                                }
                                break;

                            case "addresses.state_abbrev":
                                if (inptPatient.addresses != null && inptPatient.addresses.Any())
                                {
                                    foreach (MemberAddress addr in inptPatient.addresses)
                                    {
                                        source.sourceTextValue = addr.state_abbrev;
                                        break;
                                    }
                                }
                                break;

                            case "hospitalContacts.fullName":
                                source.sourceTextValue = (inptContact != null) ? inptContact.firstName + " " + inptContact.lastName : "";
                                break;

                            case "hospitalContacts.phoneNumber":
                                source.sourceTextValue = (inptContact != null) ? inptContact.phoneNumber : "";
                                break;

                            case "generalProvider.fullName":
                                source.sourceTextValue = (generalProvider != null) ?  generalProvider.fullName : "";
                                break;

                            case "generalProvider.phoneNumber":
                                source.sourceTextValue = (generalProvider != null) ? generalProvider.phoneNumber : "";
                                break;

                            case "medicalHistory.history":
                                source.sourceTextValue = (inptPatient.medicalHistory != null) ? inptPatient.medicalHistory.history : "";
                                break;

                            case "familyMedicalHistory.history":
                                source.sourceTextValue = (inptPatient.familyMedicalHistory != null) ? inptPatient.familyMedicalHistory.history : "";
                                break;

                            case "nextOfKin.fullName":
                                source.sourceTextValue = (inptPatient.nextOfKin != null) ? inptPatient.nextOfKin.fullName : "";
                                break;

                            case "nextOfKin.relationshipId":
                                source.sourceIntValue = (inptPatient.nextOfKin != null && inptPatient.nextOfKin.relationshipId > 0) ? inptPatient.nextOfKin.relationshipId : 0;
                                break;

                            case "nextOfKin.phoneNumber":
                                source.sourceTextValue = (inptPatient.nextOfKin != null) ? inptPatient.nextOfKin.phoneNumber: "";
                                break;

                        }
                    }

                }

            }

        }

        private void getInpatientAdmissionDetails(InpatientChart searchChart, ref InpatientChart chart)
        {
            try
            {

                Admission hospInptAdmit = null;

                if (searchChart.admissionId > 0)
                {

                    hospInptAdmit = (
                                                from hospinptadm in _icmsContext.HospitalInpatientAdmissions

                                                join reasonVisit in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                                                on hospinptadm.hospital_inpatient_admission_id equals (int)reasonVisit.hospital_inpatient_admission_id into rsnVisits
                                                from inptReasonForVisit in rsnVisits.DefaultIfEmpty()

                                                join hospInptAdmSrc in _icmsContext.HospitalInpatientAdmissionSources
                                                on hospinptadm.hospital_inpatient_admission_id equals hospInptAdmSrc.hospital_inpatient_admission_id into inptAdmSrc
                                                from inptAdmAdmitSrc in inptAdmSrc.DefaultIfEmpty()

                                                join admitSrc in _icmsContext.HospitalAdmissionSources
                                                on inptAdmAdmitSrc.hosptial_admission_source_id equals admitSrc.hosptial_admission_source_id into admitSrcs
                                                from hospAdmitSrc in admitSrcs.DefaultIfEmpty()

                                                where hospinptadm.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                                                select new Admission
                                                {
                                                    hospital_inpatient_admission_id = hospinptadm.hospital_inpatient_admission_id,
                                                    admitDate = (hospinptadm.registered_date.HasValue) ? hospinptadm.registered_date : DateTime.MinValue,
                                                    reasonForVisit = (inptReasonForVisit != null) ? inptReasonForVisit.reason_for_visit : "",
                                                    admissionAdmitSourceId = (inptAdmAdmitSrc != null) ? inptAdmAdmitSrc.hosptial_admission_source_id : 0,
                                                    admissionAdmitSource = (hospAdmitSrc != null) ? hospAdmitSrc.admission_source_name : ""
                                                }
                                              )
                                              .FirstOrDefault();

                    if (hospInptAdmit != null)
                    {

                        foreach (InpatientChartSource source in chart.sources)
                        {
                            switch (source.modelVariableName)
                            {
                                case "displayAdmitDate":
                                    source.sourceTextValue = (hospInptAdmit.admitDate.Value != null && !hospInptAdmit.admitDate.Value.Equals(DateTime.MinValue)) ?
                                                                    hospInptAdmit.admitDate.Value.ToShortDateString() : "";
                                    break;

                                case "admissionSourceId":
                                    source.sourceIntValue = (hospInptAdmit.admissionAdmitSourceId > 0) ? hospInptAdmit.admissionAdmitSourceId : 0;
                                    break;

                                case "reasonForVisit":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(hospInptAdmit.reasonForVisit)) ? hospInptAdmit.reasonForVisit : "";
                                    break;

                            }
                        }

                    }

                }
            }
            catch(Exception ex)
            {

            }

        }

        private PatientContacts getAdmissionContact(InpatientChart searchChart)
        {
            PatientContacts contact = null;

            contact = (
                        from admitcontacts in _icmsContext.rMemberAdmissionContactses
                        where admitcontacts.hospital_inpatient_admission_id.Equals(searchChart.admissionId)
                        select new PatientContacts
                        {
                            contactsId = admitcontacts.member_admission_contacts_id,
                            firstName = admitcontacts.first_name,
                            lastName = admitcontacts.last_name,
                            phoneNumber = admitcontacts.phone1,
                        }
                      )
                      .FirstOrDefault();

            return contact;
        }

        private string getSelectControlDataLoaderValue(InpatientChartSource dailySource)
        {

            if (dailySource != null)
            {
                List<AssessItem> loaderForSource = getSourceItemLoader(dailySource);

                foreach(AssessItem dataItem in loaderForSource)
                {
                    if (dailySource.sourceIntValue.Equals(dataItem.sourceLoaderId))
                    {
                        return dataItem.sourceLoaderDescription;

                    }
                }
            }
                
            return "";
        }

        public List<InpatientChartSource> getInpatientChartSources(InpatientChart searchChart)
        {
            List<InpatientChartSource> returnSources = null;

            returnSources = (
                                from inptchrtsrc in _icmsContext.HospitalInpatientAdmissionChartSources
                                where inptchrtsrc.hospital_inpatient_admission_chart_id.Equals(searchChart.chartId)
                                orderby inptchrtsrc.display_order
                                select new InpatientChartSource
                                {
                                    controlClass = inptchrtsrc.source_control_class,
                                    controlType = inptchrtsrc.source_control_type,
                                    loaderDescriptionColumnName = inptchrtsrc.source_control_loader_description_column,
                                    loaderIdColumnName = inptchrtsrc.source_control_loader_id_column,
                                    loaderTableName = inptchrtsrc.source_control_loader_table,
                                    modelVariableName = inptchrtsrc.source_model_variable,
                                    modelDataType = inptchrtsrc.source_model_type,
                                    sourceId = inptchrtsrc.hospital_inpatient_admission_chart_source_id,
                                    sourceName = inptchrtsrc.source_name,
                                    sourceNameAbbrev = inptchrtsrc.source_name_abbrev,
                                    sourceOrder = inptchrtsrc.display_order,
                                    controlPattern = inptchrtsrc.pattern,
                                    maxLength = inptchrtsrc.max_length,
                                    placeholder = inptchrtsrc.placeholder,
                                    textareaCols = inptchrtsrc.textarea_cols,
                                    textareaRows = inptchrtsrc.textarea_rows,
                                    controlGroupName = inptchrtsrc.source_control_groupname,
                                    controlMin = inptchrtsrc.source_control_min,
                                    controlMax = inptchrtsrc.source_control_max,
                                    controlStep = inptchrtsrc.source_control_step,
                                    controlValue = inptchrtsrc.source_control_value,
                                    controlLabel = inptchrtsrc.source_control_label,
                                    controlChecked = inptchrtsrc.source_control_checked
                                }
                            )
                            .ToList();

            return returnSources;
        }

        public InpatientChartSource getAdmissionChartSourceItem(InpatientChart chart)
        {
            InpatientChartSource returnSourceItem = null;

            if (chart.sourceId > 0)
            {

                returnSourceItem = (
                                        from source in _icmsContext.HospitalInpatientAdmissionChartSources

                                        join chrt in _icmsContext.HospitalInpatientAdmissionCharts
                                        on source.hospital_inpatient_admission_chart_id equals chrt.hospital_inpatient_admission_chart_id into admitchrt
                                        from charts in admitchrt.DefaultIfEmpty()

                                        where source.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                        select new InpatientChartSource
                                        {
                                            sourceId = source.hospital_inpatient_admission_chart_source_id,
                                            chartId = source.hospital_inpatient_admission_chart_id,
                                            chartName = charts.chart_name,
                                            chartTableName = charts.chart_table_name,
                                            sourceName = source.source_name,
                                            sourceNameAbbrev = source.source_name_abbrev,
                                            sourceOrder = source.display_order,
                                            controlType = source.source_control_type,
                                            controlClass = source.source_control_class,
                                            modelVariableName = source.source_model_variable,
                                            modelDataType = source.source_model_type,
                                            loaderTableName = source.source_control_loader_table,
                                            loaderIdColumnName = source.source_control_loader_id_column,
                                            loaderDescriptionColumnName = source.source_control_loader_description_column,
                                            controlPattern = source.pattern,
                                            maxLength = source.max_length,
                                            placeholder = source.placeholder,
                                            textareaCols = source.textarea_cols,
                                            textareaRows = source.textarea_rows,
                                            controlGroupName = source.source_control_groupname,
                                            controlMin = source.source_control_min,
                                            controlMax = source.source_control_max,
                                            controlStep = source.source_control_step,
                                            controlValue = source.source_control_value,
                                            controlLabel = source.source_control_label,
                                            controlChecked = source.source_control_checked
                                        }
                                   )
                                   .FirstOrDefault();

                if (returnSourceItem != null)
                {

                    //load select control dropdown options
                    if (!string.IsNullOrEmpty(returnSourceItem.loaderTableName) &&
                        !string.IsNullOrEmpty(returnSourceItem.loaderIdColumnName) &&
                        !string.IsNullOrEmpty(returnSourceItem.loaderDescriptionColumnName))
                    {
                        returnSourceItem.loaderForSource = getSourceItemLoader(returnSourceItem);
                    }

                    if (returnSourceItem.controlType.Equals("radio"))
                    {
                        returnSourceItem.sourceRadios = getSourceItemAllRadios(returnSourceItem);
                    }


                    getTableSourceControlValues(chart, ref returnSourceItem);

                }

            }
            return returnSourceItem;
        }

        public List<InpatientChartSource> getAdmissionChartSources(InpatientChart chart)
        {
            List<InpatientChartSource> returnSources = null;

            if (chart.chartId > 0)
            {

                returnSources = (
                                        from source in _icmsContext.HospitalInpatientAdmissionChartSources

                                        join chrt in _icmsContext.HospitalInpatientAdmissionCharts
                                        on source.hospital_inpatient_admission_chart_id equals chrt.hospital_inpatient_admission_chart_id into chrts
                                        from charts in chrts.DefaultIfEmpty()

                                        where source.hospital_inpatient_admission_chart_id.Equals(chart.chartId)
                                        select new InpatientChartSource
                                        {
                                            sourceId = source.hospital_inpatient_admission_chart_source_id,
                                            chartId = source.hospital_inpatient_admission_chart_id,
                                            chartName = charts.chart_name,
                                            chartTableName = charts.chart_table_name,
                                            sourceName = source.source_name,
                                            sourceNameAbbrev = source.source_name_abbrev,
                                            sourceOrder = source.display_order,
                                            controlType = source.source_control_type,
                                            controlClass = source.source_control_class,
                                            modelVariableName = source.source_model_variable,
                                            modelDataType = source.source_model_type,
                                            loaderTableName = source.source_control_loader_table,
                                            loaderIdColumnName = source.source_control_loader_id_column,
                                            loaderDescriptionColumnName = source.source_control_loader_description_column,
                                            controlPattern = source.pattern,
                                            maxLength = source.max_length,
                                            placeholder = source.placeholder,
                                            textareaCols = source.textarea_cols,
                                            textareaRows = source.textarea_rows,
                                            controlGroupName = source.source_control_groupname,
                                            controlMin = source.source_control_min,
                                            controlMax = source.source_control_max,
                                            controlStep = source.source_control_step,
                                            controlValue = source.source_control_value,
                                            controlLabel = source.source_control_label,
                                            controlChecked = source.source_control_checked
                                        }
                                   )
                                   .ToList();

                if (returnSources != null && returnSources.Count > 0)                    
                {

                    string groupName = null;

                    foreach(InpatientChartSource source in returnSources)
                    {
                        //loader source for select controls
                        if (!string.IsNullOrEmpty(source.loaderTableName) &&
                        !string.IsNullOrEmpty(source.loaderIdColumnName) &&
                        !string.IsNullOrEmpty(source.loaderDescriptionColumnName))
                        {

                            source.loaderForSource = getSourceItemLoader(source);
                        }

                        
                        if (source.controlType.Equals("radio"))
                        {
                            if (source.controlGroupName != groupName)
                            {
                                //radio button setup
                                source.sourceRadios = getSourceItemAllRadios(source);
                                groupName = source.controlGroupName;
                            }                                                       
                        }                        
                    }

                    loadSourceControlValue(chart, ref returnSources);
                }

            }
            return returnSources;
        }

        public InpatientChartSource getAdmissionChartSourceHighlightNote(InpatientChart source)
        {
            InpatientChartSource highlightNote = null;

            if (source.chartTableId > 0 
                && source.sourceId > 0
                && source.admissionId > 0)
            {

                string chartTableName = getSourceTableName(source.sourceId);

                if (!string.IsNullOrEmpty(chartTableName))
                {
                    getSourceHighlight(source, ref highlightNote, chartTableName);
                    getSourceMdtNote(source, ref highlightNote, chartTableName);
                }
            }

            return highlightNote;
        }

        private string getSourceTableName(int sourceId)
        {
            string tableName = "";

            InpatientChartSource dbSource = (

                from srces in _icmsContext.HospitalInpatientAdmissionChartSources

                join chrts in _icmsContext.HospitalInpatientAdmissionCharts
                on srces.hospital_inpatient_admission_chart_id equals chrts.hospital_inpatient_admission_chart_id

                where srces.hospital_inpatient_admission_chart_source_id.Equals(sourceId)
                select new InpatientChartSource
                {
                    chartTableName = chrts.chart_table_name,
                }
            )
            .FirstOrDefault();

            if (dbSource != null)
            {
                tableName = dbSource.chartTableName;
            }


            return tableName;
        }

        private void getSourceHighlight(InpatientChart source, ref InpatientChartSource highlightNote, string chartTableName)
        {
            switch (chartTableName)
            {
                case "MEMBER_VITALS":
                    highlightNote = getVitalSignsHighlight(source);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                    highlightNote = getCardiacHighlight(source);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                    highlightNote = getNeurologicalHighlight(source);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                    highlightNote = getBloodGasHighlight(source);
                    break;
            }
        }

        private InpatientChartSource getVitalSignsHighlight(InpatientChart source)
        {            

            HospitalInpatientAdmissionMdtNoteVitalSignReference highlightReference = (
                        from hglghtRef in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                        where hglghtRef.member_vitals_id.Equals(source.chartTableId)
                        && hglghtRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        select hglghtRef
                    )
                    .FirstOrDefault();

            if (highlightReference != null)
            {

                InpatientChartSource highlight = new InpatientChartSource();
                highlight.highlightSource = false;


                if (highlightReference.highlight.HasValue)
                {
                    highlight.highlightSource = highlightReference.highlight;
                }

                if (!string.IsNullOrEmpty(highlightReference.highlight_color))
                {
                    highlight.highlightColor = highlightReference.highlight_color;
                }

                return highlight;
            }


            return null;
        }

        private InpatientChartSource getCardiacHighlight(InpatientChart source)
        {

            HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference dbHighlight = (
                        from hglghtRef in _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences
                        where hglghtRef.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(source.chartTableId)
                        && hglghtRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        select hglghtRef
                    )
                    .FirstOrDefault();

            if (dbHighlight != null)
            {

                InpatientChartSource highlight = new InpatientChartSource();
                highlight.highlightSource = false;


                if (dbHighlight.highlight.HasValue)
                {
                    highlight.highlightSource = dbHighlight.highlight;
                }

                if (!string.IsNullOrEmpty(dbHighlight.highlight_color))
                {
                    highlight.highlightColor = dbHighlight.highlight_color;
                }

                return highlight;
            }


            return null;
        }

        private InpatientChartSource getNeurologicalHighlight(InpatientChart source)
        {
 
            HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference dbHighlight = (
                        from hglghtRef in _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences
                        where hglghtRef.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(source.chartTableId)
                        && hglghtRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        select hglghtRef
                    )
                    .FirstOrDefault();

            if (dbHighlight != null)
            {

                InpatientChartSource highlight = new InpatientChartSource();
                highlight.highlightSource = false;


                if (dbHighlight.highlight.HasValue)
                {
                    highlight.highlightSource = dbHighlight.highlight;
                }

                if (!string.IsNullOrEmpty(dbHighlight.highlight_color))
                {
                    highlight.highlightColor = dbHighlight.highlight_color;
                }

                return highlight;
            }


            return null;
        }

        private InpatientChartSource getBloodGasHighlight(InpatientChart source)
        {

            HospitalInpatientAdmissionMdtNoteBloodGasReference dbHighlight = (
                        from hglghtRef in _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences
                        where hglghtRef.hosptial_inpatient_admission_blood_gas_id.Equals(source.chartTableId)
                        && hglghtRef.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                        select hglghtRef
                    )
                    .FirstOrDefault();

            if (dbHighlight != null)
            {

                InpatientChartSource highlight = new InpatientChartSource();
                highlight.highlightSource = false;


                if (dbHighlight.highlight.HasValue)
                {
                    highlight.highlightSource = dbHighlight.highlight;
                }

                if (!string.IsNullOrEmpty(dbHighlight.highlight_color))
                {
                    highlight.highlightColor = dbHighlight.highlight_color;
                }

                return highlight;
            }


            return null;
        }
        

        private void getSourceMdtNote(InpatientChart source, ref InpatientChartSource highlightNote, string chartTableName)
        {            
         
            int noteId = getSourceMdtNoteId(source, chartTableName);

            if (noteId > 0)
            {
                Note note = getSourceMdtNoteUsingNoteId(noteId);

                if (note != null)
                {
                    
                    if (highlightNote == null)
                    {
                        highlightNote = new InpatientChartSource();
                    }

                    highlightNote.mdtNote = note;
                }
            }            
        }

        private Note getSourceMdtNoteUsingNoteId(int noteId)
        {
            Note note = null;

            note = (
                from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                join hospnotetyp in _icmsContext.HospitalNoteTypes
                on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                where hospInptAdmMdt.hosptial_inpatient_admission_mdt_note.Equals(noteId)
                orderby hospInptAdmMdt.creation_date descending
                select new Note
                {
                    noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                    recordDate = hospInptAdmMdt.creation_date,
                    displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                        hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                        : "N/A",
                    caseOwnerId = hospInptAdmMdt.creation_user_id,
                    noteText = hospInptAdmMdt.hospital_note,
                    hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                    hospitalNoteTypeName = hospnotetyp.note_type_name
                }
            )
            .FirstOrDefault();
                            

            return note;
        }

        private int getSourceMdtNoteId(InpatientChart source, string chartTableName)
        {
            int noteId = 0;

            switch (chartTableName)
            {
                case "MEMBER_VITALS":
                    HospitalInpatientAdmissionMdtNoteVitalSignReference vitalRef = getVitalSignsMdtNoteReference(source);

                    if (vitalRef != null)
                    {
                        noteId = vitalRef.hosptial_inpatient_admission_mdt_note;
                    }

                    break;

                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                    HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference cardiacRef = getCardiacMdtNoteReference(source);

                    if (cardiacRef != null)
                    {
                        noteId = cardiacRef.hosptial_inpatient_admission_mdt_note;
                    }

                    break;

                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                    HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference neuroRef = getNeurologicalMdtNoteReference(source);

                    if (neuroRef != null)
                    {
                        noteId = neuroRef.hosptial_inpatient_admission_mdt_note;
                    }

                    break;

                case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                    HospitalInpatientAdmissionMdtNoteBloodGasReference bloodgasRef = getBloodGasMdtNoteReference(source);

                    if (bloodgasRef != null)
                    {
                        noteId = bloodgasRef.hosptial_inpatient_admission_mdt_note;
                    }
                    break;
            }

            return noteId;
        }

        private HospitalInpatientAdmissionMdtNoteVitalSignReference getVitalSignsMdtNoteReference(InpatientChart source)
        {
            HospitalInpatientAdmissionMdtNoteVitalSignReference reference = (
                    from refs in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                    where refs.member_vitals_id.Equals(source.chartTableId)
                    && refs.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                    && refs.hospital_inpatient_admission_id.Equals(source.admissionId)
                    select refs
                )
                .FirstOrDefault();                

            return reference;
        }

        private HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference getCardiacMdtNoteReference(InpatientChart source)
        {
            HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference reference = (
                    from refs in _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences
                    where refs.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(source.chartTableId)
                    && refs.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                    && refs.hospital_inpatient_admission_id.Equals(source.admissionId)
                    select refs
                )
                .FirstOrDefault();

            return reference;
        }

        private HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference getNeurologicalMdtNoteReference(InpatientChart source)
        {
            HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference reference = (
                    from refs in _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences
                    where refs.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(source.chartTableId)
                    && refs.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                    && refs.hospital_inpatient_admission_id.Equals(source.admissionId)
                    select refs
                )
                .FirstOrDefault();

            return reference;
        }

        private HospitalInpatientAdmissionMdtNoteBloodGasReference getBloodGasMdtNoteReference(InpatientChart source)
        {
            HospitalInpatientAdmissionMdtNoteBloodGasReference reference = (
                    from refs in _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences
                    where refs.hosptial_inpatient_admission_blood_gas_id.Equals(source.chartTableId)
                    && refs.hospital_inpatient_admission_chart_source_id.Equals(source.sourceId)
                    && refs.hospital_inpatient_admission_id.Equals(source.admissionId)
                    select refs
                )
                .FirstOrDefault();

            return reference;
        }
        

        private void loadSourceControlValue(InpatientChart chart, ref List<InpatientChartSource> sources)
        {
            string tableName = getSourceControlTableName(sources);
            string strDateMeasured = getSourceControlStringDateMeasured(chart);
            DateTime dteDateMeasured = getSourceControlDateMeasured(chart);
            int hourMeasured = getSourceControlHourMeasured(chart);
            int admissionId = getSourceControlAdmissionId(chart);

            getTableSourceControlValuesForLoad(tableName, dteDateMeasured, hourMeasured, chart.timeInterval, admissionId, ref sources);
           
        }

        private string getSourceControlTableName(List<InpatientChartSource> sources)
        {
            string tableName = "";

            foreach (InpatientChartSource source in sources)
            {
                if (!string.IsNullOrEmpty(source.chartTableName))
                {
                    tableName = source.chartTableName;
                    break;
                }

            }

            return tableName;
        }

        private DateTime getSourceControlDateMeasured(InpatientChart chart)
        {
            if (chart.dateMeasured != null && !chart.dateMeasured.Equals(DateTime.MinValue))
            {
                return chart.dateMeasured;
            }
            

            return DateTime.MinValue;
        }

        private string getSourceControlStringDateMeasured(InpatientChart chart)
        {
            if (!string.IsNullOrEmpty(chart.date))
            {
                return chart.date;
            }

            return null;
        }

        private int getSourceControlHourMeasured(InpatientChart chart)
        {
            if (chart.hour > 0)
            {
                return chart.hour;
            }

            return 0;
        }

        private int getSourceControlAdmissionId(InpatientChart chart)
        {
            if (chart.admissionId > 0)
            {
                return (int)chart.admissionId;
            }

            return 0;
        }

        private void getTableSourceControlValues(InpatientChart chart, ref InpatientChartSource sourceItem)
        {
            if (!string.IsNullOrEmpty(sourceItem.chartTableName))
            {
                switch (sourceItem.chartTableName)
                {
                    case "MEMBER_VITALS":
                        getVitalSignsItemSourceControlValue(chart, ref sourceItem);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                        getCardiacItemSourceControlValue(chart, ref sourceItem);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                        getNeurologicalItemSourceControlValue(chart, ref sourceItem);
                        break;
                    case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                        getBloodGasItemSourceControlValue(chart, ref sourceItem);
                        break;
                }

            }
            else if (!string.IsNullOrEmpty(sourceItem.chartType))
            {

                switch (sourceItem.chartType)
                {
                    case "demographicChart":
                        break;
                    case "admissionChart":
                        break;
                }

            }
        }

        private void getTableSourceControlValuesForLoad(string chartTableName, DateTime dateMeasured, int hourMeasured,
                                                        int timeInterval, int admissionId, ref List<InpatientChartSource> chartSources)
        {
            switch (chartTableName)
            {
                case "MEMBER_VITALS":
                    getVitalSignSourceControlValues(dateMeasured, hourMeasured, timeInterval, admissionId,  ref chartSources);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                    getCardiacAssessmentSourceControlValues(dateMeasured, hourMeasured, admissionId, ref chartSources);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                    getNeurologicalAssessmentSourceControlValues(dateMeasured, hourMeasured, admissionId, ref chartSources);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                    getBloodGasAssessmentSourceControlValues(dateMeasured, hourMeasured, admissionId, ref chartSources);
                    break;
            }
        }

        private void getVitalSignSourceControlValues(DateTime dateMeasured, int hourMeasured,
                                                     int timeInterval, int admissionId, ref List<InpatientChartSource> sources)
        {
            try
            {
                if (admissionId > 0 &&
                    !dateMeasured.Equals(DateTime.MinValue))
                {


                    MemberVitals dbVital = (
                                            from vitals in _icmsContext.MemberVitalses
                                            where vitals.hospital_inpatient_admission_id.Equals(admissionId)
                                            && vitals.date_measured.Equals(dateMeasured)
                                            select vitals
                                            )
                                            .FirstOrDefault();

                    if (dbVital != null)
                    {

                        foreach (InpatientChartSource source in sources)
                        {

                            source.chartTableId = dbVital.member_vitals_id;

                            source.mdtNote = getVitalSignSourceMtdNote(source, dbVital);

                            InpatientChartSource vitalSource = new InpatientChartSource();
                            getVitalSignSourceHighlightAndColor(ref vitalSource, dbVital);

                            source.highlightSource = vitalSource.highlightSource;
                            source.highlightColor = vitalSource.highlightColor;

                            switch (source.modelVariableName)
                            {
                                case "temperature":
                                    source.sourceDecimalValue = (dbVital.temperature_in_fahrenheit != null) ? (decimal)dbVital.temperature_in_fahrenheit : 0;
                                    break;
                                case "temperatureSiteId":
                                    source.sourceIntValue =  (dbVital.hospital_temperature_site_id != null) ? (int)dbVital.hospital_temperature_site_id : 0;
                                    break;
                                case "temperatureManagement":
                                    source.sourceTextValue = dbVital.temperature_management;
                                    break;
                                case "heartRate":
                                    source.sourceTextValue = dbVital.heart_rate;
                                    break;
                                case "heartRhythm":
                                    source.sourceIntValue = (dbVital.hospital_pulse_rhythm_id != null) ? (int)dbVital.hospital_pulse_rhythm_id : 0;
                                    break;
                                case "systolicBloodPressure":
                                    source.sourceIntValue = (dbVital.seated_blood_pressure_systolic != null) ? (int)dbVital.seated_blood_pressure_systolic : 0;
                                    break;
                                case "diastolicBloodPressure":
                                    source.sourceIntValue = (dbVital.seated_blood_pressure_diastolic != null) ? (int)dbVital.seated_blood_pressure_diastolic : 0;
                                    break;
                                case "meanArterialPressure":
                                    source.sourceIntValue = (dbVital.mean_arterial_pressure != null) ? (int)dbVital.mean_arterial_pressure : 0;
                                    break;
                                case "nonInvasiveBloodPressure":
                                    source.sourceTextValue = dbVital.non_invasive_blood_pressure;
                                    break;
                                case "respirationRate":
                                    source.sourceIntValue = (dbVital.respiration_per_minute != null) ? (int)dbVital.respiration_per_minute : 0;
                                    break;
                                case "fi02":
                                    source.sourceIntValue = (dbVital.fi02 != null) ? (int)dbVital.fi02 : 0;
                                    break;
                                case "sp02":
                                    source.sourceTextValue = dbVital.sp02;
                                    break;
                                case "etco02":
                                    source.sourceTextValue = dbVital.etc02;
                                    break;
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {

            }
        }

        private void getVitalSignsItemSourceControlValue(InpatientChart chart, ref InpatientChartSource sourceItem)
        {

            MemberVitals dbVital = (
                                        from memvital in _icmsContext.MemberVitalses
                                        where memvital.member_vitals_id.Equals(chart.chartTableId)
                                        select memvital
                                    )
                                    .FirstOrDefault();

            if (dbVital != null)
            {

                sourceItem.chartTableId = chart.chartTableId;
                sourceItem.hour = chart.hour;

                sourceItem.mdtNote = getVitalSignSourceMtdNote(sourceItem, dbVital);
                getVitalSignSourceHighlightAndColor(ref sourceItem, dbVital);

                switch (sourceItem.modelVariableName)
                {
                    case "temperature":
                        sourceItem.sourceDecimalValue = (dbVital.temperature_in_fahrenheit != null) ? (decimal)dbVital.temperature_in_fahrenheit : 0;
                        break;
                    case "temperatureSiteId":
                        sourceItem.sourceIntValue = (dbVital.hospital_temperature_site_id != null) ? (int)dbVital.hospital_temperature_site_id : 0;
                        break;
                    case "temperatureManagement":
                        sourceItem.sourceTextValue = dbVital.temperature_management;
                        break;
                    case "heartRate":
                        sourceItem.sourceTextValue = dbVital.heart_rate;
                        break;
                    case "heartRhythm":
                        sourceItem.sourceIntValue = (dbVital.hospital_pulse_rhythm_id != null) ? (int)dbVital.hospital_pulse_rhythm_id : 0;
                        break;
                    case "systolicBloodPressure":
                        sourceItem.sourceIntValue = (dbVital.seated_blood_pressure_systolic != null) ? (int)dbVital.seated_blood_pressure_systolic : 0;
                        break;
                    case "diastolicBloodPressure":
                        sourceItem.sourceIntValue = (dbVital.seated_blood_pressure_diastolic != null) ? (int)dbVital.seated_blood_pressure_diastolic : 0;
                        break;
                    case "meanArterialPressure":
                        sourceItem.sourceIntValue = (dbVital.mean_arterial_pressure != null) ? (int)dbVital.mean_arterial_pressure : 0;
                        break;
                    case "nonInvasiveBloodPressure":
                        sourceItem.sourceTextValue = dbVital.non_invasive_blood_pressure;
                        break;
                    case "respirationRate":
                        sourceItem.sourceIntValue = (dbVital.respiration_per_minute != null) ? (int)dbVital.respiration_per_minute : 0;
                        break;
                    case "fi02":
                        sourceItem.sourceIntValue = (dbVital.fi02 != null) ? (int)dbVital.fi02 : 0;
                        break;
                    case "sp02":
                        sourceItem.sourceTextValue = dbVital.sp02;
                        break;
                    case "etco02":
                        sourceItem.sourceTextValue = dbVital.etc02;
                        break;
                }
            }
        }

        private void getCardiacAssessmentSourceControlValues(DateTime dateMeasured, int hourMeasured,
                                                             int admissionId, ref List<InpatientChartSource> sources)
        {
            try
            {

                if (admissionId > 0 &&
                    !dateMeasured.Equals(DateTime.MinValue))
                {

                    HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac dbCardiacAssess = (
                            from cardiac in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs
                            where cardiac.hospital_inpatient_admission_id.Equals(admissionId)
                            && cardiac.date_measured.Equals(dateMeasured)
                            select cardiac
                        )
                        .FirstOrDefault();

                    if (dbCardiacAssess != null)
                    {

                        foreach (InpatientChartSource source in sources)
                        {

                            source.chartTableId = dbCardiacAssess.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id;

                            source.mdtNote = getCardiacSourceMtdNote(source, dbCardiacAssess);

                            InpatientChartSource cardiacSource = new InpatientChartSource();
                            getCardiacSourceHighlightAndColor(ref cardiacSource, dbCardiacAssess);

                            source.highlightSource = cardiacSource.highlightSource;
                            source.highlightColor = cardiacSource.highlightColor;


                            switch (source.modelVariableName)
                            {
                                case "cardiacAorticSoundNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.aortic_sound_normal != null) ? (bool)dbCardiacAssess.aortic_sound_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacApicalPulseSoundNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.apical_pulse_sound_normal != null) ? (bool)dbCardiacAssess.apical_pulse_sound_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacEdemaSymptoms":
                                    source.sourceBoolValue = (dbCardiacAssess.edema_symptoms != null) ? (bool)dbCardiacAssess.edema_symptoms : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacErbsPointSoundNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.erb_point_sound_normal != null) ? (bool)dbCardiacAssess.erb_point_sound_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacIndex":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(dbCardiacAssess.cardiac_index)) ? dbCardiacAssess.cardiac_index : "";
                                    break;
                                case "cardiacNailClubbing":
                                    source.sourceBoolValue = (dbCardiacAssess.nail_clubbing != null) ? (bool)dbCardiacAssess.nail_clubbing : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacOutput":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(dbCardiacAssess.cardiac_output)) ? dbCardiacAssess.cardiac_output : "";
                                    break;
                                case "cardiacPulmonicSoundNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.pulmonic_sound_normal != null) ? (bool)dbCardiacAssess.pulmonic_sound_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacPulsesNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.pulses_normal != null) ? (bool)dbCardiacAssess.pulses_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                                case "cardiacStrokeVolumeIndex":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(dbCardiacAssess.stroke_volume_index)) ? dbCardiacAssess.stroke_volume_index : "";
                                    break;
                                case "cardiacStrokeVolume":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(dbCardiacAssess.stroke_volume)) ? dbCardiacAssess.stroke_volume : "";
                                    break;
                                case "cardiacTricuspidSoundNormal":
                                    source.sourceBoolValue = (dbCardiacAssess.tricuspid_sound_normal != null) ? (bool)dbCardiacAssess.tricuspid_sound_normal : false;
                                    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                    break;
                               
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void getCardiacItemSourceControlValue(InpatientChart chart, ref InpatientChartSource sourceItem)
        {

            HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment = (
                                        from assess in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs
                                        where assess.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(chart.chartTableId)
                                        select assess
                                    )
                                    .FirstOrDefault();

            if (assessment != null)
            {

                sourceItem.chartTableId = chart.chartTableId;
                sourceItem.hour = chart.hour;

                sourceItem.mdtNote = getCardiacSourceMtdNote(sourceItem, assessment);
                getCardiacSourceHighlightAndColor(ref sourceItem, assessment);

                switch (sourceItem.modelVariableName)
                {
                    case "cardiacAorticSoundNormal":
                        sourceItem.sourceBoolValue = (assessment.aortic_sound_normal != null) ? (bool)assessment.aortic_sound_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacApicalPulseSoundNormal":
                        sourceItem.sourceBoolValue = (assessment.apical_pulse_sound_normal != null) ? (bool)assessment.apical_pulse_sound_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacEdemaSymptoms":
                        sourceItem.sourceBoolValue = (assessment.edema_symptoms != null) ? (bool)assessment.edema_symptoms : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacErbsPointSoundNormal":
                        sourceItem.sourceBoolValue = (assessment.erb_point_sound_normal != null) ? (bool)assessment.erb_point_sound_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacIndex":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.cardiac_index)) ? assessment.cardiac_index : "";
                        break;
                    case "cardiacNailClubbing":
                        sourceItem.sourceBoolValue = (assessment.nail_clubbing != null) ? (bool)assessment.nail_clubbing : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacOutput":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.cardiac_output)) ? assessment.cardiac_output : "";
                        break;
                    case "cardiacPulmonicSoundNormal":
                        sourceItem.sourceBoolValue = (assessment.pulmonic_sound_normal != null) ? (bool)assessment.pulmonic_sound_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacPulsesNormal":
                        sourceItem.sourceBoolValue = (assessment.pulses_normal != null) ? (bool)assessment.pulses_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;
                    case "cardiacStrokeVolumeIndex":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.stroke_volume_index)) ? assessment.stroke_volume_index : "";
                        break;
                    case "cardiacStrokeVolume":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.stroke_volume)) ? assessment.stroke_volume : "";
                        break;
                    case "cardiacTricuspidSoundNormal":
                        sourceItem.sourceBoolValue = (assessment.tricuspid_sound_normal != null) ? (bool)assessment.tricuspid_sound_normal : false;
                        sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                        break;                    
                }
            }
        }

        private void getNeurologicalAssessmentSourceControlValues(DateTime dateMeasured, int hourMeasured,
                                                                  int admissionId, ref List<InpatientChartSource> sources)
        {
            try
            {

                if (admissionId > 0 &&
                    !dateMeasured.Equals(DateTime.MinValue))
                {

                    HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment = (
                                from assess in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals
                                where assess.hospital_inpatient_admission_id.Equals(admissionId)
                                && assess.date_measured.Equals(dateMeasured)
                                select assess
                            )
                            .FirstOrDefault();

                    if (assessment != null)
                    {

                        foreach (InpatientChartSource source in sources)
                        {

                            source.chartTableId = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id;

                            source.mdtNote = getNeuroSourceMtdNote(source, assessment);

                            InpatientChartSource neuroSource = new InpatientChartSource();
                            getNeurologicalSourceHighlightAndColor(ref neuroSource, assessment);

                            source.highlightSource = neuroSource.highlightSource;
                            source.highlightColor = neuroSource.highlightColor;


                            switch (source.modelVariableName)
                            {
                                case "neurologyHandSqueezeStrengthId":
                                    source.sourceIntValue = (assessment.hand_squeeze_strength_id != null) ? (int)assessment.hand_squeeze_strength_id : 0;
                                    break;


                                //case "cardiacApicalPulseSoundNormal":
                                //    source.sourceBoolValue = (assessment.apical_pulse_sound_normal != null) ? (bool)assessment.apical_pulse_sound_normal : false;
                                //    source.controlLabel = (source.sourceBoolValue) ? "Yes" : "No";
                                //    break;
                                
                                //case "cardiacOutput":
                                //    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.cardiac_output)) ? assessment.cardiac_output : "";
                                //    break;
                                

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void getNeurologicalItemSourceControlValue(InpatientChart chart, ref InpatientChartSource sourceItem)
        {

            HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment = (
                                        from assess in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals
                                        where assess.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(chart.chartTableId)
                                        select assess
                                    )
                                    .FirstOrDefault();

            if (assessment != null)
            {

                sourceItem.chartTableId = chart.chartTableId;
                sourceItem.hour = chart.hour;

                sourceItem.mdtNote = getNeuroSourceMtdNote(sourceItem, assessment);
                getNeurologicalSourceHighlightAndColor(ref sourceItem, assessment);

                switch (sourceItem.modelVariableName)
                {
                    case "neurologyHandSqueezeStrengthId":
                        sourceItem.sourceIntValue = (assessment.hand_squeeze_strength_id != null) ? (int)assessment.hand_squeeze_strength_id : 0;
                        break;

                    //case "cardiacApicalPulseSoundNormal":
                    //    sourceItem.sourceBoolValue = (assessment.apical_pulse_sound_normal != null) ? (bool)assessment.apical_pulse_sound_normal : false;
                    //    sourceItem.controlLabel = (sourceItem.sourceBoolValue) ? "Yes" : "No";
                    //    break;
                    
                }
            }
        }

        private void getBloodGasAssessmentSourceControlValues(DateTime dateMeasured, int hourMeasured,
                                                              int admissionId, ref List<InpatientChartSource> sources)
        {
            try
            {

                if (admissionId > 0 &&
                    !dateMeasured.Equals(DateTime.MinValue))
                {

                    HospitalInpatientAdmissionBloodGas assessment = (
                                from assess in _icmsContext.HospitalInpatientAdmissionBloodGases
                                where assess.hospital_inpatient_admission_id.Equals(admissionId)
                                && assess.date_measured.Equals(dateMeasured)
                                select assess
                            )
                            .FirstOrDefault();

                    if (assessment != null)
                    {

                        foreach (InpatientChartSource source in sources)
                        {

                            source.chartTableId = assessment.hosptial_inpatient_admission_blood_gas_id;

                            source.mdtNote = getBloodGasSourceMtdNote(source, assessment);

                            InpatientChartSource bloodGasSource = new InpatientChartSource();
                            getBloodGasSourceHighlightAndColor(ref bloodGasSource, assessment);

                            source.highlightSource = bloodGasSource.highlightSource;
                            source.highlightColor = bloodGasSource.highlightColor;


                            switch (source.modelVariableName)
                            {
                                case "bloodGasPh":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.ph)) ? assessment.ph : "";
                                    break;
                                case "bloodGasPc02":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.pco2)) ? assessment.pco2 : "";
                                    break;
                                case "bloodGasP02":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.po2)) ? assessment.po2 : "";
                                    break;
                                case "bloodGasHc03":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.hco3)) ? assessment.hco3 : "";
                                    break;
                                case "bloodGasBaseExcess":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.base_excess)) ? assessment.base_excess : "";
                                    break;
                                case "bloodGasHb":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.hb)) ? assessment.hb : "";
                                    break;
                                case "bloodGas02Saturation":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.o2_saturation)) ? assessment.o2_saturation : "";
                                    break;
                                case "bloodGasSodium":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.sodium)) ? assessment.sodium : "";
                                    break;
                                case "bloodGasPotassium":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.potassium)) ? assessment.potassium : "";
                                    break;
                                case "bloodGasCalcium":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.calcium)) ? assessment.calcium : "";
                                    break;
                                case "bloodGasBloodSugar":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.blood_sugar)) ? assessment.blood_sugar : "";
                                    break;
                                case "bloodGasLactate":
                                    source.sourceTextValue = (!string.IsNullOrEmpty(assessment.lactate)) ? assessment.lactate : "";
                                    break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        
        private void getBloodGasItemSourceControlValue(InpatientChart chart, ref InpatientChartSource sourceItem)
        {

            HospitalInpatientAdmissionBloodGas assessment = (
                                        from assess in _icmsContext.HospitalInpatientAdmissionBloodGases
                                        where assess.hosptial_inpatient_admission_blood_gas_id.Equals(chart.chartTableId)
                                        select assess
                                    )
                                    .FirstOrDefault();

            if (assessment != null)
            {

                sourceItem.chartTableId = chart.chartTableId;
                sourceItem.hour = chart.hour;

                sourceItem.mdtNote = getBloodGasSourceMtdNote(sourceItem, assessment);
                getBloodGasSourceHighlightAndColor(ref sourceItem, assessment);

                switch (sourceItem.modelVariableName)
                {
                    case "bloodGasPh":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.ph)) ? assessment.ph : "";
                        break;
                    case "bloodGasPc02":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.pco2)) ? assessment.pco2 : "";
                        break;
                    case "bloodGasP02":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.po2)) ? assessment.po2 : "";
                        break;
                    case "bloodGasHc03":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.hco3)) ? assessment.hco3 : "";
                        break;
                    case "bloodGasBaseExcess":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.base_excess)) ? assessment.base_excess : "";
                        break;
                    case "bloodGasHb":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.hb)) ? assessment.hb : "";
                        break;
                    case "bloodGas02Saturation":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.o2_saturation)) ? assessment.o2_saturation : "";
                        break;
                    case "bloodGasSodium":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.sodium)) ? assessment.sodium : "";
                        break;
                    case "bloodGasPotassium":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.potassium)) ? assessment.potassium : "";
                        break;
                    case "bloodGasCalcium":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.calcium)) ? assessment.calcium : "";
                        break;
                    case "bloodGasBloodSugar":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.blood_sugar)) ? assessment.blood_sugar : "";
                        break;
                    case "bloodGasLactate":
                        sourceItem.sourceTextValue = (!string.IsNullOrEmpty(assessment.lactate)) ? assessment.lactate : "";
                        break;
                }
            }
        }
        

        public string getSourceControlTime(int hour, string timeOfDay)
        {
            string returnTimeOfDay = " 12:00 AM";

            switch (hour)
            {
                case 1:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 1:00 AM";
                    } else
                    {
                        returnTimeOfDay = " 1:59 AM";
                    }
                    break;

                case 2:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 2:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 2:59 AM";
                    }
                    break;
                case 3:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 3:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 3:59 AM";
                    }
                    break;
                case 4:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 4:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 4:59 AM";
                    }
                    break;
                case 5:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 5:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 5:59 AM";
                    }
                    break;
                case 6:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 6:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 6:59 AM";
                    }
                    break;
                case 7:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 7:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 7:59 AM";
                    }
                    break;
                case 8:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 8:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 8:59 AM";
                    }
                    break;
                case 9:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 9:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 9:59 AM";
                    }
                    break;
                case 10:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 10:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 10:59 AM";
                    }
                    break;
                case 11:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 11:00 AM";
                    }
                    else
                    {
                        returnTimeOfDay = " 11:59 AM";
                    }
                    break;
                case 12:
                    if (timeOfDay.Equals("start"))
                    {
                        returnTimeOfDay = " 12:00 PM";
                    }
                    else
                    {
                        returnTimeOfDay = " :59 PM";
                    }
                    break;
                case 13:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 1:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 1:59 PM";
                    }
                    break;
                case 14:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 2:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 2:59 PM";
                    }
                    break;
                case 15:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 3:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 3:59 PM";
                    }
                    break;
                case 16:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 4:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 4:59 PM";
                    }
                    break;
                case 17:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 5:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 5:59 PM";
                    }
                    break;
                case 18:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 6:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 6:59 PM";
                    }
                    break;
                case 19:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 7:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 7:59 PM";
                    }
                    break;
                case 20:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 8:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 8:59 PM";
                    }
                    break;
                case 21:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 9:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 9:59 PM";
                    }
                    break;
                case 22:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 10:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 10:59 PM";
                    }
                    break;
                case 23:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 11:00 PM"; }
                    else
                    {
                        returnTimeOfDay = " 11:59 PM";
                    }
                    break;
                case 24:
                    if (timeOfDay.Equals("start"))
                    { returnTimeOfDay = " 12:00 AM"; }
                    else
                    {
                        returnTimeOfDay = " 12:59 AM";
                    }
                    break;
            }


            return returnTimeOfDay;
        }

        public List<AssessItem> getSourceItemLoader(InpatientChartSource source)
        {
            List<AssessItem> returnLoaderSource = null;

            StandardService standServ = new StandardService(_icmsContext, _emrContext);
            returnLoaderSource = standServ.getLoaderTableRows(source.loaderTableName, source.loaderIdColumnName, source.loaderDescriptionColumnName);

            return returnLoaderSource;
        }

        public List<InpatientChartSource> getSourceItemAllRadios(InpatientChartSource source)
        {
            
            List<InpatientChartSource> radios = null;

            List<InpatientChartSource> dbRadios = (
                                                    from ctrlSource in _icmsContext.HospitalInpatientAdmissionChartSources
                                                    where ctrlSource.hospital_inpatient_admission_chart_id.Equals(source.chartId)
                                                    && ctrlSource.source_control_groupname.Equals(source.controlGroupName)
                                                    && ctrlSource.source_control_type.Equals("radio")
                                                    select new InpatientChartSource
                                                    {
                                                        sourceId = ctrlSource.hospital_inpatient_admission_chart_source_id,
                                                        chartId = ctrlSource.hospital_inpatient_admission_chart_id,
                                                        sourceName = ctrlSource.source_name,
                                                        sourceNameAbbrev = ctrlSource.source_name_abbrev,
                                                        sourceOrder = ctrlSource.display_order,
                                                        controlType = ctrlSource.source_control_type,
                                                        controlClass = ctrlSource.source_control_class,
                                                        modelVariableName = ctrlSource.source_model_variable,
                                                        modelDataType = ctrlSource.source_model_type,
                                                        controlPattern = ctrlSource.pattern,
                                                        controlGroupName = ctrlSource.source_control_groupname,
                                                        controlChecked = ctrlSource.source_control_checked,
                                                        controlLabel = ctrlSource.source_control_label,
                                                        controlValue = ctrlSource.source_control_value
                                                    }
                                                  )
                                                  .ToList();

            if (dbRadios != null && dbRadios.Count > 0)
            {
                radios = dbRadios;
            }

            return radios;

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

        private List<VitalSign> getInpatientAdmissionVitalSigns(int hospitalInpatientAdmissionId)
        {
            List<VitalSign> vitals = null;

            try
            {
                List<MemberVitals> dbVitals = null;

                dbVitals = (
                                from memVits in _icmsContext.MemberVitalses
                                where memVits.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)
                                && memVits.deleted_flag.Equals(false)
                                orderby memVits.date_measured descending
                                select memVits
                            )
                            .ToList();

                if (dbVitals != null && dbVitals.Count > 0)
                {

                    vitals = new List<VitalSign>();

                    foreach (MemberVitals vit in dbVitals)
                    {
                        VitalSign addVital = assignAdmissionVitalSignReadings(vit); new VitalSign();

                        if (addVital != null)
                        {
                            vitals.Add(addVital);
                        }
                    }

                }

                return vitals;
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return vitals;
            }
        }

        private VitalSign assignAdmissionVitalSignReadings(MemberVitals vitalSign)
        {
            VitalSign returnVitalSign = null;

            if (vitalSign.member_vitals_id > 0)
            {
                returnVitalSign = new VitalSign();

                returnVitalSign.vitalSignId = vitalSign.member_vitals_id;
                returnVitalSign.dateTaken = vitalSign.date_measured;
                returnVitalSign.displayDateTaken = vitalSign.date_measured.ToShortDateString() + " " + vitalSign.date_measured.ToShortTimeString();

                //temperature
                if (vitalSign.temperature_in_celsius > 0)
                {
                    returnVitalSign.temperature = vitalSign.temperature_in_celsius;
                }
                else if (vitalSign.temperature_in_fahrenheit > 0)
                {
                    returnVitalSign.temperature = vitalSign.temperature_in_fahrenheit;
                    returnVitalSign.isFarenheit = true;
                }

                if (returnVitalSign.temperature > 0)
                {

                    returnVitalSign.alertLowTemperature = vitalSign.alert_low_temperature;
                    returnVitalSign.alertHighTemperature = vitalSign.alert_high_temperature;

                    if (vitalSign.hospital_temperature_site_id.Equals(3))
                    {
                        returnVitalSign.isRectal = true;
                    }

                }


                //pulse rate
                if (vitalSign.pulse_per_minute > 0)
                {

                    returnVitalSign.pulseRate = vitalSign.pulse_per_minute;

                    returnVitalSign.alertHighPulseRate = vitalSign.alert_high_pulse_rate;
                    returnVitalSign.alertLowPulseRate = vitalSign.alert_low_pulse_rate;

                }


                //respiration rate
                if (vitalSign.respiration_per_minute > 0)
                {

                    returnVitalSign.respirationRate = vitalSign.respiration_per_minute;
                    returnVitalSign.alertLowRespirationRate = vitalSign.alert_low_respiration_rate;
                    returnVitalSign.alertHighRespirationRate = vitalSign.alert_high_respiration_rate;

                }


                //blood pressure
                if (vitalSign.seated_blood_pressure_systolic > 00 && vitalSign.seated_blood_pressure_diastolic > 0)
                {

                    returnVitalSign.systolicBloodPressure = vitalSign.seated_blood_pressure_systolic;
                    returnVitalSign.diastolicBloodPressure = vitalSign.seated_blood_pressure_diastolic;

                    returnVitalSign.alertLowBloodPressure = vitalSign.alert_low_blood_pressure;
                    returnVitalSign.alertHighBloodPressure = vitalSign.alert_high_blood_pressure;

                }

            }

            return returnVitalSign;
        }

        public Admission getInpatientAdmissionUserDashboardDefaults(Admission admit)
        {
            Admission userDefaults = null;

            List<string> admitUserDefaults = getInpatientAdmissionUserDefaults(admit);

            if (admitUserDefaults != null)
            {
                userDefaults = new Admission();
                userDefaults.userInpatientAdmissionDashboardDefaults = admitUserDefaults;
            }

            return userDefaults;
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

        public decimal getPatientHeight(Guid memberId)
        {
            decimal height = 0;

            MemberVitals lastHeightVital = (
                                                from memvit in _icmsContext.MemberVitalses
                                                where memvit.member_id.Equals(memberId)
                                                && memvit.height_in_inches.HasValue
                                                orderby memvit.date_measured descending
                                                select memvit
                                           )
                                           .Take(1)
                                           .FirstOrDefault();

            if (lastHeightVital != null && lastHeightVital.height_in_inches > 0)
            {
                height = (decimal)lastHeightVital.height_in_inches;
            }

            return height;
        }

        public string convertHeightToDisplayHeight(decimal height)
        {
            string displayHeight = "";

            int feet = (int)height / 12;
            int inches = (int)height % 12;
            
            displayHeight = feet.ToString() + "' " + inches.ToString() + "''";

            return displayHeight;
        }

        public decimal getPatientWeight(Guid memberId)
        {
            decimal weight = 0;

            MemberVitals lastWeightVital = (
                                                from memvit in _icmsContext.MemberVitalses
                                                where memvit.member_id.Equals(memberId)
                                                && memvit.weight_in_pounds.HasValue
                                                orderby memvit.date_measured descending
                                                select memvit
                                           )
                                           .Take(1)
                                           .FirstOrDefault();

            if (lastWeightVital != null && lastWeightVital.weight_in_pounds > 0)
            {
                weight = (decimal)lastWeightVital.weight_in_pounds;
            }

            return weight;
        }

        public string convertWeightToDisplayWeight(decimal weight)
        {
            string displayWeight = "";

            displayWeight = weight.ToString() + " lbs";

            return displayWeight;
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


        public List<Note> getHospitalNoteTypes()
        {
            List<Note> noteTypes = null;

            noteTypes = (
                    from hospNteTyp in _icmsContext.HospitalNoteTypes
                    orderby hospNteTyp.note_type_name
                    select new Note
                    {
                        hospitalNoteTypeId = hospNteTyp.hospital_note_type_id,
                        hospitalNoteTypeName = hospNteTyp.note_type_name
                    }
                )
                .ToList();

            return noteTypes;
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

            try
            {

                if (order.hospital_inpatient_admission_id > 0)
                {
                    _icmsContext.HospitalInpatientAdmissionOrders.Add(order);

                    orderCreated = (_icmsContext.SaveChanges() > 0) ? true : false;
                }
            }
            catch(Exception ex)
            {

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



        public Admission getAdmissionMedications(string admitId)
        {
            Admission admitMeds = null;

            int hospInptAdmId = 0;

            if (!string.IsNullOrEmpty(admitId) && int.TryParse(admitId, out hospInptAdmId))
            {

                admitMeds = new Admission();

                string medAllergies = "";

                medAllergies = (
                                    from patallergy in _icmsContext.HospitalInpatientAdmissionAllergys
                                    where patallergy.hospital_inpatient_admission_id.Equals(hospInptAdmId)
                                    select patallergy.medication_allergy
                               )
                               .FirstOrDefault();

                if (!string.IsNullOrEmpty(medAllergies))
                {
                    admitMeds.allergies = new Allergy();
                    admitMeds.allergies.medicationAllergy = medAllergies;
                }

                List<HospitalInpatientAdmissionOrderMedication> inptMeds = null;

                inptMeds = (
                                from hospinptmeds in _icmsContext.HospitalInpatientAdmissionOrderMedications

                                join hospordrs in _icmsContext.HospitalInpatientAdmissionOrders
                                on hospinptmeds.hospital_inpatient_admission_order_id equals hospordrs.hospital_inpatient_admission_order_id

                                join hospinpt in _icmsContext.HospitalInpatientAdmissions
                                on hospordrs.hospital_inpatient_admission_id equals hospinpt.hospital_inpatient_admission_id

                                where hospinpt.hospital_inpatient_admission_id.Equals(hospInptAdmId)
                                && (hospinptmeds.deleted_flag.Equals(false) || !hospinptmeds.deleted_flag.HasValue)

                                orderby hospinptmeds.administered_date descending, hospinptmeds.creation_date descending

                                select hospinptmeds
                           )
                           .ToList();

                if (inptMeds != null)
                {
                    admitMeds.medication = new List<Medication>();

                    StandardService standServ = new StandardService(_icmsContext, _emrContext);

                    foreach(HospitalInpatientAdmissionOrderMedication med in inptMeds)
                    {
                        Medication addMed = new Medication();
                        addMed.admissionMedicationOrderId = med.hospital_inpatient_admission_order_medication_id;
                        addMed.createDate = med.creation_date;
                        addMed.displayCreateDate = (med.creation_date.HasValue) ?
                            med.creation_date.Value.ToShortDateString() + " " + med.creation_date.Value.ToShortTimeString() : "";
                        addMed.medicationName = med.medication_name;
                        addMed.sequenceNumber = med.gcn_seqno;
                        addMed.ndc = med.ndc;
                        addMed.dose = med.dose;
                        addMed.displayDateGive = (med.administered_date.HasValue) ? med.administered_date.Value.ToShortDateString() : "N/A";
                        addMed.timeGiven = (med.time_of_administration != null) ? med.time_of_administration : "N/A";

                        if (med.administered_user_id.HasValue)
                        {
                            IcmsUser adminByUser = standServ.getAspUser((Guid)med.administered_user_id);

                            addMed.administeredByName = (!string.IsNullOrEmpty(adminByUser.FullName)) ? adminByUser.FullName : "N/A";
                        } 
                        else
                        {
                            addMed.administeredByName = "N/A";
                        }

                        addMed.lastAdministeredData = getAdmissionMedicationLastAdministeredData(med);

                        admitMeds.medication.Add(addMed);
                    }
                }

            }

            return admitMeds;
        }

        public Admission getRemovedAdmissionMedications(string admitId)
        {
            Admission admitMeds = null;

            int hospInptAdmId = 0;

            if (!string.IsNullOrEmpty(admitId) && int.TryParse(admitId, out hospInptAdmId))
            {

                admitMeds = new Admission();

                List<HospitalInpatientAdmissionOrderMedication> inptMeds = null;

                inptMeds = (
                                from hospinptmeds in _icmsContext.HospitalInpatientAdmissionOrderMedications

                                join hospordrs in _icmsContext.HospitalInpatientAdmissionOrders
                                on hospinptmeds.hospital_inpatient_admission_order_id equals hospordrs.hospital_inpatient_admission_order_id

                                join hospinpt in _icmsContext.HospitalInpatientAdmissions
                                on hospordrs.hospital_inpatient_admission_id equals hospinpt.hospital_inpatient_admission_id

                                where hospinpt.hospital_inpatient_admission_id.Equals(hospInptAdmId)
                                && hospinptmeds.deleted_flag.Equals(true)

                                orderby hospinptmeds.administered_date descending, hospinptmeds.creation_date descending

                                select hospinptmeds
                           )
                           .ToList();

                if (inptMeds != null)
                {
                    admitMeds.medication = new List<Medication>();

                    StandardService standServ = new StandardService(_icmsContext, _emrContext);

                    foreach (HospitalInpatientAdmissionOrderMedication med in inptMeds)
                    {
                        Medication addMed = new Medication();
                        addMed.admissionMedicationOrderId = med.hospital_inpatient_admission_order_medication_id;
                        addMed.createDate = med.creation_date;
                        addMed.displayCreateDate = (med.creation_date.HasValue) ?
                            med.creation_date.Value.ToShortDateString() + " " + med.creation_date.Value.ToShortTimeString() : "";
                        addMed.medicationName = med.medication_name;
                        addMed.sequenceNumber = med.gcn_seqno;
                        addMed.ndc = med.ndc;
                        addMed.dose = med.dose;
                        addMed.displayDateGive = (med.administered_date.HasValue) ? med.administered_date.Value.ToShortDateString() : "N/A";
                        addMed.timeGiven = (med.time_of_administration != null) ? med.time_of_administration : "N/A";
                        addMed.dateRemoved = med.deleted_date;
                        addMed.displayDateRemoved = (med.deleted_date.HasValue) ?
                            med.deleted_date.Value.ToShortDateString() + " " + med.deleted_date.Value.ToShortTimeString() : "";

                        if (med.administered_user_id.HasValue)
                        {
                            IcmsUser adminByUser = standServ.getAspUser((Guid)med.administered_user_id);

                            addMed.administeredByName = (!string.IsNullOrEmpty(adminByUser.FullName)) ? adminByUser.FullName : "N/A";
                        }
                        else
                        {
                            addMed.administeredByName = "N/A";
                        }

                        admitMeds.medication.Add(addMed);
                    }
                }

            }

            return admitMeds;
        }

        private MedicationAdministered getAdmissionMedicationLastAdministeredData(HospitalInpatientAdmissionOrderMedication medication)
        {
            MedicationAdministered medAdministered = null;

            if (medication.hospital_inpatient_admission_order_medication_id > 0)
            {
                medAdministered = (
                        from medAdmistr in _icmsContext.HospitalInpatientAdmissionOrderMedicationAdministereds
                        where medAdmistr.hospital_inpatient_admission_order_medication_id.Equals(medication.hospital_inpatient_admission_order_medication_id)
                        orderby medAdmistr.administered_date descending
                        select new MedicationAdministered
                        {
                            administeredId = medAdmistr.hosptial_inpatient_admission_order_medication_administered_id,
                            admissionMedicationOrderId = medAdmistr.hospital_inpatient_admission_order_medication_id,
                            administeredDate = medAdmistr.administered_date,
                            displayAdministeredDate = (medAdmistr.administered_date != null) ? 
                                medAdmistr.administered_date.ToShortDateString() + " " + medAdmistr.administered_date.ToShortTimeString() : "N/A",
                            administeredById = medAdmistr.administered_by_user_id
                        }
                    )
                    .Take(1)
                    .FirstOrDefault();
            }

            return medAdministered;
        }

        public Admission updateAdmissionMedicationAllergies(Admission medication)
        {
            Admission allergies = null;
            string assignMedicationAllergy = "";

            HospitalInpatientAdmissionAllergies dbAllergy = (
                                                                    from inptAllergy in _icmsContext.HospitalInpatientAdmissionAllergys
                                                                    where inptAllergy.hospital_inpatient_admission_id.Equals(medication.hospital_inpatient_admission_id)
                                                                    select inptAllergy
                                                                 )
                                                                 .FirstOrDefault();

            if (dbAllergy != null)
            {
                dbAllergy.medication_allergy = medication.allergies.medicationAllergy;
                assignMedicationAllergy = dbAllergy.medication_allergy;
            } 
            else
            {
                HospitalInpatientAdmissionAllergies newAllergy = new HospitalInpatientAdmissionAllergies();
                newAllergy.hospital_inpatient_admission_id = medication.hospital_inpatient_admission_id;
                newAllergy.medication_allergy = medication.allergies.medicationAllergy;
                newAllergy.creation_date = DateTime.Now;
                newAllergy.creation_user_id = medication.usr;

                _icmsContext.HospitalInpatientAdmissionAllergys.Add(newAllergy);

                assignMedicationAllergy = newAllergy.medication_allergy;
            }

            int results = _icmsContext.SaveChanges();
            
            if (results > 0)
            {
                allergies = new Admission();
                allergies.allergies = new Allergy();
                allergies.allergies.medicationAllergy = assignMedicationAllergy;
            }

            return allergies;

        }

        public List<MedicationAdministered> administerAdmissionMedication(MedicationAdministered medication)
        {
            List<MedicationAdministered> medsAdministered = null;

            HospitalInpatientAdmissionOrderMedicationAdministered administeredMedication = new HospitalInpatientAdmissionOrderMedicationAdministered();
            administeredMedication.hospital_inpatient_admission_order_medication_id = medication.admissionMedicationOrderId;
            administeredMedication.administered_date = medication.administeredDate;
            administeredMedication.administered_by_user_id = medication.administeredById;
            administeredMedication.hospital_medication_route_of_administration_id = medication.routeOfAdministrationId;
            administeredMedication.hospital_medication_dosage_forms_id = medication.dosageFormsId;
            administeredMedication.creation_date = medication.creationDate;
            administeredMedication.creation_user_id = medication.creationUserId;

            return medsAdministered;
        } 



        public Admission getAdmissionLabs(string admitId)
        {
            Admission admit = null;

            int hospInptAdmId = 0;

            if (!string.IsNullOrEmpty(admitId) && int.TryParse(admitId, out hospInptAdmId))
            {

                admit = new Admission();

                List<Lab> admissionLabs = null;

                admissionLabs = (
                                    from admits in _icmsContext.HospitalInpatientAdmissions

                                    join orders in _icmsContext.HospitalInpatientAdmissionOrders
                                    on admits.hospital_inpatient_admission_id equals orders.hospital_inpatient_admission_id

                                    join admitLabs in _icmsContext.HospitalInpatientAdmissionOrderLabs
                                    on orders.hospital_inpatient_admission_order_id equals admitLabs.hospital_inpatient_admission_order_id

                                    join hospitalLabs in _icmsContext.LabTypes
                                    on admitLabs.hospital_order_test_id equals hospitalLabs.hospital_order_test_id

                                    join hospitalDeptRef in _icmsContext.HospitalOrderTestDepartmentReferences
                                    on admitLabs.hospital_order_test_id equals hospitalDeptRef.hospital_order_test_id into deptRefs
                                    from hospitalDeptRefs in deptRefs.DefaultIfEmpty()

                                    join hospitalDept in _icmsContext.HospitalDepartments
                                    on hospitalDeptRefs.hospital_department_id equals hospitalDept.hospital_department_id into depts
                                    from hospitalDepts in depts.DefaultIfEmpty()

                                    where admits.hospital_inpatient_admission_id.Equals(hospInptAdmId)
                                    orderby admitLabs.specimen_collection_date descending

                                    select new Lab
                                    {
                                        labId = admitLabs.hospital_inpatient_admission_order_lab_id,
                                        labName = hospitalLabs.test_name,
                                        departmentName = hospitalDepts.hospital_department_name,
                                        collectionDate = admitLabs.specimen_collection_date,
                                        displayCollectionDate = (admitLabs.specimen_collection_date.HasValue) ? admitLabs.specimen_collection_date.Value.ToShortDateString() : "N/A",
                                        completed = admitLabs.completed,
                                    }
                                )
                                .ToList();


                admit.labs = admissionLabs;
            }

            return admit;
        }



        public List<Note> getAdmissionNotesMdt(Note mdtNote)
        {
            List<Note> admissionNotes = null;

            List<Note> dbNotes = (
                                from hospInptAdmMdt in _icmsContext.HospitalInpatientAdmissionMdtNotes

                                join hospnotetyp in _icmsContext.HospitalNoteTypes
                                on hospInptAdmMdt.hospital_note_type_id equals hospnotetyp.hospital_note_type_id

                                where hospInptAdmMdt.hospital_inpatient_admission_id.Equals(mdtNote.admissionId)
                                orderby hospInptAdmMdt.creation_date descending
                                select new Note
                                {
                                    noteId = hospInptAdmMdt.hosptial_inpatient_admission_mdt_note,
                                    recordDate = hospInptAdmMdt.creation_date,
                                    displayRecordDate = (hospInptAdmMdt.creation_date != null) ?
                                        hospInptAdmMdt.creation_date.ToShortDateString() + " " + hospInptAdmMdt.creation_date.ToShortTimeString()
                                        : "N/A",
                                    caseOwnerId = hospInptAdmMdt.creation_user_id,
                                    noteText = hospInptAdmMdt.hospital_note,
                                    hospitalNoteTypeId = hospInptAdmMdt.hospital_note_type_id,
                                    hospitalNoteTypeName = hospnotetyp.note_type_name
                                }
                            )
                            .ToList();


            if (dbNotes != null && dbNotes.Count > 0)
            {                

                if (mdtNote.noteTypeIds.Contains("all"))
                {
                    admissionNotes = dbNotes;
                }  else
                {

                    string[] list = mdtNote.noteTypeIds.ToArray();
                    int[] typIds = Array.ConvertAll(list, lst => int.Parse(lst));

                    admissionNotes = (
                                from nts in dbNotes
                                where typIds.Contains(nts.hospitalNoteTypeId)
                                select nts
                            )
                            .ToList();
                }

                StandardService standServ = new StandardService(_icmsContext, _emrContext);

                foreach (Note nte in admissionNotes)
                {
                    if (!nte.caseOwnerId.Equals(Guid.Empty))
                    {
                        IcmsUser enteredBy = standServ.getAspUser((Guid)nte.caseOwnerId);

                        nte.caseOwnerName = enteredBy.FullName;
                    }
                }
            }

            return admissionNotes;
        }

        public List<Note> updateAdmissionNotesMdt(Note mdtNote, bool getReturnNotes)
        {
            List<Note> admissionNotes = null;

            HospitalInpatientAdmissionMdtNote addNote = new HospitalInpatientAdmissionMdtNote();
            addNote.creation_date = mdtNote.recordDate;
            addNote.creation_user_id = mdtNote.caseOwnerId;
            addNote.hospital_note = mdtNote.noteText;
            addNote.hospital_note_type_id = mdtNote.hospitalNoteTypeId;
            addNote.hospital_inpatient_admission_id = mdtNote.admissionId;

            _icmsContext.HospitalInpatientAdmissionMdtNotes.Add(addNote);

            int result = _icmsContext.SaveChanges();

            if (getReturnNotes)
            {
                admissionNotes = getAdmissionNotesMdt(mdtNote);            
            }
            else
            {
                admissionNotes = new List<Note>();
                Note returnNote = new Note();

                returnNote.noteId = addNote.hosptial_inpatient_admission_mdt_note;

                admissionNotes.Add(returnNote);
            }

            return admissionNotes;
        }



        public Admission getAdmissionDocuments(string admitId)
        {
            Admission admit = null;

            int hospInptAdmId = 0;

            if (!string.IsNullOrEmpty(admitId) && int.TryParse(admitId, out hospInptAdmId))
            {

                admit = new Admission();

                List<DocumentForm> admissionDocs = null;

                admissionDocs = (
                                    from admitDocs in _icmsContext.HospitalInpatientAdmissionDocumentForms                                    
                                    where admitDocs.hospital_inpatient_admission_id.Equals(hospInptAdmId)
                                    orderby admitDocs.creation_date descending

                                    select new DocumentForm
                                    {
                                        documentId = admitDocs.hosptial_inpatient_admission_document_form_id,
                                        creationDate = admitDocs.creation_date,
                                        displayCreationDate = (admitDocs.creation_date.HasValue) ? admitDocs.creation_date.Value.ToShortDateString() : "N/A",
                                        documentContentType = admitDocs.content_type,
                                        documentFileName = admitDocs.document_name,
                                    }
                                )
                                .ToList();

                    admit.documents = new List<DocumentForm>();
                    admit.documents = admissionDocs;
            }

            return admit;
        }

        public DocumentForm getAdmissionDocument(string documentId)
        {
         
            DocumentForm admissionDoc = null;

            int docId = 0;

            if (!string.IsNullOrEmpty(documentId) && int.TryParse(documentId, out docId))
            {

                admissionDoc = (
                                    from admitDocs in _icmsContext.HospitalInpatientAdmissionDocumentForms
                                    where admitDocs.hosptial_inpatient_admission_document_form_id.Equals(docId)
                                    orderby admitDocs.creation_date descending

                                    select new DocumentForm
                                    {
                                        documentId = admitDocs.hosptial_inpatient_admission_document_form_id,
                                        creationDate = admitDocs.creation_date,
                                        displayCreationDate = (admitDocs.creation_date.HasValue) ? admitDocs.creation_date.Value.ToShortDateString() : "N/A",
                                        documentContentType = admitDocs.content_type,
                                        documentFileName = admitDocs.document_name,
                                        documentImage = admitDocs.document_image,
                                        documentBase64 = Convert.ToBase64String(admitDocs.document_image)
            }
                                )
                                .FirstOrDefault();
            }

            return admissionDoc;
        }

        public Admission uploadAdmissionDocument(DocumentForm doc)
        {
            Admission admit = null;

            if (doc.admissionId > 0)
            {

                HospitalInpatientAdmissionDocumentForm admitDoc = new HospitalInpatientAdmissionDocumentForm();
                admitDoc.hospital_inpatient_admission_id = doc.admissionId;
                admitDoc.creation_date = doc.creationDate;
                admitDoc.creation_user_id = doc.usr;
                admitDoc.document_name = doc.documentFileName;
                admitDoc.document_image = doc.documentImage;
                admitDoc.content_type = doc.documentContentType;

                _icmsContext.HospitalInpatientAdmissionDocumentForms.Add(admitDoc);
                _icmsContext.SaveChanges();
            
                admit = getAdmissionDocuments(doc.admissionId.ToString());            
            }

            return admit;
        }







        private List<string> getInpatientAdmissionUserDefaults(Admission admit)
        {
            List<string> defaults = null;

            if (!admit.usr.Equals(Guid.Empty))
            {

                Admission admissionUserDefaults = getUserInpatientAdmissionDashboardDefaults(admit.hospital_inpatient_admission_id.ToString(),
                                                                                             admit.usr.ToString());

                if (admissionUserDefaults != null)
                {
                    defaults = admissionUserDefaults.userInpatientAdmissionDashboardDefaults;
                }

            }


            return defaults;
        }

        public Admission getUserInpatientAdmissionDashboardDefaults(string admitId, string userId)
        {
            Admission dashboard = null;

            int hospInptAdmitId = 0;
            Guid simsId = Guid.Empty;

            if (int.TryParse(admitId, out hospInptAdmitId) && 
                Guid.TryParse(userId, out simsId))
            {
                
                List<HospitalInpatientAdmissionUserDashboardDefaults> defaults = (
                                                                                    from userdefs in _icmsContext.HospitalInpatientAdmissionUserDashboardDefaults
                                                                                    where userdefs.hospital_inpatient_admission_id.Equals(hospInptAdmitId)
                                                                                    && userdefs.userId.Equals(simsId)
                                                                                    select userdefs
                                                                                  )
                                                                                  .ToList();

                if (defaults != null)
                {
                    dashboard = new Admission();

                    dashboard.userInpatientAdmissionDashboardDefaults = new List<String>();

                    foreach(HospitalInpatientAdmissionUserDashboardDefaults dashdefault in defaults)
                    {
                        if (!string.IsNullOrEmpty(dashdefault.dashboard_button_id))
                        {
                            dashboard.userInpatientAdmissionDashboardDefaults.Add(dashdefault.dashboard_button_id);
                        }
                    }
                }

            }

            return dashboard;
        }


        public Admission updateUserInpatientAdmissionDashboardDefaults(Admission defaults)
        {
            Admission dashboard = null;

            if (defaults.hospital_inpatient_admission_id > 0 &&
                !defaults.usr.Equals(Guid.Empty))
            {

                removeUserInpatientAdmissionDashboardDefaults(defaults.hospital_inpatient_admission_id, defaults.usr);
                dashboard = addUserInpatientAdmissionDashboardDefaults(defaults);

            }

            return dashboard;
        }

        public bool removeUserInpatientAdmissionDashboardDefaults(int hospitalInpatientAdmissionId, Guid userId)
        {
            bool removed = false;


            _icmsContext.HospitalInpatientAdmissionUserDashboardDefaults.RemoveRange(
                _icmsContext.HospitalInpatientAdmissionUserDashboardDefaults.Where(
                    dash => dash.hospital_inpatient_admission_id.Equals(hospitalInpatientAdmissionId)
                    && dash.userId.Equals(userId)
                 )
            );

            int results = _icmsContext.SaveChanges();

            if (results > 0)
            {
                removed = true;
            }

            return removed;
        }

        public Admission addUserInpatientAdmissionDashboardDefaults(Admission defaults)
        {
            Admission dashboard = null;

            if (defaults.userInpatientAdmissionDashboardDefaults != null)
            {

                foreach (String defs in defaults.userInpatientAdmissionDashboardDefaults)
                {
                    HospitalInpatientAdmissionUserDashboardDefaults userDefault = new HospitalInpatientAdmissionUserDashboardDefaults();
                    userDefault.userId = defaults.usr;
                    userDefault.hospital_inpatient_admission_id = defaults.hospital_inpatient_admission_id;
                    userDefault.dashboard_button_id = defs;

                    _icmsContext.HospitalInpatientAdmissionUserDashboardDefaults.Add(userDefault);

                }

                if (_icmsContext.ChangeTracker.HasChanges())
                {

                    int results = _icmsContext.SaveChanges();

                    if (results > 0)
                    {
                        dashboard = getInpatientAdmission(defaults);
                    }

                }


            }

            return dashboard;
        }




        public Admission insertAdmissionVitalSign(Admission admit)
        {
            
            Admission patientAdmit = null;

            if (admit.hospital_inpatient_admission_id > 0 && 
                !admit.member_id.Equals(Guid.Empty))
            {

                if (admit.vitalSigns != null && 
                    admit.vitalSigns.Count.Equals(1))
                {

                    MemberVitals addVital = new MemberVitals();

                    addVital.member_id = admit.member_id;
                    addVital.hospital_inpatient_admission_id = admit.hospital_inpatient_admission_id;
                    addVital.date_measured = DateTime.Now;
                    addVital.deleted_flag = false;
                    addVital.physician_reported_flag = false;
                    addVital.pulse_is_regular = true;
                    addVital.respiration_is_regular = true;

                    assignVitalSignReadings(admit.vitalSigns[0], addVital);

                    _icmsContext.MemberVitalses.Add(addVital);
                    int result = _icmsContext.SaveChanges();

                    patientAdmit = (result > 0) ? getInpatientAdmission(admit) : null;

                }

            }

            return patientAdmit;

        }

        private void assignVitalSignReadings(VitalSign readings, MemberVitals addVital)
        {
            //temperature
            if (readings.temperature > 0)
            {
                if (readings.isFarenheit)
                {
                    addVital.temperature_in_fahrenheit = readings.temperature;
                }
                else
                {
                    addVital.temperature_in_celsius = readings.temperature;
                }

                if (readings.isRectal)
                {
                    addVital.hospital_temperature_site_id = 3; //retcum
                }

                if (readings.alertHighTemperature != null && (bool)readings.alertHighTemperature)
                {
                    addVital.alert_high_temperature = true;
                }
                else if (readings.alertLowTemperature != null && (bool)readings.alertLowTemperature)
                {
                    addVital.alert_low_temperature = true;
                }
            }

            //pulse rate
            if (readings.pulseRate > 0)
            {

                addVital.pulse_per_minute = readings.pulseRate;

                if (readings.alertHighPulseRate != null && (bool)readings.alertHighPulseRate)
                {
                    addVital.alert_high_pulse_rate = true;
                    addVital.pulse_is_regular = false;
                }
                else if (readings.alertLowPulseRate != null && (bool)readings.alertLowPulseRate)
                {
                    addVital.alert_low_pulse_rate = true;
                    addVital.pulse_is_regular = false;
                }

            }

            //respiration rate
            if (readings.respirationRate > 0)
            {

                addVital.respiration_per_minute = readings.respirationRate;

                if (readings.alertHighRespirationRate != null && (bool)readings.alertHighRespirationRate)
                {
                    addVital.alert_high_respiration_rate = true;
                    addVital.respiration_is_regular = false;
                }
                else if (readings.alertLowRespirationRate != null && (bool)readings.alertLowRespirationRate)
                {
                    addVital.alert_low_respiration_rate= true;
                    addVital.respiration_is_regular = false;
                }

            }

            //blood pressure
            if (readings.systolicBloodPressure > 0 && readings.diastolicBloodPressure > 0)
            {

                addVital.seated_blood_pressure_systolic = readings.systolicBloodPressure;
                addVital.seated_blood_pressure_diastolic = readings.diastolicBloodPressure;

                if (readings.alertHighBloodPressure != null && (bool)readings.alertHighBloodPressure)
                {
                    addVital.alert_high_blood_pressure = true;
                }
                else if (readings.alertLowBloodPressure != null && (bool)readings.alertLowBloodPressure)
                {
                    addVital.alert_low_blood_pressure = true;
                }

            }

        }







        public InpatientChart updateAllChartSources(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.chartId > 0)
            {
                HospitalInpatientAdmissionChart updateChart = (
                                                                    from chrts in _icmsContext.HospitalInpatientAdmissionCharts
                                                                    where chrts.hospital_inpatient_admission_chart_id.Equals(chart.chartId)
                                                                    select chrts
                                                               )
                                                               .FirstOrDefault();

                if (updateChart != null && !string.IsNullOrEmpty(updateChart.chart_table_name))
                {
                    returnChart = updateChartTable(updateChart.chart_table_name, chart);
                } else if (updateChart != null && !string.IsNullOrEmpty(updateChart.chart_type))
                {
                    returnChart = updateChartType(updateChart.chart_type, chart);
                }

            }

            return returnChart;
        }


        public InpatientChart updateChartSource(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.sourceId > 0)
            {
                InpatientChartSource dbSource = (
                                                    from srces in _icmsContext.HospitalInpatientAdmissionChartSources

                                                    join chrts in _icmsContext.HospitalInpatientAdmissionCharts
                                                    on srces.hospital_inpatient_admission_chart_id equals chrts.hospital_inpatient_admission_chart_id                

                                                    where srces.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                                    select new InpatientChartSource
                                                    {
                                                        chartTableName = chrts.chart_table_name,
                                                        chartType = chrts.chart_type,
                                                        sourceId = srces.hospital_inpatient_admission_chart_source_id
                                                    }
                                                )
                                                .FirstOrDefault();

                if (dbSource != null)
                {
                    if (!string.IsNullOrEmpty(dbSource.chartTableName))
                    {

                        dbSource.chartTableId = chart.chartTableId;

                        returnChart = updateChartTableItem(dbSource, chart);

                    } else if (!string.IsNullOrEmpty(dbSource.chartType))
                    {
                        
                        dbSource.chartTableId = chart.chartTableId;
                        
                        returnChart = updateChartTypeItem(dbSource, chart);

                    }
                }
            }

            return returnChart;
        }


        private InpatientChart updateChartTable(string tableName, InpatientChart chart)
        {
            InpatientChart returnChart = null;

            switch (tableName)
            {
                case "MEMBER_VITALS":
                    returnChart = updateVitalSignSources(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                    returnChart = updateCardiacAssessmentSources(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                    returnChart = updateNeurogolicalAssessment(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                    returnChart = updateBloodGasAssessment(chart);
                    break;
            }

            return returnChart;
        }

        private InpatientChart updateChartTableItem(InpatientChartSource dbSource, InpatientChart chart)
        {
            InpatientChart returnChart = null;

            switch (dbSource.chartTableName)
            {
                case "MEMBER_VITALS":
                    returnChart = updateVitalSignSources(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_CARDIAC":
                    returnChart = updateCardiacAssessmentSources(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_NURSING_PROCESS_ASSESSMENT_BASIC_NEUROLOGICAL":
                    returnChart = updateNeurogolicalAssessment(chart);
                    break;
                case "HOSPITAL_INPATIENT_ADMISSION_BLOOD_GAS":
                    returnChart = updateBloodGasAssessment(chart);
                    break;
            }

            return returnChart;
        }

        private InpatientChart updateChartType(string chartType, InpatientChart chart)
        {
            InpatientChart returnChart = null;

            switch (chartType)
            {
                case "demographicChart":
                    returnChart = updateDemographicSources(chart);
                    break;

                case "admissionChart":
                    returnChart = updateAdmissionSources(chart);
                    break;
            }

            return returnChart;
        }

        private InpatientChart updateChartTypeItem(InpatientChartSource dbSource, InpatientChart chart)
        {
            InpatientChart returnChart = null;

            switch (dbSource.chartType)
            {
                case "demographicChart":
                    returnChart = updateDemographicSources(chart);
                    break;

                case "admissionChart":
                    returnChart = updateAdmissionSources(chart);
                    break;
            }

            return returnChart;
        }


        private InpatientChart updateDemographicSources(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (!chart.patientId.Equals(Guid.Empty))
            {
                Member dbMember = null;

                dbMember = (
                            from pats in _icmsContext.Patients
                            where pats.member_id.Equals(chart.patientId)
                            select pats
                            )
                            .FirstOrDefault();
              
                int results = 0;

                if (dbMember != null)
                {

                    dbMember.update_user_id = chart.usr;
                    dbMember.last_update_user_id = chart.usr;
                    dbMember.member_updated = DateTime.Now;

                    //update existing vital sign
                    setDemographicSourceValue(ref dbMember, chart.sources);

                    _icmsContext.Patients.Update(dbMember);
                    results = _icmsContext.SaveChanges();
                }
                
                if (results > 0)
                {
                    updatePatientHealthPlanReference(chart);
                    updatePatientAddress(chart);
                    updatePatientMedicalHistory(chart);
                    updatePatientFamilyHistory(chart);
                    updatePatientNextOfKin(chart);
                    //NEED GENERAL PRACTIONER//

                    if (chart.admissionId > 0)
                    {
                        updatePatientAdmissionContact(chart);
                    }

                    returnChart = getInpatientChart(chart);

                }

            }

            return returnChart;
        }

        private bool updatePatientHealthPlanReference(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty))
            {
                MemberHealthPlanReference dbHealthPlanRef = null;

                dbHealthPlanRef = (
                                    from hlthplnref in _icmsContext.MemberHealthPlanReferences
                                    where hlthplnref.member_id.Equals(chart.patientId)
                                    select hlthplnref
                                  )
                                  .FirstOrDefault();

                int results = 0;

                if (dbHealthPlanRef != null)
                {
                    //update existing member health plan reference
                    dbHealthPlanRef.last_update_date = DateTime.Now;
                    dbHealthPlanRef.update_user_id = chart.usr;

                    setPatientHealthPlanReferenceSourceValue(ref dbHealthPlanRef, chart.sources);

                    _icmsContext.MemberHealthPlanReferences.Update(dbHealthPlanRef);
                    results = _icmsContext.SaveChanges();
                } 
                else
                {
                    dbHealthPlanRef = new MemberHealthPlanReference();
                    dbHealthPlanRef.member_id = (Guid)chart.patientId;
                    dbHealthPlanRef.creation_date = DateTime.Now;
                    dbHealthPlanRef.created_user_id = chart.usr;

                    setPatientHealthPlanReferenceSourceValue(ref dbHealthPlanRef, chart.sources);

                    _icmsContext.MemberHealthPlanReferences.Add(dbHealthPlanRef);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updatePatientAddress(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty))
            {
                MemberAddress dbAddress = null;

                dbAddress = (
                                from addrs in _icmsContext.MemberAddresses
                                where addrs.member_id.Equals(chart.patientId)
                                orderby addrs.member_address_id descending
                                select addrs
                            )
                            .Take(1)
                            .FirstOrDefault();

                int results = 0;

                if (dbAddress != null)
                {
                    //update existing member address
                    setPatientAddressSourceValue(ref dbAddress, chart.sources);

                    _icmsContext.MemberAddresses.Update(dbAddress);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbAddress = new MemberAddress();
                    dbAddress.member_id = (Guid)chart.patientId;
                    dbAddress.address_type_id = 1;

                    setPatientAddressSourceValue(ref dbAddress, chart.sources);

                    _icmsContext.MemberAddresses.Add(dbAddress);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updatePatientMedicalHistory(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty))
            {
                MemberMedicalHistory dbMedHistory = null;

                dbMedHistory = (
                                    from medhist in _icmsContext.MemberMedicalHistorys
                                    where medhist.member_id.Equals(chart.patientId)
                                    select medhist
                                )
                                .FirstOrDefault();

                int results = 0;

                if (dbMedHistory != null)
                {
                    dbMedHistory.last_update_ate = DateTime.Now;
                    dbMedHistory.last_update_user_id = chart.usr;

                    //update existing member address
                    setPatientMedicalHistorySourceValue(ref dbMedHistory, chart.sources);

                    _icmsContext.MemberMedicalHistorys.Update(dbMedHistory);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbMedHistory = new MemberMedicalHistory();
                    dbMedHistory.member_id = (Guid)chart.patientId;
                    dbMedHistory.creation_date = DateTime.Now;
                    dbMedHistory.creation_user_id = chart.usr;

                    setPatientMedicalHistorySourceValue(ref dbMedHistory, chart.sources);

                    _icmsContext.MemberMedicalHistorys.Add(dbMedHistory);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updatePatientFamilyHistory(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty))
            {
                MemberFamilyMedicalHistory dbFamilyMedicalHistory = null;

                dbFamilyMedicalHistory = (
                                            from fammedhist in _icmsContext.MemberFamilyMedicalHistorys
                                            where fammedhist.member_id.Equals(chart.patientId)
                                            select fammedhist
                                         )
                                         .FirstOrDefault();

                int results = 0;

                if (dbFamilyMedicalHistory != null)
                {
                    dbFamilyMedicalHistory.last_update_ate = DateTime.Now;
                    dbFamilyMedicalHistory.last_update_user_id = chart.usr;

                    //update existing member address
                    setPatientFamilyMedicalHistorySourceValue(ref dbFamilyMedicalHistory, chart.sources);

                    _icmsContext.MemberFamilyMedicalHistorys.Update(dbFamilyMedicalHistory);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbFamilyMedicalHistory = new MemberFamilyMedicalHistory();
                    dbFamilyMedicalHistory.member_id = (Guid)chart.patientId;
                    dbFamilyMedicalHistory.creation_date = DateTime.Now;
                    dbFamilyMedicalHistory.creation_user_id = chart.usr;

                    setPatientFamilyMedicalHistorySourceValue(ref dbFamilyMedicalHistory, chart.sources);

                    _icmsContext.MemberFamilyMedicalHistorys.Add(dbFamilyMedicalHistory);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updatePatientNextOfKin(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty))
            {
                MemberNextOfKin dbNextOfKin = null;

                dbNextOfKin = (
                                from nxtofkin in _icmsContext.MemberNextOfKins
                                where nxtofkin.member_id.Equals(chart.patientId)
                                select nxtofkin
                                )
                                .FirstOrDefault();

                int results = 0;

                if (dbNextOfKin != null)
                {
                    dbNextOfKin.last_update_date = DateTime.Now;
                    dbNextOfKin.last_update_user_id = chart.usr;

                    //update existing member address
                    setPatientMemberNextOfKinSourceValue(ref dbNextOfKin, chart.sources);

                    _icmsContext.MemberNextOfKins.Update(dbNextOfKin);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbNextOfKin = new MemberNextOfKin();
                    dbNextOfKin.member_id = (Guid)chart.patientId;
                    dbNextOfKin.creation_date = DateTime.Now;
                    dbNextOfKin.creation_user_id = chart.usr;

                    setPatientMemberNextOfKinSourceValue(ref dbNextOfKin, chart.sources);

                    _icmsContext.MemberNextOfKins.Add(dbNextOfKin);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updatePatientAdmissionContact(InpatientChart chart)
        {

            if (!chart.patientId.Equals(Guid.Empty) && chart.admissionId > 0)
            {
                rMemberAdmissionContacts dbAdmitContact = null;

                dbAdmitContact = (
                                    from contacts in _icmsContext.rMemberAdmissionContactses
                                    where contacts.member_id.Equals(chart.patientId)
                                    && contacts.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                    select contacts
                                 )
                                 .FirstOrDefault();

                int results = 0;

                if (dbAdmitContact != null)
                {
                    dbAdmitContact.last_update_date = DateTime.Now;
                    dbAdmitContact.last_update_user_id = chart.usr;

                    //update existing admission contact
                    setPatientAdmissionContactSourceValue(ref dbAdmitContact, chart.sources);

                    _icmsContext.rMemberAdmissionContactses.Update(dbAdmitContact);
                    results = _icmsContext.SaveChanges();
                } 
                else
                {
                    dbAdmitContact = new rMemberAdmissionContacts();
                    dbAdmitContact.member_id = (Guid)chart.patientId;
                    dbAdmitContact.hospital_inpatient_admission_id = chart.admissionId;
                    dbAdmitContact.creation_date = DateTime.Now;
                    dbAdmitContact.creation_user_id = chart.usr;

                    setPatientAdmissionContactSourceValue(ref dbAdmitContact, chart.sources);

                    _icmsContext.rMemberAdmissionContactses.Add(dbAdmitContact);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }



        private InpatientChart updateAdmissionSources(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmission hospinptadm = null;

                hospinptadm = (
                                from admits in _icmsContext.HospitalInpatientAdmissions
                                where admits.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                select admits
                              )
                              .FirstOrDefault();

                int results = 0;

                if (hospinptadm != null)
                {

                    hospinptadm.last_update_user_id = chart.usr;
                    hospinptadm.last_update_date = DateTime.Now;

                    //update existing vital sign
                    setInpatientAdmissionSourceValue(ref hospinptadm, chart.sources);

                    _icmsContext.HospitalInpatientAdmissions.Update(hospinptadm);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    updateAdmissionReasonForVisit(chart);
                    updateAdmissionSourceForAdmission(chart);

                    returnChart = getInpatientChart(chart);
                }

            }

            return returnChart;
        }

        private bool updateAdmissionReasonForVisit(InpatientChart chart)
        {

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmissionReasonForVisit dbReasonForVisit = null;

                dbReasonForVisit = (
                                    from rsnforvisit in _icmsContext.HospitalInpatientAdmissionReasonForVisits
                                    where rsnforvisit.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                    select rsnforvisit
                                   )
                                   .FirstOrDefault();

                int results = 0;

                if (dbReasonForVisit != null)
                {
                    dbReasonForVisit.update_date = DateTime.Now;
                    dbReasonForVisit.update_user_id= chart.usr;

                    //update existing member address
                    setAdmissionReasonFormVisitSourceValue(ref dbReasonForVisit, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionReasonForVisits.Update(dbReasonForVisit);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbReasonForVisit = new HospitalInpatientAdmissionReasonForVisit();
                    dbReasonForVisit.hospital_inpatient_admission_id = chart.admissionId;
                    dbReasonForVisit.creation_date = DateTime.Now;
                    dbReasonForVisit.creation_user_id = chart.usr;

                    setAdmissionReasonFormVisitSourceValue(ref dbReasonForVisit, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionReasonForVisits.Add(dbReasonForVisit);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }

        private bool updateAdmissionSourceForAdmission(InpatientChart chart)
        {

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmissionSource dbAdmissionSource = null;

                dbAdmissionSource = (
                                        from admitsource in _icmsContext.HospitalInpatientAdmissionSources
                                        where admitsource.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                        select admitsource
                                    )
                                    .FirstOrDefault();

                int results = 0;

                if (dbAdmissionSource != null)
                {
                    dbAdmissionSource.last_update_date = DateTime.Now;
                    dbAdmissionSource.last_update_user_id= chart.usr;

                    //update existing member address
                    setAdmissionSourceSourceValue(ref dbAdmissionSource, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionSources.Update(dbAdmissionSource);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    dbAdmissionSource = new HospitalInpatientAdmissionSource();
                    dbAdmissionSource.hospital_inpatient_admission_id = (int)chart.admissionId;
                    dbAdmissionSource.creation_date = DateTime.Now;
                    dbAdmissionSource.creation_user_id = chart.usr;

                    setAdmissionSourceSourceValue(ref dbAdmissionSource, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionSources.Add(dbAdmissionSource);
                    results = _icmsContext.SaveChanges();
                }

                if (results > 0)
                {
                    return true;
                }

            }

            return false;

        }


        private InpatientChart updateVitalSignSources(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.admissionId > 0)
            {
                MemberVitals dbVital = null;
                bool hasNotes = false;

                if (chart.chartTableId > 0)
                {

                    dbVital = (
                                from vitals in _icmsContext.MemberVitalses
                                where vitals.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                && vitals.member_vitals_id.Equals(chart.chartTableId)
                                select vitals
                               )
                               .FirstOrDefault();

                }

                int results = 0;

                if (dbVital != null)
                {
                    //update existing vital sign
                    hasNotes = setVitalSignSourceValue(ref dbVital, chart.sources);

                    dbVital.last_update_date = chart.creationDate;
                    dbVital.last_update_user_id = chart.usr;

                    _icmsContext.MemberVitalses.Update(dbVital);
                    results = _icmsContext.SaveChanges();
                } 
                else
                {
                    if (!chart.patientId.Equals(Guid.Empty) && chart.admissionId > 0)
                    {
                        DateTime measuredDate = (!chart.dateMeasured.Equals(DateTime.MinValue)) ? chart.dateMeasured : DateTime.Now;

                        //add new vital sign
                        dbVital = new MemberVitals();

                        dbVital.member_id = (Guid)chart.patientId;
                        dbVital.date_measured = measuredDate;
                        dbVital.hospital_inpatient_admission_id = chart.admissionId;
                        dbVital.creation_date = chart.creationDate;
                        dbVital.creation_user_id = chart.usr;

                        hasNotes = setVitalSignSourceValue(ref dbVital, chart.sources);

                        _icmsContext.MemberVitalses.Add(dbVital); 
                        results = _icmsContext.SaveChanges();

                    }

                }

                if (results > 0)
                {
                    
                    if (hasNotes)
                    {

                        if (addChartMdtNotes(ref chart))
                        {
                            addVitalSignSourceMdtNoteReferences(chart, dbVital);
                        }
                    }

                    addVitalSignSourceHighlightReference(chart, dbVital);

                    returnChart = getInpatientChart(chart);
                }

            }

            return returnChart;
        }

        private InpatientChart updateCardiacAssessmentSources(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac dbCardiacAssessment = null;
                bool hasNotes = false;

                if (chart.chartTableId > 0)
                {

                    dbCardiacAssessment = (
                                            from cardiac in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs
                                            where cardiac.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                            && cardiac.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(chart.chartTableId)
                                            select cardiac
                                          )
                                          .FirstOrDefault();

                }

                int results = 0;

                if (dbCardiacAssessment != null)
                {
                    dbCardiacAssessment.last_update_date = DateTime.Now;
                    dbCardiacAssessment.last_update_user_id = chart.usr;

                    //update existing cardiac assessment
                    hasNotes= setCardiacAssessmentSourceValue(ref dbCardiacAssessment, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs.Update(dbCardiacAssessment);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    if (!chart.patientId.Equals(Guid.Empty) && chart.admissionId > 0)
                    {
                        DateTime measuredDate = (!chart.dateMeasured.Equals(DateTime.MinValue)) ? chart.dateMeasured : DateTime.Now;

                        //add new vital sign
                        dbCardiacAssessment = new HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac();

                        dbCardiacAssessment.hospital_inpatient_admission_id = (int)chart.admissionId;
                        dbCardiacAssessment.date_measured = measuredDate;
                        dbCardiacAssessment.creation_date = DateTime.Now;
                        dbCardiacAssessment.creation_user_id = chart.usr;

                        hasNotes = setCardiacAssessmentSourceValue(ref dbCardiacAssessment, chart.sources);

                        _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiacs.Add(dbCardiacAssessment);
                        results = _icmsContext.SaveChanges();

                    }

                }

                if (results > 0)
                {
                    if (hasNotes)
                    {
                        if (addChartMdtNotes(ref chart))
                        {
                            addCardiacAssessmentSourceMdtNoteReferences(chart, dbCardiacAssessment);
                        }
                    }

                    addCardiacSourceHighlightReference(chart, dbCardiacAssessment);

                    returnChart = getInpatientChart(chart);
                }
            }

            return returnChart;
        }

        private InpatientChart updateNeurogolicalAssessment(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment = null;
                bool hasNotes = false;

                if (chart.chartTableId > 0)
                {

                    assessment = (
                                    from cardiac in _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals
                                    where cardiac.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                    && cardiac.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(chart.chartTableId)
                                    select cardiac
                                    )
                                    .FirstOrDefault();

                }

                int results = 0;

                if (assessment != null)
                {
                    assessment.last_update_date = DateTime.Now;
                    assessment.last_update_user_id = chart.usr;

                    //update existing assessment
                    hasNotes = setNeurologicalAssessmentSourceValue(ref assessment, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals.Update(assessment);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    if (!chart.patientId.Equals(Guid.Empty) && chart.admissionId > 0)
                    {
                        DateTime measuredDate = (!chart.dateMeasured.Equals(DateTime.MinValue)) ? chart.dateMeasured : DateTime.Now;

                        //add new assessment
                        assessment = new HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological();

                        assessment.hospital_inpatient_admission_id = (int)chart.admissionId;
                        assessment.date_measured = measuredDate;
                        assessment.creation_date = DateTime.Now;
                        assessment.creation_user_id = chart.usr;

                        hasNotes = setNeurologicalAssessmentSourceValue(ref assessment, chart.sources);

                        _icmsContext.HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurologicals.Add(assessment);
                        results = _icmsContext.SaveChanges();

                    }

                }

                if (results > 0)
                {
                    if (hasNotes)
                    {
                        if (addChartMdtNotes(ref chart))
                        {
                            addNeurologicalAssessmentSourceMdtNoteReferences(chart, assessment);
                        }
                    }

                    addNeurologicalSourceHighlightReference(chart, assessment);

                    returnChart = getInpatientChart(chart);
                }

            }

            return returnChart;
        }

        private InpatientChart updateBloodGasAssessment(InpatientChart chart)
        {
            InpatientChart returnChart = null;

            if (chart.admissionId > 0)
            {
                HospitalInpatientAdmissionBloodGas assessment = null;
                bool hasNotes = false;

                if (chart.chartTableId > 0)
                {

                    assessment = (
                                    from chrtAssess in _icmsContext.HospitalInpatientAdmissionBloodGases
                                    where chrtAssess.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                    && chrtAssess.hosptial_inpatient_admission_blood_gas_id.Equals(chart.chartTableId)
                                    select chrtAssess
                                    )
                                    .FirstOrDefault();

                }

                int results = 0;

                if (assessment != null)
                {
                    assessment.last_update_date = DateTime.Now;
                    assessment.last_update_user_id = chart.usr;

                    //update existing assessment
                    hasNotes = setBloodGasAssessmentSourceValue(ref assessment, chart.sources);

                    _icmsContext.HospitalInpatientAdmissionBloodGases.Update(assessment);
                    results = _icmsContext.SaveChanges();
                }
                else
                {
                    if (!chart.patientId.Equals(Guid.Empty) && chart.admissionId > 0)
                    {
                        DateTime measuredDate = (!chart.dateMeasured.Equals(DateTime.MinValue)) ? chart.dateMeasured : DateTime.Now;

                        //add new assessment
                        assessment = new HospitalInpatientAdmissionBloodGas();

                        assessment.hospital_inpatient_admission_id = (int)chart.admissionId;
                        assessment.date_measured = measuredDate;
                        assessment.creation_date = DateTime.Now;
                        assessment.creation_user_id = chart.usr;

                        hasNotes = setBloodGasAssessmentSourceValue(ref assessment, chart.sources);

                        _icmsContext.HospitalInpatientAdmissionBloodGases.Add(assessment);
                        results = _icmsContext.SaveChanges();

                    }

                }

                if (results > 0)
                {
                    if (hasNotes)
                    {
                        if (addChartMdtNotes(ref chart))
                        {
                            addBloodGasAssessmentSourceMdtNoteReferences(chart, assessment);
                        }
                    }

                    addBloodSourceHighlightReference(chart, assessment);

                    returnChart = getInpatientChart(chart);
                }

            }

            return returnChart;
        }
        


        private void setDemographicSourceValue(ref Member patient, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "firstName":
                        patient.member_first_name = source.sourceTextValue;
                        break;
                    case "lastName":
                        patient.member_last_name = source.sourceTextValue;
                        break;
                    case "dateOfBirthDisplay":
                        patient.member_birth = source.sourceDateValue;
                        break;
                    

                    //case "generalProvider.fullName":
                    //    patient.respiration_per_minute = source.sourceIntValue;
                    //    break;
                    //case "generalProvider.phoneNumber":
                    //    patient.fi02 = source.sourceIntValue;
                    //    break;

                }
            }

        }

        private void setPatientHealthPlanReferenceSourceValue(ref MemberHealthPlanReference healthPlanRef, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "mrn":
                        healthPlanRef.mrn = source.sourceTextValue;
                        break;                   
                }
            }

        }

        private void setPatientAddressSourceValue(ref MemberAddress address, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "addresses.address_line1":

                        if (source.sourceTextValue.Length > 50)
                        {
                            address.address_line1 = source.sourceTextValue.Substring(0, 50);
                            address.address_line2 = source.sourceTextValue.Substring(50, 50);
                        } else
                        {
                            address.address_line1 = source.sourceTextValue;
                        }
                        break;

                    case "addresses.city":
                        address.city = source.sourceTextValue;
                        break;

                    case "addresses.state_abbrev":
                        address.state_abbrev= source.sourceTextValue;
                        break;
                }
            }

        }

        private void setPatientMedicalHistorySourceValue(ref MemberMedicalHistory medicalHistory, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "medicalHistory.history":
                        medicalHistory.medical_history = source.sourceTextValue;
                        break;
                }
            }

        }

        private void setPatientFamilyMedicalHistorySourceValue(ref MemberFamilyMedicalHistory familyMedicalHistory, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "familyMedicalHistory.history":
                        familyMedicalHistory.family_history = source.sourceTextValue;
                        break;
                }
            }

        }

        private void setPatientMemberNextOfKinSourceValue(ref MemberNextOfKin nextOfKin, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "nextOfKin.fullName":
                        string firstName = "";
                        string lastName = "";

                        int firstSpace = source.sourceTextValue.IndexOf(" ");

                        if (firstSpace > 0)
                        {
                            firstName = source.sourceTextValue.Substring(0, firstSpace);

                            if (firstName.Length > 50)
                            {
                                firstName = firstName.Substring(0, 49);
                            }

                            lastName = source.sourceTextValue.Substring(firstSpace + 1);

                            if (lastName.Length > 50)
                            {
                                lastName = lastName.Substring(0, 49);
                            }

                        }
                        else
                        {
                            firstName = source.sourceTextValue;
                        }

                        nextOfKin.first_name = firstName;
                        nextOfKin.last_name = lastName;
                        break;

                    case "nextOfKin.relationshipId":
                        nextOfKin.sims_er_relationship_id = source.sourceIntValue;
                        break;

                    case "nextOfKin.phoneNumber":
                        nextOfKin.phone_number = source.sourceTextValue;
                        break;
                }
            }

        }

        private void setPatientAdmissionContactSourceValue(ref rMemberAdmissionContacts contact, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "hospitalContacts.fullName":
                        string firstName = "";
                        string lastName = "";

                        int firstSpace = source.sourceTextValue.IndexOf(" ");

                        if (firstSpace > 0)
                        {
                            firstName = source.sourceTextValue.Substring(0, firstSpace);

                            if (firstName.Length > 50)
                            {
                                firstName = firstName.Substring(0, 49);
                            }

                            lastName = source.sourceTextValue.Substring(firstSpace + 1);

                            if (lastName.Length > 50)
                            {
                                lastName = lastName.Substring(0, 49);
                            }

                        } else
                        {
                            firstName = source.sourceTextValue;
                        }

                        contact.first_name = firstName;
                        contact.last_name = lastName;
                        break;

                    case "hospitalContacts.phoneNumber":
                        contact.phone1 = source.sourceTextValue;
                        break;
                }
            }

        }


        private void setInpatientAdmissionSourceValue(ref HospitalInpatientAdmission admission, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "displayAdmitDate":
                        admission.registered_date = source.sourceDateValue;
                        break;

                }
            }

        }

        private void setAdmissionReasonFormVisitSourceValue(ref HospitalInpatientAdmissionReasonForVisit reasonForVisit, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "reasonForVisit":
                        reasonForVisit.reason_for_visit = source.sourceTextValue;
                        break;
                }
            }

        }

        private void setAdmissionSourceSourceValue(ref HospitalInpatientAdmissionSource admitSource, List<InpatientChartSource> sources)
        {

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "admissionSourceId":
                        admitSource.hosptial_admission_source_id = source.sourceIntValue;
                        break;
                }
            }

        }



        private bool setVitalSignSourceValue(ref MemberVitals vital, List<InpatientChartSource> sources)
        {

            bool hasNote = false;

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "temperature":
                        vital.temperature_in_fahrenheit = source.sourceDecimalValue;                       
                        break;
                    case "temperatureSiteId":
                        vital.hospital_temperature_site_id = source.sourceIntValue;
                        break;
                    case "temperatureManagement":
                        vital.temperature_management = source.sourceTextValue;
                        break;
                    case "heartRate":
                        vital.heart_rate = source.sourceTextValue;
                        break;
                    case "heartRhythm":
                        vital.hospital_pulse_rhythm_id = source.sourceIntValue;
                        break;
                    case "systolicBloodPressure":
                        vital.seated_blood_pressure_systolic = source.sourceIntValue;
                        break;
                    case "diastolicBloodPressure":
                        vital.seated_blood_pressure_diastolic = source.sourceIntValue;
                        break;
                    case "meanArterialPressure":
                        vital.mean_arterial_pressure = source.sourceIntValue;
                        break;
                    case "nonInvasiveBloodPressure":
                        vital.non_invasive_blood_pressure = source.sourceTextValue;
                        break;
                    case "respirationRate":
                        vital.respiration_per_minute = source.sourceIntValue;
                        break;
                    case "fi02":
                        vital.fi02 = source.sourceIntValue;
                        break;
                    case "sp02":
                        vital.sp02 = source.sourceTextValue;
                        break;
                    case "etco02":
                        vital.etc02 = source.sourceTextValue;
                        break;
                }

                if (!hasNote)
                { 
                    hasNote = sourceHasNote(source);
                }
            }

            return hasNote;
        }


        private bool setCardiacAssessmentSourceValue(ref HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment, List<InpatientChartSource> sources)
        {

            bool hasNote = false;

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "cardiacOutput":
                        assessment.cardiac_output = source.sourceTextValue;
                        break;
                    case "cardiacIndex":
                        assessment.cardiac_index = source.sourceTextValue;
                        break;
                    case "cardiacStrokeVolume":
                        assessment.stroke_volume = source.sourceTextValue;
                        break;
                    case "cardiacStrokeVolumeIndex":
                        assessment.stroke_volume_index = source.sourceTextValue;
                        break;
                    case "cardiacNailClubbing":
                        assessment.nail_clubbing = source.sourceBoolValue;
                        break;
                    case "cardiacEdemaSymptoms":
                        assessment.edema_symptoms = source.sourceBoolValue;
                        break;
                    case "cardiacPulsesNormal":
                        assessment.pulses_normal = source.sourceBoolValue;
                        break;
                    case "cardiacAorticSoundNormal":
                        assessment.aortic_sound_normal = source.sourceBoolValue;
                        break;
                    case "cardiacPulmonicSoundNormal":
                        assessment.pulmonic_sound_normal = source.sourceBoolValue;
                        break;
                    case "cardiacErbsPointSoundNormal":
                        assessment.erb_point_sound_normal = source.sourceBoolValue;
                        break;
                    case "cardiacTricuspidSoundNormal":
                        assessment.tricuspid_sound_normal = source.sourceBoolValue;
                        break;
                    case "cardiacApicalPulseSoundNormal":
                        assessment.apical_pulse_sound_normal = source.sourceBoolValue;
                        break;                    
                }

                assessment.highlight = source.highlightSource;

                if ((bool)assessment.highlight)
                {
                    assessment.highlight_color = source.highlightColor;
                }

                if (!hasNote)
                {
                    hasNote = sourceHasNote(source);
                }
            }

            return hasNote;
        }


        private bool setNeurologicalAssessmentSourceValue(ref HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment, List<InpatientChartSource> sources)
        {

            bool hasNote = false;

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "neurologyHandSqueezeStrengthId":
                        assessment.hand_squeeze_strength_id = source.sourceIntValue;
                        break;

                    //case "cardiacIndex":
                    //    assessment.cardiac_index = source.sourceTextValue;
                    //    break;
                    //case "cardiacApicalPulseSoundNormal":
                    //    assessment.apical_pulse_sound_normal = source.sourceBoolValue;
                    //    break;
                }

                assessment.highlight = source.highlightSource;

                if ((bool)assessment.highlight)
                {
                    assessment.highlight_color = source.highlightColor;
                }

                if (!hasNote)
                {
                    hasNote = sourceHasNote(source);
                }

            }

            return hasNote;
        }

        private bool setBloodGasAssessmentSourceValue(ref HospitalInpatientAdmissionBloodGas assessment, List<InpatientChartSource> sources)
        {

            bool hasNote = false;

            foreach (InpatientChartSource source in sources)
            {
                switch (source.modelVariableName)
                {
                    case "bloodGasPh":
                        assessment.ph = source.sourceTextValue;
                        break;
                    case "bloodGasPc02":
                        assessment.pco2 = source.sourceTextValue;
                        break;
                    case "bloodGasP02":
                        assessment.po2 = source.sourceTextValue;
                        break;
                    case "bloodGasHc03":
                        assessment.hco3 = source.sourceTextValue;
                        break;
                    case "bloodGasBaseExcess":
                        assessment.base_excess= source.sourceTextValue;
                        break;
                    case "bloodGasHb":
                        assessment.hb = source.sourceTextValue;
                        break;
                    case "bloodGas02Saturation":
                        assessment.o2_saturation = source.sourceTextValue;
                        break;
                    case "bloodGasSodium":
                        assessment.sodium = source.sourceTextValue;
                        break;
                    case "bloodGasPotassium":
                        assessment.potassium = source.sourceTextValue;
                        break;
                    case "bloodGasCalcium":
                        assessment.calcium = source.sourceTextValue;
                        break;
                    case "bloodGasBloodSugar":
                        assessment.blood_sugar = source.sourceTextValue;
                        break;
                    case "bloodGasLactate":
                        assessment.lactate = source.sourceTextValue;
                        break;
                }


                if (!hasNote)
                {
                    hasNote = sourceHasNote(source);
                }
            }

            return hasNote;
        }
        



        private bool sourceHasNote(InpatientChartSource source)
        {

            if (source.mdtNote != null 
                && !string.IsNullOrEmpty(source.mdtNote.noteText))
            {
                return true;
            }

            return false;
        }


        private bool addChartMdtNotes(ref InpatientChart chart)
        {
            bool added = false;

            foreach (InpatientChartSource source in chart.sources)
            {
                
                if (source.mdtNote != null
                    && !string.IsNullOrEmpty(source.mdtNote.noteText)
                    && source.mdtNote.admissionId > 0)
                {

                    List<Note> dbMdtNote = updateAdmissionNotesMdt(source.mdtNote, false);
                    
                    if (dbMdtNote != null)
                    {
                        source.mdtNote.noteId = dbMdtNote[0].noteId;

                        if (!added)
                        {
                            added = true;
                        }
                    }
                }
            }

            return added;
        }

        private void addVitalSignSourceMdtNoteReferences(InpatientChart chart, MemberVitals dbVital)
        {
            if (dbVital.member_vitals_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {

                    if (source.mdtNote != null 
                        && source.mdtNote.noteId > 0
                        && chart.sourceId > 0)
                    {

                        HospitalInpatientAdmissionMdtNoteVitalSignReference highlightReference = null;


                        highlightReference = (
                                from hghRef in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                                where hghRef.member_vitals_id.Equals(dbVital.member_vitals_id)
                                && hghRef.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                && hghRef.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                select hghRef
                            )
                            .FirstOrDefault();

                        if (highlightReference == null)
                        {

                            highlightReference = new HospitalInpatientAdmissionMdtNoteVitalSignReference();

                            highlightReference.hospital_inpatient_admission_id = (int)chart.admissionId;
                            highlightReference.member_vitals_id = dbVital.member_vitals_id;
                            highlightReference.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                            highlightReference.hosptial_inpatient_admission_mdt_note = source.mdtNote.noteId;
                            highlightReference.creation_user_id = chart.usr;
                            highlightReference.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences.Add(highlightReference);
                            _icmsContext.SaveChanges();

                        } else
                        {

                            highlightReference.hosptial_inpatient_admission_mdt_note = source.mdtNote.noteId;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences.Update(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void addVitalSignSourceHighlightReference(InpatientChart chart, MemberVitals dbVital)
        {

            if (dbVital.member_vitals_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {
                    
                    HospitalInpatientAdmissionMdtNoteVitalSignReference highlightReference = null;

                    if (chart.sourceId > 0)
                    {

                        highlightReference = (
                                from hghRef in _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences
                                where  hghRef.member_vitals_id.Equals(dbVital.member_vitals_id)
                                && hghRef.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                && hghRef.hospital_inpatient_admission_id.Equals(chart.admissionId)    
                                select hghRef
                            )
                            .FirstOrDefault();


                        if (highlightReference != null)
                        {
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences.Update(highlightReference);
                            _icmsContext.SaveChanges();
                        } else
                        {
                            highlightReference = new HospitalInpatientAdmissionMdtNoteVitalSignReference();

                            highlightReference.hospital_inpatient_admission_id = (int)chart.admissionId;
                            highlightReference.member_vitals_id = dbVital.member_vitals_id;
                            highlightReference.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;
                            highlightReference.creation_user_id = chart.usr;
                            highlightReference.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteVitalSignReferences.Add(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void addCardiacAssessmentSourceMdtNoteReferences(InpatientChart chart, HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment)
        {
            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {
                    if (source.mdtNote != null
                        && source.mdtNote.noteId > 0
                        && source.sourceId > 0)
                    {

                        HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference noteCardiacRef = new HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference();

                        noteCardiacRef.hospital_inpatient_admission_id = (int)chart.admissionId;
                        noteCardiacRef.hosptial_inpatient_admission_mdt_note = source.mdtNote.noteId;
                        noteCardiacRef.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id;
                        noteCardiacRef.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                        noteCardiacRef.creation_user_id = chart.usr;
                        noteCardiacRef.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                        _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences.Add(noteCardiacRef);
                        _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addCardiacSourceHighlightReference(InpatientChart chart, HospitalInpatientAdmissionNursingProcessAssessmentBasicCardiac assessment)
        {

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {

                    HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference highlightReference = null;

                    if (chart.sourceId > 0)
                    {

                        highlightReference = (
                                from hghRef in _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences
                                where hghRef.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id)
                                && hghRef.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                && hghRef.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                select hghRef
                            )
                            .FirstOrDefault();


                        if (highlightReference != null)
                        {
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences.Update(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                        else
                        {
                            highlightReference = new HospitalInpatientAdmissionMdtNoteCardiacAssessmentReference();

                            highlightReference.hospital_inpatient_admission_id = (int)chart.admissionId;
                            highlightReference.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_cardiac_id;
                            highlightReference.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;
                            highlightReference.creation_user_id = chart.usr;
                            highlightReference.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteCardiacAssessmentReferences.Add(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void addNeurologicalAssessmentSourceMdtNoteReferences(InpatientChart chart, HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment)
        {
            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {
                    if (source.mdtNote != null
                        && source.mdtNote.noteId > 0
                        && source.sourceId > 0)
                    {

                        HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference noteNeuroRef = new HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference();

                        noteNeuroRef.hospital_inpatient_admission_id = (int)chart.admissionId;
                        noteNeuroRef.hosptial_inpatient_admission_mdt_note = source.mdtNote.noteId;
                        noteNeuroRef.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id;
                        noteNeuroRef.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                        noteNeuroRef.creation_user_id = chart.usr;
                        noteNeuroRef.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                        _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences.Add(noteNeuroRef);
                        _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addNeurologicalSourceHighlightReference(InpatientChart chart, HospitalInpatientAdmissionNursingProcessAssessmentBasicNeurological assessment)
        {

            if (assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {

                    HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference highlightReference = null;

                    if (chart.sourceId > 0)
                    {

                        highlightReference = (
                                from hghRef in _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences
                                where hghRef.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id.Equals(assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id)
                                && hghRef.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                && hghRef.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                select hghRef
                            )
                            .FirstOrDefault();


                        if (highlightReference != null)
                        {
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences.Update(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                        else
                        {

                            highlightReference = new HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReference();

                            highlightReference.hospital_inpatient_admission_id = (int)chart.admissionId;
                            highlightReference.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id = assessment.hospital_inpatient_admission_nursing_process_assessment_basic_neurological_id;
                            highlightReference.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;
                            highlightReference.creation_user_id = chart.usr;
                            highlightReference.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteNeurologicalAssessmentReferences.Add(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void addBloodGasAssessmentSourceMdtNoteReferences(InpatientChart chart, HospitalInpatientAdmissionBloodGas assessment)
        {
            if (assessment.hosptial_inpatient_admission_blood_gas_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {
                    if (source.mdtNote != null
                        && source.mdtNote.noteId > 0
                        && (source.sourceId > 0 || chart.sourceId > 0))
                    {

                        HospitalInpatientAdmissionMdtNoteBloodGasReference bloodGasRef = new HospitalInpatientAdmissionMdtNoteBloodGasReference();

                        bloodGasRef.hospital_inpatient_admission_id = (int)chart.admissionId;
                        bloodGasRef.hosptial_inpatient_admission_mdt_note = source.mdtNote.noteId;
                        bloodGasRef.hosptial_inpatient_admission_blood_gas_id = assessment.hosptial_inpatient_admission_blood_gas_id;
                        bloodGasRef.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                        bloodGasRef.creation_user_id = chart.usr;
                        bloodGasRef.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                        _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences.Add(bloodGasRef);
                        _icmsContext.SaveChanges();
                    }
                }
            }
        }

        private void addBloodSourceHighlightReference(InpatientChart chart, HospitalInpatientAdmissionBloodGas assessment)
        {

            if (assessment.hosptial_inpatient_admission_blood_gas_id > 0
                && chart.admissionId > 0)
            {

                foreach (InpatientChartSource source in chart.sources)
                {

                    HospitalInpatientAdmissionMdtNoteBloodGasReference highlightReference = null;

                    if (chart.sourceId > 0)
                    {

                        highlightReference = (
                                from hghRef in _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences
                                where hghRef.hosptial_inpatient_admission_blood_gas_id.Equals(assessment.hosptial_inpatient_admission_blood_gas_id)
                                && hghRef.hospital_inpatient_admission_chart_source_id.Equals(chart.sourceId)
                                && hghRef.hospital_inpatient_admission_id.Equals(chart.admissionId)
                                select hghRef
                            )
                            .FirstOrDefault();


                        if (highlightReference != null)
                        {
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences.Update(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                        else
                        {

                            highlightReference = new HospitalInpatientAdmissionMdtNoteBloodGasReference();

                            highlightReference.hospital_inpatient_admission_id = (int)chart.admissionId;
                            highlightReference.hosptial_inpatient_admission_blood_gas_id = assessment.hosptial_inpatient_admission_blood_gas_id;
                            highlightReference.hospital_inpatient_admission_chart_source_id = (source.sourceId > 0) ? source.sourceId : chart.sourceId;
                            highlightReference.highlight = source.highlightSource;
                            highlightReference.highlight_color = source.highlightColor;
                            highlightReference.creation_user_id = chart.usr;
                            highlightReference.creation_date = (chart.creationDate != null) ? (DateTime)chart.creationDate : DateTime.Now;

                            _icmsContext.HospitalInpatientAdmissionMdtNoteBloodGasReferences.Add(highlightReference);
                            _icmsContext.SaveChanges();
                        }
                    }
                }
            }
        }
        





        public Patient insertHospitalInpatientAdmission(Patient patient)
        {
            Patient returnPatient = null;

            if (!patient.PatientId.Equals(Guid.Empty))
            {

                returnPatient = new Patient();
                returnPatient.PatientId = patient.PatientId;

                updatePatientAdmissionThroughCreateNewAdmission(patient, ref returnPatient);
            }

            return returnPatient;
        }

        private void updatePatientAdmissionThroughCreateNewAdmission(Patient patient, ref Patient returnPatient)
        {

            updatePatientAddressThroughCreateAdmission(patient, ref returnPatient);
            updatePatientInsuranceThroughCreateAdmission(patient, ref returnPatient);
            updatePatientAdvancedDirectivesThroughCreateAdmission(patient, ref returnPatient);

            returnPatient.currentAdmission = createPatientNormalAdmission(patient);

            if (returnPatient.currentAdmission != null)
            {              
                createPatientAdmissionReasonThroughCreateAdmission(patient, ref returnPatient);
                createPatientAllergyThroughCreateAdmission(patient, ref returnPatient);
                createPatientIsolationThroughCreateAdmission(patient, ref returnPatient);
                createPatientPossessionThroughCreateAdmission(patient, ref returnPatient);
            }            
        }

        private Admission createPatientNormalAdmission(Patient patient)
        {
            Admission createdAdmission = null;

            DateTime now = DateTime.Now;

            HospitalInpatientAdmission newAdmit = new HospitalInpatientAdmission();
            newAdmit.member_id = (Guid)patient.PatientId;
            newAdmit.hospital_id = patient.currentAdmission.hospital_id;
            newAdmit.hospital_department_id = patient.currentAdmission.hospital_department_id;
            newAdmit.registered_date = now;
            newAdmit.registration_number = generateRegistrationNumber();
            
            newAdmit.creation_date = now;
            newAdmit.creation_user_id = patient.usr;

            _icmsContext.HospitalInpatientAdmissions.Add(newAdmit);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                createdAdmission = new Admission();
                createdAdmission.member_id = newAdmit.member_id;
                createdAdmission.hospital_inpatient_admission_id = newAdmit.hospital_inpatient_admission_id;
                createdAdmission.registration_number = newAdmit.registration_number;
                createdAdmission.hospital_id = newAdmit.hospital_id;
                createdAdmission.admitDate = newAdmit.registered_date;
                createdAdmission.hospital_department_id = newAdmit.hospital_department_id;
            }

            return createdAdmission;
        }

        private void updatePatientAddressThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            Guid patientId = (Guid)returnPatient.PatientId;
            int results = 0;

            MemberAddress dbAddress = (
                                        from memAddr in _icmsContext.MemberAddresses
                                        where memAddr.member_id.Equals(patientId)
                                        && (memAddr.is_alternate.Equals(false) || memAddr.is_alternate.Equals(null))
                                        orderby memAddr.member_address_id descending
                                        select memAddr
                                      )
                                      .Take(1)
                                      .FirstOrDefault();

            if (dbAddress != null)
            {
                dbAddress.address_line1 = patient.addresses.First().address_line1;
                dbAddress.address_line2 = patient.addresses.First().address_line2;
                dbAddress.city = patient.addresses.First().city;
                dbAddress.state_abbrev = patient.addresses.First().state_abbrev;
                dbAddress.zip_code = patient.addresses.First().zip_code;

                _icmsContext.MemberAddresses.Update(dbAddress);
                results = _icmsContext.SaveChanges();

                if (results > 0)
                {

                    List<MemberAddress> addresses = new List<MemberAddress>();
                    addresses.Add(dbAddress);

                    returnPatient.addresses = addresses;
                }
            }
            else
            {
                MemberAddress newAddress = new MemberAddress();
                newAddress.member_id = patientId;
                newAddress.address_line1 = patient.addresses.First().address_line1;
                newAddress.address_line2 = patient.addresses.First().address_line2;
                newAddress.city = patient.addresses.First().city;
                newAddress.state_abbrev = patient.addresses.First().state_abbrev;
                newAddress.zip_code = patient.addresses.First().zip_code;

                _icmsContext.MemberAddresses.Add(newAddress);
                results = _icmsContext.SaveChanges();

                if (results > 0)
                {

                    List<MemberAddress> addresses = new List<MemberAddress>();
                    addresses.Add(newAddress);

                    returnPatient.addresses = addresses;
                }
            }
        }

        private void updatePatientInsuranceThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            Guid patientId = (Guid)returnPatient.PatientId;
            int results = 0;

            MemberInsurance dbInsurance = (
                                            from memInsr in _icmsContext.MemberInsurances
                                            where memInsr.member_id.Equals(patientId)
                                            orderby memInsr.member_insurance_id descending
                                            select memInsr
                                          )
                                          .Take(1)
                                          .FirstOrDefault();

            if (dbInsurance != null)
            {
                dbInsurance.insurance_group_number = patient.insuranceGroupNumber;
                dbInsurance.insurance_id = patient.insuranceMemberId;
                dbInsurance.insurance_plan_number = patient.insurancePlanNumber;
                dbInsurance.subscriber_first_name = patient.insuranceSubscriberFirstName;
                dbInsurance.subscriber_last_name = patient.insuranceSubscriberLastName;
                dbInsurance.insurance_name = patient.InsuranceName;
                dbInsurance.last_update_date = DateTime.Now;
                dbInsurance.last_update_user_id = patient.usr;

                _icmsContext.MemberInsurances.Update(dbInsurance);
                results = _icmsContext.SaveChanges();

                if (results > 0)
                {
                    returnPatient.insuranceId = dbInsurance.member_insurance_id;
                    returnPatient.insuranceGroupNumber = dbInsurance.insurance_group_number;
                    returnPatient.insuranceMemberId = dbInsurance.insurance_id;
                    returnPatient.insurancePlanNumber = dbInsurance.insurance_plan_number;
                    returnPatient.insuranceSubscriberFirstName = dbInsurance.subscriber_first_name;
                    returnPatient.insuranceSubscriberLastName = dbInsurance.subscriber_last_name;
                    returnPatient.InsuranceName = dbInsurance.insurance_name;
                }
            }
            else
            {
                MemberInsurance newInsurance = new MemberInsurance();
                newInsurance.member_id = patientId;
                newInsurance.insurance_group_number = patient.insuranceGroupNumber;
                newInsurance.insurance_id = patient.insuranceMemberId;
                newInsurance.insurance_plan_number = patient.insurancePlanNumber;
                newInsurance.subscriber_first_name = patient.insuranceSubscriberFirstName;
                newInsurance.subscriber_last_name= patient.insuranceSubscriberLastName;
                newInsurance.insurance_name = patient.InsuranceName;
                newInsurance.creation_date = DateTime.Now;
                newInsurance.creation_user_id = patient.usr;

                _icmsContext.MemberInsurances.Add(newInsurance);
                results = _icmsContext.SaveChanges();

                if (results > 0)
                {
                    returnPatient.insuranceId = newInsurance.member_insurance_id;
                    returnPatient.insuranceGroupNumber = newInsurance.insurance_group_number;
                    returnPatient.insuranceMemberId = newInsurance.insurance_id;
                    returnPatient.insurancePlanNumber = newInsurance.insurance_plan_number;
                    returnPatient.insuranceSubscriberFirstName = newInsurance.subscriber_first_name;
                    returnPatient.insuranceSubscriberLastName = newInsurance.subscriber_last_name;
                    returnPatient.InsuranceName = newInsurance.insurance_name;
                }
            }
        }

        private void updatePatientAdvancedDirectivesThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            Guid patientId = (Guid)returnPatient.PatientId;
            int results = 0;

            tblAdvanceDirectives dbAdvDirects = (
                                                    from memAdvDirect in _emrContext.AdvancedDirectives
                                                    where memAdvDirect.member_id.Equals(patientId)
                                                    orderby memAdvDirect.entryId descending
                                                    select memAdvDirect
                                                  )
                                                  .Take(1)
                                                  .FirstOrDefault();

            if (dbAdvDirects != null)
            {
                dbAdvDirects.DNR = patient.dnr.DoNotResuscitatePatient;
                dbAdvDirects.last_update_date = DateTime.Now;
                dbAdvDirects.last_update_user_id = patient.usr;

                _emrContext.AdvancedDirectives.Update(dbAdvDirects);
                results = _emrContext.SaveChanges();

                if (results > 0)
                {
                    returnPatient.dnr = new AdvancedDirective();
                    returnPatient.dnr.advanceDirectiveId = dbAdvDirects.entryId;
                    returnPatient.dnr.DoNotResuscitatePatient = dbAdvDirects.DNR;
                }
            }
            else
            {
                tblAdvanceDirectives newAdvDirective = new tblAdvanceDirectives();
                newAdvDirective.member_id = patientId;
                newAdvDirective.DNR = patient.dnr.DoNotResuscitatePatient;
                newAdvDirective.creation_date = DateTime.Now;
                newAdvDirective.creation_user_id = patient.usr;

                _emrContext.AdvancedDirectives.Add(newAdvDirective);
                results = _emrContext.SaveChanges();

                if (results > 0)
                {
                    returnPatient.dnr = new AdvancedDirective();
                    returnPatient.dnr.advanceDirectiveId = newAdvDirective.entryId;
                    returnPatient.dnr.DoNotResuscitatePatient = newAdvDirective.DNR;
                }
            }
        }


        private void createPatientAdmissionReasonThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            if (returnPatient.currentAdmission.hospital_inpatient_admission_id > 0)
            {
                HospitalInpatientAdmissionReasonForVisit addReasonForVisit = new HospitalInpatientAdmissionReasonForVisit();
                addReasonForVisit.hospital_inpatient_admission_id = returnPatient.currentAdmission.hospital_inpatient_admission_id;
                addReasonForVisit.reason_for_visit = patient.currentAdmission.reasonForVisit;
                addReasonForVisit.creation_date = DateTime.Now;
                addReasonForVisit.creation_user_id = patient.usr;

                _icmsContext.HospitalInpatientAdmissionReasonForVisits.Add(addReasonForVisit);
                int result = _icmsContext.SaveChanges();

                if(result > 0)
                {
                    returnPatient.currentAdmission.reasonForVisit = addReasonForVisit.reason_for_visit;
                }
            }
        }

        private void createPatientAllergyThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            if (returnPatient.currentAdmission.hospital_inpatient_admission_id > 0)
            {
                HospitalInpatientAdmissionAllergies addAllergy = new HospitalInpatientAdmissionAllergies();
                addAllergy.hospital_inpatient_admission_id = returnPatient.currentAdmission.hospital_inpatient_admission_id;
                addAllergy.medication_allergy = patient.allergies.medicationAllergy;
                addAllergy.other_allergy = patient.allergies.otherAllergies;
                addAllergy.latex_allergy = patient.allergies.latexAllergy;
                addAllergy.creation_date = DateTime.Now;
                addAllergy.creation_user_id = patient.usr;

                _icmsContext.HospitalInpatientAdmissionAllergys.Add(addAllergy);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    Allergy admitAllergy = new Allergy();
                    admitAllergy.allergyId = addAllergy.hospital_inpatient_admission_allergies_id;
                    admitAllergy.admissionId = (int)addAllergy.hospital_inpatient_admission_id;
                    admitAllergy.medicationAllergy = addAllergy.medication_allergy;
                    admitAllergy.otherAllergies = addAllergy.other_allergy;
                    admitAllergy.latexAllergy = (bool)addAllergy.latex_allergy;

                    returnPatient.allergies = admitAllergy;
                }
            }            
        }

        private void createPatientIsolationThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            if (returnPatient.currentAdmission.hospital_inpatient_admission_id > 0)
            {
                HospitalInpatientAdmissionIsolation addIsolation = new HospitalInpatientAdmissionIsolation();
                addIsolation.hosptial_inpatient_admission_id = returnPatient.currentAdmission.hospital_inpatient_admission_id;
                addIsolation.isolate_patient = patient.isolation.isolateMe;
                addIsolation.infection_status = patient.isolation.infectionStatus;
                addIsolation.creation_date = DateTime.Now;
                addIsolation.creation_user_id = patient.usr;

                _icmsContext.HospitalInpatientAdmissionIsolations.Add(addIsolation);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    Isolation admitIsolation = new Isolation();
                    admitIsolation.isolationId = addIsolation.hosptial_inpatient_admission_isolation_id;
                    admitIsolation.admissionId = addIsolation.hosptial_inpatient_admission_id;
                    admitIsolation.isolateMe = addIsolation.isolate_patient;
                    admitIsolation.infectionStatus = addIsolation.infection_status;

                    returnPatient.isolation = admitIsolation;
                }
            }
        }

        private void createPatientPossessionThroughCreateAdmission(Patient patient, ref Patient returnPatient)
        {
            if (returnPatient.currentAdmission.hospital_inpatient_admission_id > 0)
            {
                HospitalInpatientAdmissionSafety addIsolation = new HospitalInpatientAdmissionSafety();
                addIsolation.hospital_inpatient_admission_id = returnPatient.currentAdmission.hospital_inpatient_admission_id;
                addIsolation.has_contact_lenses = patient.admissionValuables.hasContactLenses;
                addIsolation.has_dentures_lower = patient.admissionValuables.hasDenturesLower;
                addIsolation.has_dentures_upper = patient.admissionValuables.hasDenturesUpper;
                addIsolation.has_glasses = patient.admissionValuables.hasGlasses;
                addIsolation.has_hearing_aid_left = patient.admissionValuables.hasHearingAidLeft;
                addIsolation.has_hearing_aid_right = patient.admissionValuables.hasHearingAidRight;
                addIsolation.has_walking_aid = patient.admissionValuables.hasWalkingAid;
                addIsolation.valuables_nill = patient.admissionValuables.noValuables;
                addIsolation.valuables_sent_home = patient.admissionValuables.valuablesSentHome;
                addIsolation.valuable_shospital_safe = patient.admissionValuables.valuablesInHospitalSafe;
                addIsolation.property_list = patient.admissionValuables.valuablesList;
                addIsolation.walking_aid = patient.admissionValuables.walkingAid;
                addIsolation.creation_date = DateTime.Now;
                addIsolation.creation_user_id = patient.usr;

                _icmsContext.HospitalInpatientAdmissionSafeties.Add(addIsolation);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    PatientValuables admitValuable = new PatientValuables();
                    admitValuable.safetyId = addIsolation.hospital_inpatient_admission_safety_id;
                    admitValuable.admissionId = addIsolation.hospital_inpatient_admission_id;
                    admitValuable.hasContactLenses = addIsolation.has_contact_lenses;
                    admitValuable.hasDenturesLower = addIsolation.has_dentures_lower;
                    admitValuable.hasDenturesUpper = addIsolation.has_dentures_upper;
                    admitValuable.hasGlasses = addIsolation.has_glasses;
                    admitValuable.hasHearingAidLeft = addIsolation.has_hearing_aid_left;
                    admitValuable.hasHearingAidRight = addIsolation.has_hearing_aid_right;
                    admitValuable.hasWalkingAid = addIsolation.has_walking_aid;
                    admitValuable.noValuables = addIsolation.valuables_nill;
                    admitValuable.valuablesSentHome = addIsolation.valuables_sent_home;
                    admitValuable.valuablesInHospitalSafe = addIsolation.valuable_shospital_safe;
                    admitValuable.valuablesList = addIsolation.property_list;
                    admitValuable.walkingAid = addIsolation.walking_aid;

                    returnPatient.admissionValuables = admitValuable;
                }
            }
        }





        public bool disableFacility(HospitalFacility facility)
        {

            if (facility.hospitalId > 0)
            {

                rDepartment dbFacility = (

                        from fac in _icmsContext.rDepartments
                        where fac.id.Equals(facility.hospitalId)
                        select fac
                    )
                    .FirstOrDefault();

                if (dbFacility != null)
                {

                    dbFacility.disable_flag = true;

                    if (!facility.creationDate.Equals(DateTime.MinValue)) dbFacility.date_updated = facility.creationDate;
                    if (!facility.usr.Equals(Guid.Empty)) dbFacility.user_updated = facility.usr;

                    _icmsContext.rDepartments.Update(dbFacility);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }


}

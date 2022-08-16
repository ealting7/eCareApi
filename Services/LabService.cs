using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class LabService : ILab
    {
        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _datastagingContext;

        public LabService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext datastagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _datastagingContext = datastagingContext ?? throw new ArgumentNullException(nameof(datastagingContext));
        }


        public List<Lab> getAllLabTypes()
        {
            List<Lab> labTypes = null;

            labTypes = (from labtyps in _icmsContext.LabTypes
                        where (labtyps.disabled.Equals(0) || labtyps.disabled == null)
                        orderby labtyps.test_name
                        select new Lab
                        {
                            labId = labtyps.hospital_order_test_id,
                            labName = labtyps.test_name
                        })
                           .ToList();

            return labTypes;
        }

        public List<LabResult> getAllLabResultFlags()
        {
            List<LabResult> flags = null;

            flags = (
                        from labresultflags in _icmsContext.HospitalOrderResultFlags
                        orderby labresultflags.flag_name
                        select new LabResult
                        {
                            flagId = labresultflags.hospital_order_result_flag_id,
                            flagName = labresultflags.flag_name
                        }
                     )
                     .ToList();

            return flags;
        }

        public IEnumerable<Lab> GetHospitalLabTypes()
        {
            IEnumerable<Lab> labTypes = Enumerable.Empty<Lab>();

            labTypes = (from labtyps in _icmsContext.LabTypes
                          where labtyps.is_labq_test.Equals(true) 
                          && (labtyps.disabled.Equals(0) || labtyps.disabled == null)
                          orderby labtyps.test_name
                          select new Lab
                          {
                              labId = labtyps.hospital_order_test_id,
                              labName = labtyps.test_name
                          })
                           .ToList();

            return labTypes;
        }

        public Lab GetLabTypeTests(string id)
        {
            Lab labTests = new Lab();
            int hospOrdTstId = 0;

            if (int.TryParse(id, out hospOrdTstId))
            {
                IList<MedicalCode> labCpts = (from lbCpts in _icmsContext.LabTypeCpts
                                              join cpts in _icmsContext.CptCodes
                                              on lbCpts.cpt_codes_2015_id equals cpts.cpt_codes_2015_id
                                              where lbCpts.hospital_order_test_id.Equals(hospOrdTstId)
                                              select new MedicalCode {
                                              CodeType = "CPT",
                                              CodeId = lbCpts.cpt_codes_2015_id,
                                              Code = cpts.cpt_code,
                                              ShortDescription = cpts.short_descr,
                                              DisplayDescription = (!string.IsNullOrEmpty(cpts.cpt_descr) ? cpts.cpt_descr :
                                                                            (!string.IsNullOrEmpty(cpts.medium_descr)) ? cpts.medium_descr :
                                                                            cpts.short_descr)
                                              }).ToList();

                IList<MedicalCode> labHcpcs = (from lbHcpcs in _icmsContext.LabTypeHcpcs
                                               join hcpcs in _icmsContext.HcpcsCodes
                                               on lbHcpcs.hcpcs_codes_2015_id equals hcpcs.hcpcs_codes_2015_id
                                               where lbHcpcs.hospital_order_test_id.Equals(hospOrdTstId)
                                               select new MedicalCode
                                               {
                                                   CodeType="HCPCS",
                                                   CodeId = lbHcpcs.hcpcs_codes_2015_id,
                                                   Code = hcpcs.hcp_code,
                                                   ShortDescription = hcpcs.hcpcs_short,
                                                   DisplayDescription = (!string.IsNullOrEmpty(hcpcs.hcpcs_full) ? hcpcs.hcpcs_full : hcpcs.hcpcs_short)
                                               }).ToList();

                if (labCpts.Count > 0)
                {
                    labTests.labTypeCpts = labCpts;
                }

                if (labHcpcs.Count > 0)
                {
                    labTests.labTypeHcpcs = labHcpcs;
                }
            }

            return labTests;
        }

        public List<HospitalDepartment> GetHospitalCollectionDepartments(int hospId)
        {
            List<HospitalDepartment> hospDepts = new List<HospitalDepartment>();

            hospDepts = (from dept in _icmsContext.HospitalDepartments
                        where dept.hospital_id.Equals(hospId)
                        && dept.has_specimen_collection_capabilities.Equals(true)
                        select dept).ToList();

            return hospDepts;
        }

        public List<HospitalDepartmentAppointmentTypes> GetLabTypeAppointmentTypes(int labTypeId)
        {
            List<HospitalDepartmentAppointmentTypes> apptTypes = new List<HospitalDepartmentAppointmentTypes>();

            apptTypes = (from hospApptTypes in _icmsContext.HospitalDepartmentAppointmentTypes
                         where hospApptTypes.hospital_order_test_id.Equals(labTypeId)
                         orderby hospApptTypes.appointment_type_name
                         select hospApptTypes).ToList();

            return apptTypes;
        }

        public List<MedicalCode> GetLabTypeIcd10s(int labTypeId)
        {
            List<MedicalCode> icd10s = new List<MedicalCode>();

            icd10s = (from hospOrdTestDiag in _icmsContext.HospitalOrderTestDiagnosis
                      join diag in _icmsContext.DiagnosisCodes10
                      on hospOrdTestDiag.diagnosis_codes_10_id equals diag.diagnosis_codes_10_id
                      where hospOrdTestDiag.hospital_order_test_id.Equals(labTypeId)
                      orderby diag.diagnosis_code
                      select new MedicalCode
                      {
                          Code = diag.diagnosis_code,
                          CodeId = diag.diagnosis_codes_10_id,
                          CodeType = "ICD10",
                          ShortDescription = diag.short_description,
                          MediumDescription = diag.medium_description,
                          LongDescription = diag.long_description,
                          DisplayDescription = (!string.IsNullOrEmpty(diag.short_description) ? diag.short_description : 
                                                     (!string.IsNullOrEmpty(diag.medium_description) ? diag.medium_description : diag.long_description)
                                                )
                      }).ToList();

            return icd10s;
        }

        public List<Appointment> GetLabAvailableAppointments(Appointment lab, DateTime srchFromDate)
        {
            List<Appointment> appts = new List<Appointment>();            


            if (!srchFromDate.Equals(DateTime.MinValue))
            {
                int labDeptId = lab.searchCollectionDepartmentId;


                if (labDeptId > 0)
                {
                    List <HospitalDepartmentRooms> deptRooms = getDepartmentRoomList(labDeptId);
                    HospitalDepartmentWorkday deptWrkDay = getDepartmentWorkday(labDeptId);
                    AppointmentType selectedApptType = getLabTypeAppointmentTypes(lab.searchAppointmentTypeId);

                    appts = (from deptAppts in _icmsContext.HospitalAppointmentSchedules

                             join hosp in _icmsContext.Hospitals
                             on deptAppts.hospital_id equals hosp.hospital_id

                             join dept in _icmsContext.HospitalDepartments
                             on deptAppts.hospital_department_id equals dept.hospital_department_id

                             where deptAppts.hospital_id.Equals(lab.searchCollectionFacilityId)
                             && deptAppts.hospital_department_id.Equals(labDeptId)
                             && (deptAppts.appointment_start_date >= srchFromDate && 
                                 deptAppts.appointment_start_date <= srchFromDate.AddDays(14))
                             select new Appointment
                             {
                                 HospitalName = hosp.name,
                                 HospitalId = deptAppts.hospital_id,
                                 DepartmentName = dept.hospital_department_name,
                                 DepartmentId = deptAppts.hospital_department_id,
                                 appointmentStartDateTime = deptAppts.appointment_start_date,
                                 appointmentEndDateTime = deptAppts.appointment_end_date,
                                 returnSearchFromDate = srchFromDate,
                                 departmentRooms = deptRooms,
                                 departmentWorkday = deptWrkDay,
                                 selectedAppointmentType = selectedApptType
                             })
                             .ToList();



                    if (appts.Count.Equals(0))
                    {
                        appts = (from hospDept in _icmsContext.HospitalDepartments
                                 join hosp in _icmsContext.Hospitals
                                 on hospDept.hospital_id equals hosp.hospital_id
                                 where hospDept.hospital_department_id.Equals(labDeptId)
                                 select new Appointment
                                 {
                                     HospitalName = hosp.name,
                                     HospitalId = hosp.hospital_id,
                                     DepartmentName = hospDept.hospital_department_name,
                                     DepartmentId = hospDept.hospital_department_id,
                                     returnSearchFromDate = srchFromDate,
                                     departmentRooms = deptRooms,
                                     departmentWorkday = deptWrkDay,
                                     selectedAppointmentType = selectedApptType
                                 }).Take(1).ToList();
                    }
                }
            }

            //lab.searchCollectionFacilityId = parseInt(collectionFacilityId);
            //lab.searchLabTypeId = parseInt(labTypeId);
            //lab.searchCollectionDate = collectionDate;
            //lab.searchCollectionTime = collectionTime;
            //lab.searchDaysOut = srchDaysOut;
            //lab.searchType = "LabQ-SCHEDULEAPPOINTMENT-COLLECTION";

            //labTypes = (from labtyps in _icmsContext.LabTypes
            //            where labtyps.is_labq_test.Equals(true)
            //            && (labtyps.disabled.Equals(0) || labtyps.disabled == null)
            //            orderby labtyps.test_name
            //            select new Lab
            //            {
            //                labId = labtyps.hospital_order_test_id,
            //                labName = labtyps.test_name
            //            })
            //               .ToList();

            return appts;
        }

        public Appointment GetAppointmentScheduleData(Appointment lab)
        {
            Appointment apptData = new Appointment();

            if (!string.IsNullOrEmpty(lab.scheduleCollectionDate) && !string.IsNullOrEmpty(lab.scheduleCollectionTime))
            {
                DateTime collectionDateTime = Convert.ToDateTime(lab.scheduleCollectionDate + " " + lab.scheduleCollectionTime);
                lab.scheduleCollectionDateTime = collectionDateTime;

                DateTime collectionEndDateTime = Convert.ToDateTime(lab.scheduleCollectionEndDate + " " + lab.scheduleCollectionEndTime);
                lab.scheduleCollectionEndDateTime = collectionEndDateTime;

                lab.estimatedDeliveryDate = lab.scheduleCollectionDateTime.AddDays(21);
            }


            if (!string.IsNullOrEmpty(lab.schedulePatientId))
            {
                if (getPatient(lab, apptData))
                {
                    if (lab.scheduleCollectionFacilityId > 0 || lab.scheduleTestingFacilityId > 0)
                    {
                        getHospitalFacilities(lab, apptData);
                    }

                    if (lab.scheduleIcd10s.Count > 0 || lab.scheduleResultIcd10s.Count > 0)
                    {
                        getIcd10s(lab, apptData);
                    }

                    if (lab.schedulePcpAddId > 0)
                    {
                        getProvider(lab, apptData);
                    }

                    if (lab.scheduleAppointmentTypeId > 0)
                    {
                        getLabData(lab, apptData);
                    }
                }
            }


            return apptData;
        }

        public Admission CreateLabAppointment(Appointment lab)
        {
            Admission admit = new Admission();


            if (validateUser(lab))
            {
                if (!string.IsNullOrEmpty(lab.schedulePatientId))
                {
                    PatientService patServ = new PatientService(_icmsContext, _aspNetContext);
                    lab.patient = patServ.GetPatientsUsingId(lab.schedulePatientId);


                    if (!lab.patient.PatientId.Equals(Guid.Empty))
                    {
                        HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

                        admit = CreateAdmissionForLab(hospServ, lab);


                        if (!admit.member_id.Equals(Guid.Empty) &&
                            admit.hospital_id > 0 &&
                            admit.hospital_department_id > 0)
                        {
                            lab.patient = new Patient();
                            lab.patient.PatientId = admit.member_id;
                            
                            lab.HospitalId = admit.hospital_id;
                            lab.DepartmentId = admit.hospital_department_id;
                            lab.scheduleRoomId = admit.hospital_department_rooms_id;
                            lab.appointmentHospitalInpatientAdmissionId = admit.hospital_inpatient_admission_id;
                            lab.appointmentStartDateTime = lab.scheduleCollectionDateTime;
                            lab.appointmentEndDateTime = lab.scheduleCollectionEndDateTime;
                            lab.appointmentAppointmentTypeId = (lab.scheduleAppointmentTypeId > 0) ? lab.scheduleAppointmentTypeId : lab.appointmentAppointmentTypeId;
                            lab.estimatedDeliveryDate = lab.estimatedDeliveryDate;


                            admit.appointment = CreateAppointmentForLab(hospServ, lab);
                        }
                    }
                }
            }


            return admit;
        }

        public IEnumerable<Member> GetPatientWithLabs(string first, string last, string dob)
        {
            IEnumerable<Member> patients = Enumerable.Empty<Member>();

            DateTime memberBirth = DateTime.MinValue;


            if (!string.IsNullOrEmpty(dob) && DateTime.TryParse(dob, out memberBirth))
            {

                patients = (
                            from labOrders in _icmsContext.HospitalInpatientAdmissionOrderLabs

                            join orders in _icmsContext.HospitalInpatientAdmissionOrders
                            on labOrders.hospital_inpatient_admission_order_id equals orders.hospital_inpatient_admission_order_id

                            join inptadmits in _icmsContext.HospitalInpatientAdmissions
                            on orders.hospital_inpatient_admission_id equals inptadmits.hospital_inpatient_admission_id

                            join pat in _icmsContext.Patients
                            on inptadmits.member_id equals pat.member_id

                            where pat.member_first_name.StartsWith(first)
                                  && pat.member_last_name.StartsWith(last)
                                  && pat.member_birth.Equals(memberBirth)
                            select pat
                            )
                            .ToList()
                            .Take(50)
                            .Distinct();

            }
            else
            {
                patients = (
                            from labOrders in _icmsContext.HospitalInpatientAdmissionOrderLabs

                            join orders in _icmsContext.HospitalInpatientAdmissionOrders
                            on labOrders.hospital_inpatient_admission_order_id equals orders.hospital_inpatient_admission_order_id

                            join inptadmits in _icmsContext.HospitalInpatientAdmissions
                            on orders.hospital_inpatient_admission_id equals inptadmits.hospital_inpatient_admission_id

                            join pat in _icmsContext.Patients
                            on inptadmits.member_id equals pat.member_id

                            where pat.member_first_name.StartsWith(first)
                                  && pat.member_last_name.StartsWith(last)
                            select pat
                            )
                            .ToList()
                            .Take(50)
                            .Distinct();
            }


            return patients;
        }

        public List<Lab> GetPatientLabs(string id)
        {
            List<Lab> labs = null;

            Guid memberId = Guid.Empty;

            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out memberId))
            {

                labs = (
                            from labOrders in _icmsContext.HospitalInpatientAdmissionOrderLabs

                            join orders in _icmsContext.HospitalInpatientAdmissionOrders
                            on labOrders.hospital_inpatient_admission_order_id equals orders.hospital_inpatient_admission_order_id

                            join inptadmits in _icmsContext.HospitalInpatientAdmissions
                            on orders.hospital_inpatient_admission_id equals inptadmits.hospital_inpatient_admission_id

                            join tests in _icmsContext.HospitalOrderTests
                            on labOrders.hospital_order_test_id equals tests.hospital_order_test_id into labTest
                            from subtests in labTest.DefaultIfEmpty()

                            join hosp in _icmsContext.Hospitals
                            on inptadmits.hospital_id equals hosp.hospital_id

                            join pat in _icmsContext.Patients
                            on inptadmits.member_id equals pat.member_id

                            where inptadmits.member_id.Equals(memberId)
                            orderby labOrders.specimen_collection_date
                            select new Lab
                            {
                                labId = labOrders.hospital_inpatient_admission_order_lab_id,
                                labName = subtests.test_name,
                                accessionNumber = orders.accession_number,
                                collectionDate = labOrders.specimen_collection_date,
                                displayCollectionDate = (labOrders.specimen_collection_date.HasValue) ? labOrders.specimen_collection_date.Value.ToShortDateString() : "",
                                collectionSite = hosp.name
                            }
                        )
                        .Distinct()
                        .ToList();
            }


            return labs;
        }

        public Lab GetPatientLab(string id)
        {
            Lab lab = new Lab();

            int labId = 0;

            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out labId) && labId > 0)
            {

                lab = (
                        from labOrders in _icmsContext.HospitalInpatientAdmissionOrderLabs

                        join orders in _icmsContext.HospitalInpatientAdmissionOrders
                        on labOrders.hospital_inpatient_admission_order_id equals orders.hospital_inpatient_admission_order_id

                        join inptadmits in _icmsContext.HospitalInpatientAdmissions
                        on orders.hospital_inpatient_admission_id equals inptadmits.hospital_inpatient_admission_id

                        join tests in _icmsContext.HospitalOrderTests
                        on labOrders.hospital_order_test_id equals tests.hospital_order_test_id into labTest
                        from subtests in labTest.DefaultIfEmpty()

                        join hosp in _icmsContext.Hospitals
                        on inptadmits.hospital_id equals hosp.hospital_id

                        join pat in _icmsContext.Patients
                        on inptadmits.member_id equals pat.member_id

                        where labOrders.hospital_inpatient_admission_order_lab_id.Equals(labId)
                        select new Lab
                        {
                            labId = labOrders.hospital_inpatient_admission_order_lab_id,
                            labName = subtests.test_name,
                            accessionNumber = orders.accession_number,
                            collectionDate = labOrders.specimen_collection_date,
                            collectionSite = hosp.name,
                            testId = labOrders.hospital_order_test_id,
                            testName = subtests.test_name
                        })
                        .FirstOrDefault();

            }

            if (lab != null && lab.testId > 0)
            {
                List<MedicalCode> cpts = null;

                cpts = (
                            from cptTest in _icmsContext.HospitalOrderTestCpts

                            join cptCodes in _icmsContext.CptCodes
                            on cptTest.cpt_codes_2015_id equals cptCodes.cpt_codes_2015_id

                            where cptTest.hospital_order_test_id.Equals(lab.testId)
                            select new MedicalCode
                            {
                                CodeType = "CPT",
                                CodeId = cptTest.cpt_codes_2015_id,
                                Code = cptCodes.cpt_code,
                                DisplayDescription = (cptCodes.medium_descr != null) ? cptCodes.medium_descr : cptCodes.cpt_descr
                            }
                       )
                       .ToList();

                if (cpts != null)
                {
                    lab.labTypeCpts = cpts;
                }


                List<MedicalCode> hcpcs = null;

                hcpcs = (
                            from hcpcsTest in _icmsContext.HospitalOrderTestHcpcses

                            join hcpcsCodes in _icmsContext.HcpcsCodes
                            on hcpcsTest.hcpcs_codes_2015_id equals hcpcsCodes.hcpcs_codes_2015_id

                            where hcpcsTest.hospital_order_test_id.Equals(lab.testId)
                            select new MedicalCode
                            {
                                CodeType = "HCPCS",
                                CodeId = hcpcsTest.hcpcs_codes_2015_id,
                                Code = hcpcsCodes.hcp_code,
                                DisplayDescription = (hcpcsCodes.hcpcs_short != null) ? hcpcsCodes.hcpcs_short : hcpcsCodes.hcpcs_full
                            }
                       )
                       .ToList();

                if (hcpcs != null)
                {
                    lab.labTypeHcpcs = hcpcs;
                }
            }

            return lab;
        }

        public IEnumerable<Lab> GetTodaysLabs(string today, string hospitalId)
        {
            IEnumerable<Lab> labs = Enumerable.Empty<Lab>();

            DateTime todaysDte = DateTime.MinValue;
            int searchHospitalId = 0;


            if (!string.IsNullOrEmpty(today) && DateTime.TryParse(today, out todaysDte) &&
                !string.IsNullOrEmpty(hospitalId) & int.TryParse(hospitalId, out searchHospitalId) && searchHospitalId > 0)
            {

                labs = (
                        from labOrders in _icmsContext.HospitalInpatientAdmissionOrderLabs

                        join orders in _icmsContext.HospitalInpatientAdmissionOrders
                        on labOrders.hospital_inpatient_admission_order_id equals orders.hospital_inpatient_admission_order_id

                        join inptadmits in _icmsContext.HospitalInpatientAdmissions
                        on orders.hospital_inpatient_admission_id equals inptadmits.hospital_inpatient_admission_id

                        join tests in _icmsContext.HospitalOrderTests
                        on labOrders.hospital_order_test_id equals tests.hospital_order_test_id into labTest
                        from subtests in labTest.DefaultIfEmpty()

                        join hosp in _icmsContext.Hospitals
                        on inptadmits.hospital_id equals hosp.hospital_id

                        join pat in _icmsContext.Patients
                        on inptadmits.member_id equals pat.member_id

                        where labOrders.specimen_collection_date.Value >= todaysDte
                              && inptadmits.hospital_id.Equals(searchHospitalId)
                        select new Lab
                        {
                            labId = labOrders.hospital_inpatient_admission_order_lab_id,
                            patient = pat.member_first_name + ' ' + pat.member_last_name,
                            labName = subtests.test_name,
                            accessionNumber = orders.accession_number,
                            collectionDate = labOrders.specimen_collection_date,
                            collectionSite = hosp.name
                        })
                        
                        .ToList()
                        .OrderByDescending(collDte => collDte.collectionDate)
                        .Distinct();

            }


            return labs;
        }
        





        public DateTime getSearchFromDate(Appointment lab)
        {
            DateTime returnDate = DateTime.MinValue;

            DateTime startDate = Convert.ToDateTime(lab.searchCollectionDate + " " + lab.searchCollectionTime);


            if (lab.searchDaysOut > 0)
            {
                DateTime dateOut = startDate.AddDays(lab.searchDaysOut);


                if (dateOut > startDate)
                {

                    returnDate = dateOut;
                }
            }
            else
            {
                returnDate = startDate;
            }

            return returnDate;
        }

        public List<DateTime> getTwoWeeksOutDate(DateTime startDate)
        {
            List<DateTime> returnTwoWeekDate = new List<DateTime>();

            returnTwoWeekDate.Add(startDate);


            for (int i=1; i <= 14; i++)
            {
                DateTime addDate = startDate.AddDays(i);

                returnTwoWeekDate.Add(addDate);
            }


            return returnTwoWeekDate;
        }

        private int getDepartmentIdThatHasSpecimenCollectionCapabilities(Appointment lab)
        {
            int returnDeptId = 0;
            int? deptId = 0;
            
            deptId = (from dept in _icmsContext.HospitalDepartments
                      where dept.hospital_id.Equals(lab.searchCollectionFacilityId)
                      && dept.has_specimen_collection_capabilities.Equals(true)
                      select dept.hospital_department_id)
                      .SingleOrDefault();

            return returnDeptId;
        }

        private List<HospitalDepartmentRooms> getDepartmentRoomList(int deptId)
        {
            List<HospitalDepartmentRooms> rooms = new List<HospitalDepartmentRooms>();

            rooms = (from dept in _icmsContext.HospitalDepartments
                        join room in _icmsContext.HospitalDepartmentRooms
                        on dept.hospital_department_id equals room.hospital_department_id
                        where dept.hospital_department_id.Equals(deptId)
                        select new HospitalDepartmentRooms
                        {
                            hospital_department_rooms_id = room.hospital_department_rooms_id,
                            hospital_department_id= deptId,
                            name = room.name,
                            occupancy = room.occupancy,
                            description= room.description,
                            room_available=room.room_available
                        })
                        .ToList();

            return rooms;
        }

        private HospitalDepartmentWorkday getDepartmentWorkday(int deptId)
        {
            HospitalDepartmentWorkday wrkDay = new HospitalDepartmentWorkday();

            wrkDay = (from day in _icmsContext.HospitalDepartmentWorkdays
                     where day.hospital_department_id.Equals(deptId)
                     select day).FirstOrDefault();

            return wrkDay;
        }

        private AppointmentType getLabTypeAppointmentTypes(int apptTypeId)
        {
            AppointmentType apptType = new AppointmentType();

            apptType = (from deptApptTypes in _icmsContext.HospitalDepartmentAppointmentTypes
                        join apptTypeDur in _icmsContext.HospitalDepartmentAppointmentTypesDurationReferences
                        on deptApptTypes.hospital_department_appointment_types_id equals apptTypeDur.hospital_department_appointment_types_id
                        where deptApptTypes.hospital_department_appointment_types_id.Equals(apptTypeId)
                        select new AppointmentType {
                            appointmentTypeId = deptApptTypes.hospital_department_appointment_types_id,
                            appointmentTypeName = deptApptTypes.appointment_type_name,
                            duration = apptTypeDur.minimum_minutes
                        }).FirstOrDefault();

            return apptType;
        }

        private bool getPatient(Appointment lab, Appointment apptData)
        {
            bool loaded = false;

            PatientService patServ = new PatientService(_icmsContext, _aspNetContext);
            Patient patient = patServ.GetPatientsUsingId(lab.schedulePatientId);

            apptData.patient = patient;

            apptData.patient.addresses = patServ.GetPatientAddress(lab.schedulePatientId, true);
            apptData.patient.homePhoneNumber = patServ.GetPatientHomePhoneNumber(lab.schedulePatientId, true);

            Patient insurance = patServ.GetPatientInsuranceUsingId(lab.schedulePatientId);

            if (insurance != null)
            {
                apptData.patient.InsuranceName = insurance.InsuranceName;
                apptData.patient.selfPay = insurance.selfPay;
                apptData.patient.isMedicaid = insurance.isMedicaid;
                apptData.patient.isMedicare = insurance.isMedicare;
                apptData.patient.insuranceMemberId = insurance.insuranceMemberId;
                apptData.patient.insuranceSubscriberFirstName = insurance.insuranceSubscriberFirstName;
                apptData.patient.insuranceSubscriberLastName = insurance.insuranceSubscriberLastName;
                apptData.patient.insuranceRelationshipId = insurance.insuranceRelationshipId;
                apptData.patient.insuranceRelationshipToPatient = insurance.insuranceRelationshipToPatient;
            }

            if (!apptData.patient.PatientId.Equals(Guid.Empty))
            {
                loaded = true;
            }


            return loaded;
        }

        private void getHospitalFacilities(Appointment lab, Appointment apptData)
        {
            HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

            if (lab.scheduleCollectionFacilityId > 0)
            {
                Hospital hospital = hospServ.GetHospitalUsingId(lab.scheduleCollectionFacilityId);
                apptData.HospitalId = lab.scheduleCollectionFacilityId;
                apptData.HospitalName = hospital.name;
            }


            if (lab.scheduleTestingFacilityId > 0)
            {
                Hospital testingHospital = hospServ.GetHospitalUsingId(lab.scheduleTestingFacilityId);
                apptData.TestingHospitalId = lab.scheduleTestingFacilityId;
                apptData.TestingHospitalName = testingHospital.name;
            }
        }

        private void getIcd10s(Appointment lab, Appointment apptData)
        {
            if (lab.scheduleIcd10s.Count > 0)
            {
                lab.scheduleResultIcd10s = new List<MedicalCode>();
                apptData.scheduleIcd10s = new List<MedicalCode>();
                apptData.scheduleResultIcd10s = new List<MedicalCode>();


                foreach (MedicalCode icd in lab.scheduleIcd10s)
                {
                    if (icd.CodeId > 0)
                    {
                        int codeId = Convert.ToInt32(icd.CodeId);

                        MedicalCodeService medCodeServ = new MedicalCodeService(_icmsContext);
                        MedicalCode code = medCodeServ.getMedicalCodeById("ICD10", codeId);

                        if (!string.IsNullOrEmpty(code.Code))
                        {
                            MedicalCode addCode = new MedicalCode();

                            addCode.CodeId = code.CodeId;
                            addCode.Code = code.Code;
                            addCode.DisplayDescription = code.DisplayDescription;
                            addCode.LongDescription = code.LongDescription;
                            addCode.MediumDescription = code.MediumDescription;
                            addCode.LongDescription = code.LongDescription;

                            addCode.DisplayDescription = !string.IsNullOrEmpty(code.MediumDescription) ? code.MediumDescription :
                                                        ((!string.IsNullOrEmpty(code.LongDescription) ? code.LongDescription :
                                                        (!string.IsNullOrEmpty(code.ShortDescription) ? code.ShortDescription : "")
                                                        ));

                            lab.scheduleResultIcd10s.Add(addCode);

                            apptData.scheduleIcd10s.Add(addCode);
                            apptData.scheduleResultIcd10s.Add(addCode);
                        }
                    }
                }
            }


            //if (lab.scheduleResultIcd10s.Count > 0)
            //{
            //    foreach (MedicalCode icd in lab.scheduleResultIcd10s)
            //    {
            //        if (icd.CodeId > 0)
            //        {
            //            int codeId = Convert.ToInt32(icd.CodeId);

            //            MedicalCodeService medCodeServ = new MedicalCodeService(_icmsContext);
            //            MedicalCode code = medCodeServ.getMedicalCodeById("ICD10", codeId);

            //            if (!string.IsNullOrEmpty(code.Code))
            //            {
            //                icd.Code = code.Code;
            //                icd.DisplayDescription = code.DisplayDescription;
            //                icd.LongDescription = code.LongDescription;
            //                icd.MediumDescription = code.MediumDescription;
            //                icd.LongDescription = code.LongDescription;
            //            }
            //        }
            //    }
            //}
        }

        private void getProvider(Appointment lab, Appointment apptData)
        {
            DoctorService drServ = new DoctorService(_icmsContext, _aspNetContext, _datastagingContext);
            Doctor dr = drServ.GetDoctorUsingProvAddrId(lab.schedulePcpAddId.ToString(), lab.schedulePcpPhone);

            apptData.dr.pcpId = dr.pcpId;

            apptData.dr.firstName = dr.firstName;
            apptData.dr.lastName = dr.lastName;
            apptData.dr.practiceName = dr.practiceName;
            apptData.dr.npi = dr.npi;
            apptData.dr.specialtyDesc = dr.specialtyDesc;
            apptData.dr.emailAddress = dr.emailAddress;
            apptData.dr.phoneNumber = dr.phoneNumber;
            apptData.dr.address1 = dr.address1;
            apptData.dr.address2 = dr.address2;
            apptData.dr.city = dr.city;
            apptData.dr.stateAbbrev = dr.stateAbbrev;
            apptData.dr.zip = dr.zip;
        }

        private void getLabData(Appointment lab, Appointment apptData)
        {
            if (lab.scheduleAppointmentTypeId > 0)
            {
                AppointmentType apptType = getLabTypeAppointmentTypes(lab.scheduleAppointmentTypeId);
                apptData.appointmentAppointmentTypeId = apptType.appointmentTypeId;
                apptData.appointmentAppointmentTypeName = apptType.appointmentTypeName;
            }

            if (lab.scheduleLabTypeId > 0)
            {
                Lab labTyp = getLabType(lab.scheduleLabTypeId);
                apptData.appointmentLabTypeId = labTyp.labId;
                apptData.appointmentLabTypeName = labTyp.labName;
            }
        }

        private Lab getLabType(int labId)
        {
            Lab labTyp = new Lab();

            labTyp = (from labtyps in _icmsContext.LabTypes
                      where labtyps.hospital_order_test_id.Equals(labId)
                      select new Lab
                      {
                          labId = labtyps.hospital_order_test_id,
                          labName = labtyps.test_name
                      }).FirstOrDefault();

            return labTyp;
        }

        public LabResult getAdmissionLabResult(int labId)
        {
            LabResult labResult = null;

            if (labId > 0)
            {
                int orderId = (
                                from hospInptAdmOrdLabs in _icmsContext.HospitalInpatientAdmissionOrderLabs
                                where hospInptAdmOrdLabs.hospital_inpatient_admission_order_lab_id.Equals(labId)
                                select hospInptAdmOrdLabs.hospital_inpatient_admission_order_id
                              )
                              .FirstOrDefault();

                if (orderId > 0)
                {

                    labResult = (
                            from hospInptAdmOrdRslt in _icmsContext.HospitalInpatientAdmissionOrderResults
                            where hospInptAdmOrdRslt.hospital_inpatient_admission_order_id.Equals(orderId)
                            select new LabResult
                            {
                                resultId = hospInptAdmOrdRslt.hospital_inpatient_admission_order_result_id,
                                labId = labId,
                                orderId = orderId,
                                result = hospInptAdmOrdRslt.result,
                                flagId = hospInptAdmOrdRslt.hospital_order_result_flag_id
                            }
                        )
                        .FirstOrDefault();
                }

                    
            }

            return labResult;
        }




        private Admission CreateAdmissionForLab(HospitalService hospServ, Appointment lab)
        {
            Admission admit = new Admission();

            HospitalInpatientAdmission newAdmit = CreateInpatientAdmission(hospServ, lab);  


            if (newAdmit.hospital_inpatient_admission_id > 0)
            {
                admit.hospital_inpatient_admission_id = newAdmit.hospital_inpatient_admission_id;                
                admit.member_id = newAdmit.member_id;
                admit.hospital_id = newAdmit.hospital_id;
                admit.registration_number = newAdmit.registration_number;
                admit.hospital_department_id = newAdmit.hospital_department_id;
                admit.hospital_department_rooms_id = newAdmit.hospital_department_rooms_id;

                HospitalInpatientAdmissionOrder newOrder = CreateInpatientAdmissionOrder(hospServ, lab, newAdmit, "LAB");                


                if (newOrder.hospital_inpatient_admission_order_id > 0)
                {
                    admit.hospital_inpatient_admission_order_id = newOrder.hospital_inpatient_admission_order_id;
                    admit.order_number = newOrder.order_number;
                    admit.accession_number = newOrder.accession_number;

                    HospitalInpatientAdmissionOrderLab newLab = CreateInpatientAdmissionOrderLab(hospServ, lab, newOrder);


                    if (newLab.hospital_inpatient_admission_order_lab_id > 0)
                    {
                        admit.hospital_inpatient_admission_order_lab_id = newLab.hospital_inpatient_admission_order_lab_id;
                    }


                    if (lab.scheduleIcd10s.Count > 0)
                    {
                        List<HospitalInpatientAdmissionOrderDiagnosis> diags= CreateInpatientAdmissionOrderDiagnosis(hospServ, lab, newAdmit, newOrder);


                        if (diags.Count > 0)
                        {
                            admit.orderDiags = diags;
                        }
                    }
                }
            }
            

            return admit;
        }

        private HospitalInpatientAdmission CreateInpatientAdmission(HospitalService hospServ, Appointment lab)
        {
            HospitalInpatientAdmission newAdmit = new HospitalInpatientAdmission();


            if (!lab.patient.PatientId.Equals(Guid.Empty))
            {
                newAdmit.member_id = (Guid)lab.patient.PatientId;
                newAdmit.hospital_id = (int)lab.scheduleCollectionFacilityId;
                newAdmit.hospital_department_id = lab.scheduleCollectionDepartmentId;
                newAdmit.hospital_department_rooms_id = lab.scheduleRoomId;
                newAdmit.registered_date = lab.scheduleCollectionDateTime;
                newAdmit.creation_user_id = (Guid)lab.user.UserId;
                newAdmit.creation_date = DateTime.Now;

                hospServ.CreateInpatientAdmission(newAdmit);
            }


            return newAdmit;
        }

        private HospitalInpatientAdmissionOrder CreateInpatientAdmissionOrder(HospitalService hospServ,
                                                                              Appointment lab,
                                                                              HospitalInpatientAdmission admit,
                                                                              string orderType)
        {
            HospitalInpatientAdmissionOrder newOrder = new HospitalInpatientAdmissionOrder();


            if (admit.hospital_inpatient_admission_id > 0 && !string.IsNullOrEmpty(orderType))
            {
                switch (orderType)
                {
                    case "LAB":
                        newOrder.hospital_order_type_id = 1;
                        break;
                }

                newOrder.hospital_inpatient_admission_id = admit.hospital_inpatient_admission_id;
                newOrder.order_note = "Portal Generated Order";
                newOrder.creation_user_id = admit.creation_user_id;
                newOrder.creation_date = admit.creation_date;

                hospServ.CreateInpatientAdmissionOrder(newOrder);
            }


            return newOrder;
        }

        private HospitalInpatientAdmissionOrderLab CreateInpatientAdmissionOrderLab(HospitalService hospServ,
                                                                                    Appointment lab, 
                                                                                    HospitalInpatientAdmissionOrder order)
        {
            HospitalInpatientAdmissionOrderLab newLab = new HospitalInpatientAdmissionOrderLab();


            if (order.hospital_inpatient_admission_order_id > 0)
            {                
                newLab.hospital_inpatient_admission_order_id = order.hospital_inpatient_admission_order_id;
                newLab.hospital_order_test_id = lab.scheduleLabTypeId;

                if (!string.IsNullOrEmpty(lab.scheduleCollectionDateTime.ToString()))
                { 
                    newLab.specimen_collection_date = lab.scheduleCollectionDateTime;                
                }

                newLab.creation_user_id = order.creation_user_id;
                newLab.creation_date = order.creation_date;

                hospServ.CreateInpatientAdmissionOrderLab(newLab);
            }

            return newLab;
        }

        private List<HospitalInpatientAdmissionOrderDiagnosis> CreateInpatientAdmissionOrderDiagnosis(HospitalService hospServ,
                                                                                                      Appointment lab, 
                                                                                                      HospitalInpatientAdmission admit, 
                                                                                                      HospitalInpatientAdmissionOrder order)
        {
            List<HospitalInpatientAdmissionOrderDiagnosis> diags = new List<HospitalInpatientAdmissionOrderDiagnosis>();


            if (admit.hospital_inpatient_admission_id > 0 &&
                order.hospital_inpatient_admission_order_id > 0)
            {
                foreach (MedicalCode code in lab.scheduleIcd10s)
                {
                    HospitalInpatientAdmissionOrderDiagnosis icd10 = new HospitalInpatientAdmissionOrderDiagnosis();
                    icd10.hospital_inpatient_admission_id = admit.hospital_inpatient_admission_id;
                    icd10.hospital_inpatient_admission_order_id = order.hospital_inpatient_admission_order_id;
                    icd10.diagnosis_codes_10_id = code.CodeId;
                    icd10.creation_date = admit.creation_date;
                    icd10.creation_user_id = admit.creation_user_id;

                    diags.Add(icd10);
                }


                hospServ.CreateInpatientAdmissionOrderDiagnosis(diags);
            }


            return diags;
        }

        private HospitalAppointmentSchedule CreateAppointmentForLab(HospitalService hospServ, Appointment lab)
        {
            HospitalAppointmentSchedule appt = new HospitalAppointmentSchedule();


            if (!lab.patient.PatientId.Equals(Guid.Empty))
            {
                appt.member_id = (Guid)lab.patient.PatientId;
                appt.hospital_id = (int)lab.HospitalId;
                appt.hospital_department_id = (int)lab.DepartmentId;
                appt.appointment_start_date = lab.appointmentStartDateTime;
                appt.appointment_end_date = lab.appointmentEndDateTime;
                appt.appointment_type_id = lab.appointmentAppointmentTypeId;
                appt.hospital_department_rooms_id = lab.scheduleRoomId;
                appt.estimated_delivery_date = lab.estimatedDeliveryDate;
                appt.scheduled_on_date = DateTime.Now;
                appt.scheduled_by_user_id = (Guid)lab.user.UserId;

                if (lab.appointmentHospitalInpatientAdmissionId > 0)
                {
                    appt.hospital_inpatient_admission_id = lab.appointmentHospitalInpatientAdmissionId;
                }


                hospServ.CreateHospitalAppointmentSchedule(appt);
            }


            return appt;
        }




        public List<Lab> updateAdmissionLabs(Lab lab)
        {
            List<Lab> admissionLabs = null;

            DateTime creationDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(lab.createDate) && !DateTime.TryParse(lab.createDate, out creationDate))
            {
                creationDate = DateTime.Now;
            }

            HospitalService hospServ = new HospitalService(_icmsContext, _aspNetContext);

            HospitalInpatientAdmissionOrder order = new HospitalInpatientAdmissionOrder();
            order.hospital_inpatient_admission_id = lab.admissionId;
            order.hospital_order_type_id = lab.orderTypeId;
            order.order_note = lab.note;
            order.creation_date = creationDate;
            order.creation_user_id = (!lab.usr.Equals(Guid.Empty)) ? lab.usr : Guid.Empty;

            if (hospServ.CreateInpatientAdmissionOrder(order))
            {
                Appointment labAppt = new Appointment();
                labAppt.scheduleLabTypeId = (int)lab.testId;
                labAppt.scheduleCollectionDateTime = creationDate;

                HospitalInpatientAdmissionOrderLab labOrder = CreateInpatientAdmissionOrderLab(hospServ, labAppt, order);

                if (labOrder != null && labOrder.hospital_inpatient_admission_order_lab_id > 0)
                {
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

                                        where admits.hospital_inpatient_admission_id.Equals(order.hospital_inpatient_admission_id)
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
                }
            }



            return admissionLabs;
        }


        public LabResult updateAdmissionLabResults(LabResult result)
        {
            LabResult admissionLabResult = null;

            DateTime creationDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(result.createDate) && !DateTime.TryParse(result.createDate, out creationDate))
            {
                creationDate = DateTime.Now;
            }

            int orderId = (
                            from hospInptAdmOrdLab in _icmsContext.HospitalInpatientAdmissionOrderLabs

                            join hospInptAdmOrd in _icmsContext.HospitalInpatientAdmissionOrders
                            on hospInptAdmOrdLab.hospital_inpatient_admission_order_id equals hospInptAdmOrd.hospital_inpatient_admission_order_id

                            where hospInptAdmOrdLab.hospital_inpatient_admission_order_lab_id.Equals(result.labId)

                            select hospInptAdmOrdLab.hospital_inpatient_admission_order_id

                          )
                          .FirstOrDefault();

            if (orderId > 0)
            {

                HospitalInpatientAdmissionOrderResult dbLabResult = (
                                                    from hospInptAdmOrdRslt in _icmsContext.HospitalInpatientAdmissionOrderResults                                                                        
                                                    where hospInptAdmOrdRslt.hospital_inpatient_admission_order_id.Equals(orderId)
                                                    select hospInptAdmOrdRslt
                                                )
                                                .FirstOrDefault();

                if (dbLabResult != null)
                {
                    dbLabResult.result = result.result;
                    dbLabResult.hospital_order_result_flag_id = result.flagId;
                    dbLabResult.update_date = creationDate;
                    dbLabResult.update_user_id = result.usr;

                    _icmsContext.HospitalInpatientAdmissionOrderResults.Update(dbLabResult);
                }
                else
                {
                    HospitalInpatientAdmissionOrderResult newResult = new HospitalInpatientAdmissionOrderResult();
                    newResult.result = result.result;
                    newResult.result_date = creationDate;
                    newResult.hospital_inpatient_admission_order_id = orderId;
                    newResult.hospital_order_result_flag_id = result.flagId;
                    newResult.creation_date = creationDate;
                    newResult.creation_user_id = result.usr;

                    _icmsContext.HospitalInpatientAdmissionOrderResults.Add(newResult);
                }

                int dbResult = _icmsContext.SaveChanges();

                if (dbResult > 0)
                {
                    admissionLabResult = (
                                from hospInptAdmRslt in _icmsContext.HospitalInpatientAdmissionOrderResults

                                join hospInptAdmOrdLab in _icmsContext.HospitalInpatientAdmissionOrderLabs
                                on hospInptAdmRslt.hospital_inpatient_admission_order_id equals hospInptAdmOrdLab.hospital_inpatient_admission_order_id

                                where hospInptAdmRslt.hospital_inpatient_admission_order_id.Equals(orderId)
                                select new LabResult
                                {
                                    resultId = hospInptAdmRslt.hospital_inpatient_admission_order_result_id,
                                    labId = hospInptAdmOrdLab.hospital_inpatient_admission_order_lab_id,
                                    result = hospInptAdmRslt.result,
                                    flagId = hospInptAdmRslt.hospital_order_result_flag_id
                                }
                        )
                        .FirstOrDefault();
                }
            }


            return admissionLabResult;
        }






        private bool validateUser(Appointment lab)
        {
            bool userValidated = false;


            if (!string.IsNullOrEmpty(lab.scheduleUserId))
            {
                lab.user = new IcmsUser();
                lab.user.UserId = Guid.Empty;
                Guid userId = Guid.Empty;


                if (Guid.TryParse(lab.scheduleUserId, out userId))
                {
                    lab.user.UserId = userId;


                    if (!lab.user.UserId.Equals(Guid.Empty))
                    {
                        MembershipService memServ = new MembershipService(_aspNetContext);


                        if (memServ.validateUser(lab.user))
                        {
                            userValidated = true;
                        }
                    }
                }
            }


            return userValidated;
        }


    }
}

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

namespace eCareApi.Services
{
    public class IcuService : IIcu
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _emrContext;

        public IcuService(IcmsContext icmsContext, AspNetContext emrContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _emrContext = emrContext ?? throw new ArgumentNullException(nameof(emrContext));
        }

        public List<Patient> searchPreopPatients(Appointment preop)
        {
            List<Patient> patients = null;
            
            List<Member> matchingPatients = getPatientsMatchingSearch(preop);
                        
            if (matchingPatients != null && matchingPatients.Count > 0)
            {
                List<Member> patientsWithAdmits = filterPatientsWithAdmissions(matchingPatients);
                List<Member> patientsWtihAppointments = filterPatientsWithAppointments(matchingPatients);

                patients = filterAdmissionsAndAppointments(patientsWithAdmits, patientsWtihAppointments);
            }

            return patients;
        }

        public Icu getIcuFormItems()
        {
            Icu icuAssets = new Icu();

           HospitalService hospServ = new HospitalService(_icmsContext, _emrContext);

            List<Hospital> hospitals = hospServ.GetHospitals();

            StandardService standServ = new StandardService(_icmsContext, _emrContext);
            IEnumerable<State> states = standServ.GetStates();
            List<SimsErRelationship> relationships = standServ.getRelationships();


            icuAssets.hospitals = hospitals;
            icuAssets.states = states;
            icuAssets.relationships = relationships;

            return icuAssets;
        }

        public Icu getHospitalInpatientFormItems(string hospitalId)
        {
            Icu hospitalAssets = new Icu();

            int hospId = 0;

            if (int.TryParse(hospitalId, out hospId))
            {

                HospitalService hospServ = new HospitalService(_icmsContext, _emrContext);
                List<HospitalDepartment> hospitalDepartments = hospServ.getHospitalDepartmentsAsset(hospId);
                hospitalAssets.hospitalDepartments = hospitalDepartments;

            }


            return hospitalAssets;
        }

        public List<Admission> loadIcuRoomAdmissions(Admission icu)
        {
            List<Admission> icuAdmits = null;

            icuAdmits = (
                    from admits in _icmsContext.HospitalInpatientAdmissions

                    join hospDepart in _icmsContext.HospitalDepartments
                    on admits.hospital_department_id equals hospDepart.hospital_department_id

                    join deptRooms in _icmsContext.HospitalDepartmentRooms
                    on admits.hospital_department_rooms_id equals deptRooms.hospital_department_rooms_id

                    where hospDepart.is_icu.Equals(true)

                    select new Admission
                    {
                        registration_number = admits.registration_number,
                        hospital_inpatient_admission_id = admits.hospital_inpatient_admission_id,
                        admitDate = admits.registered_date,
                        displayAdmitDate = (admits.registered_date.HasValue) ? admits.registered_date.Value.ToShortDateString() : "N/A",
                        hospital_department_id = admits.hospital_department_id,
                        hospital_department_rooms_id = admits.hospital_department_rooms_id,
                        departmentRoomName = deptRooms.name
                    }
                )
                .ToList();


            return icuAdmits;
        }


        public Appointment getHospitalIcuWorkDay(Appointment icu)
        {
            Appointment workday = null;

            if (icu.HospitalId > 0)
            {
                icu.searchIcuDepartmentId = getHospitalIcuDepartmentId(icu);

                if (icu.searchIcuDepartmentId > 0)
                {

                    workday = new Appointment();

                    workday.deptRooms = getIcuDepartmentRoomList(icu.searchIcuDepartmentId);
                    workday.departmentWorkday = getIcuDepartmentWorkday(icu.searchIcuDepartmentId);
                    workday.datesSchedule = getDatesSchedule(icu);
                    workday.daySchedule = getDaySchedule(icu, workday);
                    workday.scheduleAppointments= getIcuAppointmentsScheduled(icu);

                    workday.selectedAppointmentType= getIcuTypeAppointmentTypes(icu.searchAppointmentTypeId);
                    
                }
            }

            return workday;
        }


        private List<Room> getIcuDepartmentRoomList(int deptId)
        {
            List<Room> rooms = new List<Room>();

            rooms = (
                     from dept in _icmsContext.HospitalDepartments
                     
                     join room in _icmsContext.HospitalDepartmentRooms
                     on dept.hospital_department_id equals room.hospital_department_id

                     join hospdept in _icmsContext.HospitalDepartments
                     on dept.hospital_department_id equals hospdept.hospital_department_id

                     join hosp in _icmsContext.Hospitals
                     on hospdept.hospital_id equals hosp.hospital_id

                     where room.hospital_department_id.Equals(deptId)
                     && dept.is_icu.Equals(true)
                     select new Room
                     {
                         hospitalId = hosp.hospital_id,
                         hospitalName = hosp.name,
                         departmentRoomsId = room.hospital_department_rooms_id,
                         departmentId = room.hospital_department_id,
                         roomName = room.name,
                         roomOccupancy = room.occupancy,
                         roomDescription = room.description,
                         roomAvailable = room.room_available
                     })
                        .ToList();

            return rooms;
        }

        

        private HospitalDepartmentWorkday getIcuDepartmentWorkday(int deptId)
        {
            HospitalDepartmentWorkday wrkDay = new HospitalDepartmentWorkday();

            wrkDay = (
                        from wrkday in _icmsContext.HospitalDepartmentWorkdays

                        join icudept in _icmsContext.HospitalDepartments
                        on wrkday.hospital_department_id equals icudept.hospital_department_id

                        where wrkday.hospital_department_id.Equals(deptId)
                        && icudept.is_icu.Equals(true)

                        select wrkday
                     )
                     .FirstOrDefault();

            return wrkDay;
        }

        private List<Appointment> getDatesSchedule(Appointment icu)
        {
            List<Appointment> datesSchedule = null;

            DateTime icuStartDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(icu.searchIcuDate) && DateTime.TryParse(icu.searchIcuDate, out icuStartDate))
            {
                datesSchedule = new List<Appointment>();
                
                Appointment addDate = new Appointment();
                addDate.appointmentStartDateTime = icuStartDate;
                datesSchedule.Add(addDate);

                for (int day=1; day <= 13; day++)
                {
                    Appointment nextDate = new Appointment();
                    nextDate.appointmentStartDateTime = icuStartDate.AddDays(day);
                    datesSchedule.Add(nextDate);
                }
            }

            return datesSchedule;
        }

        private List<Appointment> getDaySchedule(Appointment icu, Appointment appt)
        {
            List<Appointment> daySchedule = null;

            DateTime icuStartDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(icu.searchIcuDate) && DateTime.TryParse(icu.searchIcuDate, out icuStartDate))
            {                               
                if (appt.departmentWorkday != null)
                {
                    daySchedule = new List<Appointment>();

                    TimeSpan startTime = appt.departmentWorkday.workday_start_time;
                    TimeSpan endTime = appt.departmentWorkday.workday_end_time;
                    double totalStartMinutes = startTime.TotalMinutes;
                    double totalEndMinutes = endTime.TotalMinutes;

                    for (double start = totalStartMinutes; start <= totalEndMinutes; start += 15)
                    {
                        DateTime scheuduleDateTime = icuStartDate.AddMinutes(start);

                        Appointment addAppt = new Appointment();
                        addAppt.appointmentStartDateTime = scheuduleDateTime;

                        daySchedule.Add(addAppt);

                    }

                }
            }

            return daySchedule;
        }

        private List<Appointment> getIcuAppointmentsScheduled(Appointment icu)
        {
            List<Appointment> icuAppts = null;

            DateTime icuDate = DateTime.MinValue;

            if (!string.IsNullOrEmpty(icu.searchIcuDate) && DateTime.TryParse(icu.searchIcuDate, out icuDate))
            {
                icuAppts = (
                    
                            from deptAppts in _icmsContext.HospitalAppointmentSchedules

                                join hosp in _icmsContext.Hospitals
                                on deptAppts.hospital_id equals hosp.hospital_id

                                join dept in _icmsContext.HospitalDepartments
                                on deptAppts.hospital_department_id equals dept.hospital_department_id

                                where deptAppts.hospital_id.Equals(icu.HospitalId)
                                && deptAppts.hospital_department_id.Equals(icu.searchIcuDepartmentId)
                                && (deptAppts.appointment_start_date >= icuDate &&
                                    deptAppts.appointment_start_date <= icuDate.AddDays(14))
                                select new Appointment
                                {
                                    HospitalName = hosp.name,
                                    HospitalId = deptAppts.hospital_id,
                                    DepartmentName = dept.hospital_department_name,
                                    DepartmentId = deptAppts.hospital_department_id,
                                    appointmentStartDateTime = deptAppts.appointment_start_date,
                                    appointmentEndDateTime = deptAppts.appointment_end_date,
                                    returnSearchFromDate = icuDate,
                                }
                      )
                      .ToList();

            }

            return icuAppts;
        }



        private AppointmentType getIcuTypeAppointmentTypes(int apptTypeId)
        {
            AppointmentType apptType = new AppointmentType();

            apptType = (from deptApptTypes in _icmsContext.HospitalDepartmentAppointmentTypes
                        join apptTypeDur in _icmsContext.HospitalDepartmentAppointmentTypesDurationReferences
                        on deptApptTypes.hospital_department_appointment_types_id equals apptTypeDur.hospital_department_appointment_types_id
                        where deptApptTypes.hospital_department_appointment_types_id.Equals(apptTypeId)
                        select new AppointmentType
                        {
                            appointmentTypeId = deptApptTypes.hospital_department_appointment_types_id,
                            appointmentTypeName = deptApptTypes.appointment_type_name,
                            duration = apptTypeDur.minimum_minutes
                        }).FirstOrDefault();

            return apptType;
        }

        public int getHospitalIcuDepartmentId(Appointment icu)
        {
            int icuDeptId = 0;

            HospitalDepartment dept = (
                                        from hospdept in _icmsContext.HospitalDepartments

                                        join hosp in _icmsContext.Hospitals
                                        on hospdept.hospital_id equals hosp.hospital_id

                                        where hospdept.hospital_id.Equals(icu.HospitalId)
                                        && hospdept.is_icu.Equals(true)

                                        select hospdept
                                      )
                                      .FirstOrDefault();

            if (dept != null && dept.hospital_department_id > 0)
            {
                icuDeptId = dept.hospital_department_id;
            }

            return icuDeptId;
        }

        private List<Member> getPatientsMatchingSearch(Appointment preop)
        {
            List<Member> members = null;

            DateTime dateOfBirth = (!string.IsNullOrEmpty(preop.searchDob)) ? DateTime.Parse(preop.searchDob) : DateTime.MinValue;          

            if (!dateOfBirth.Equals(DateTime.MinValue))
            {
                members = (
                            from mems in _icmsContext.Patients

                            where mems.member_birth.Equals(dateOfBirth)
                            && mems.member_first_name.Contains(preop.searchFirstName)
                            && mems.member_last_name.Contains(preop.searchLastName)

                            select mems
                         )
                         .Take(50)
                         .ToList();
            }
            else
            {

                members = (
                            from mems in _icmsContext.Patients

                            where mems.member_first_name.Contains(preop.searchFirstName)
                            && mems.member_last_name.Contains(preop.searchLastName)

                            select mems
                         )
                         .Take(50)
                         .ToList();
            }

            return members;

        }

        private List<Member> filterPatientsWithAdmissions(List<Member> filterPatients)
        {
            List<Member> patientsWithAdmits = null;

            foreach (Member pat in filterPatients)
            {
                patientsWithAdmits = (
                            from hosadmit in _icmsContext.HospitalInpatientAdmissions
                            where hosadmit.member_id.Equals(pat.member_id)

                            join mem in _icmsContext.Patients
                            on hosadmit.member_id equals mem.member_id

                            select mem
                         )
                         .Distinct()
                         .ToList();
            }

            return patientsWithAdmits;

        }

        private List<Member> filterPatientsWithAppointments(List<Member> filterPatients)
        {
            List<Member> patientsWithAppointments = null;

            foreach (Member pat in filterPatients)
            {
                patientsWithAppointments = (
                                        from hospsched in _icmsContext.HospitalAppointmentSchedules
                                        where hospsched.member_id.Equals(pat.member_id)

                                        join mem in _icmsContext.Patients
                                        on hospsched.member_id equals mem.member_id

                                        select mem
                                     )
                                     .Distinct()
                                     .ToList();
            }

            return patientsWithAppointments;
        }

        private List<Patient> filterAdmissionsAndAppointments(List<Member> patientsWithAdmits, List<Member> patientsWtihAppointments)
        {
            List<Patient> filteredPatients = null;

            List<Member> duplicatePatients = getDuplicatePatientList(patientsWithAdmits, patientsWtihAppointments);
            List<Member> finalPatients = filterFinalPatientList(patientsWithAdmits, patientsWtihAppointments, duplicatePatients);

            if (finalPatients != null && finalPatients.Count > 0)
            {
                filteredPatients = new List<Patient>();
                filteredPatients = getPatients(finalPatients);
            }

            return filteredPatients;
        }

        private List<Member> getDuplicatePatientList(List<Member> patientsWithAdmits, List<Member> patientsWtihAppointments)
        {
            List<Member> duplicatePatients = null;

            foreach (Member admitPatient in patientsWithAdmits)
            {
                foreach (Member apptPatient in patientsWtihAppointments)
                {
                    if (admitPatient.member_id.Equals(apptPatient.member_id))
                    {
                        if (duplicatePatients == null) duplicatePatients = new List<Member>();

                        duplicatePatients.Add(admitPatient);
                    }
                }
            }

            return duplicatePatients;

        }

        private List<Member> filterFinalPatientList(List<Member> patientsWithAdmits, List<Member> patientsWtihAppointments, List<Member> duplicatePatients)
        {
            List<Member> finalPatients = null;

            bool duplicateAvailable = false;

            if (duplicatePatients != null)
            {
                duplicateAvailable = true;
                finalPatients = new List<Member>();

                foreach(Member duplicatePatient in duplicatePatients)
                {
                    finalPatients.Add(duplicatePatient);
                }
            }

            if (patientsWithAdmits != null)
            {
                foreach (Member admitPatient in patientsWithAdmits)
                {
                    if (duplicateAvailable)
                    {
                        bool notDuplicate = true;

                        foreach(Member duplicateAdmit in duplicatePatients)
                        {
                            //the patient is already in the final list
                            if (admitPatient.member_id.Equals(duplicateAdmit.member_id))
                            {
                                notDuplicate = false;
                                break;
                            }
                        }

                        if (notDuplicate)
                        {
                            finalPatients.Add(admitPatient);
                        }
                    }
                    else
                    {
                        finalPatients.Add(admitPatient);
                    }
                }
            }

            if (patientsWtihAppointments != null)
            {
                foreach (Member apptPatient in patientsWtihAppointments)
                {
                    if (duplicateAvailable)
                    {
                        bool notDuplicate = true;

                        foreach (Member duplicateAppt in duplicatePatients)
                        {
                            //the patient is already in the final list
                            if (apptPatient.member_id.Equals(duplicateAppt.member_id))
                            {
                                notDuplicate = false;
                                break;
                            }
                        }

                        if (notDuplicate)
                        {
                            finalPatients.Add(apptPatient);
                        }
                    }
                    else
                    {
                        finalPatients.Add(apptPatient);
                    }
                }
            }

            return finalPatients;
        }

        private List<Patient> getPatients(List<Member> finalPatients)
        {
            List<Patient> returnPatients = null;

            PatientService patServ = new PatientService(_icmsContext, _emrContext);

            returnPatients = patServ.getPatientsFromList(finalPatients);

            return returnPatients;
        }

    }

}

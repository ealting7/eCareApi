using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface ILab
    {

        public List<Lab> getAllLabTypes();

        public List<LabResult> getAllLabResultFlags();

        IEnumerable<Lab> GetHospitalLabTypes();

        Lab GetLabTypeTests(string id);

        List<HospitalDepartment> GetHospitalCollectionDepartments(int hospId);

        List<HospitalDepartmentAppointmentTypes> GetLabTypeAppointmentTypes(int labTypeId);

        List<MedicalCode> GetLabTypeIcd10s(int labTypeId);

        List<Appointment> GetLabAvailableAppointments(Appointment lab, DateTime srchFromDate);

        Appointment GetAppointmentScheduleData(Appointment lab);

        Admission CreateLabAppointment(Appointment lab);

        DateTime getSearchFromDate(Appointment lab);

        List<DateTime> getTwoWeeksOutDate(DateTime startDate);

        IEnumerable<Member> GetPatientWithLabs(string first, string last, string dob);

        List<Lab> GetPatientLabs(string id);

        Lab GetPatientLab(string id);

        IEnumerable<Lab> GetTodaysLabs(string today, string hospitalId);

        public LabResult getAdmissionLabResult(int labId);

        public List<Lab> updateAdmissionLabs(Lab lab);

        public LabResult updateAdmissionLabResults(LabResult result);
    }
}

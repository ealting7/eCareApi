using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Appointment
    {
        public int searchCollectionFacilityId { get; set; }
        public int searchCollectionDepartmentId { get; set; }
        public int searchLabTypeId { get; set; }
        public int searchAppointmentTypeId { get; set; }
        public string searchCollectionDate { get; set; }
        public string searchCollectionTime { get; set; }
        public int searchDaysOut { get; set; }
        public string searchType { get; set; }


        public string schedulePatientId { get; set; }
        public int? scheduleRoomId  { get; set; }

        public string scheduleUserId { get; set; }
        public int schedulePcpAddId { get; set; }
        public string schedulePcpPhone { get; set; }
        public int scheduleCollectionFacilityId  { get; set; }
        public int scheduleCollectionDepartmentId  { get; set; }
        public int scheduleTestingFacilityId  { get; set; }
        public int scheduleLabTypeId  { get; set; }
        public int scheduleAppointmentTypeId  { get; set; }
        public string scheduleCollectionDate  { get; set; }
        public string scheduleCollectionTime  { get; set; }
        public DateTime scheduleCollectionDateTime  { get; set; }
        public string scheduleCollectionEndDate { get; set; }
        public string scheduleCollectionEndTime { get; set; }
        public DateTime scheduleCollectionEndDateTime { get; set; }

        public DateTime estimatedDeliveryDate { get; set; }
        public string scheduleType  { get; set; }


        public List<MedicalCode> scheduleIcd10s { get; set; }
        public List<MedicalCode> scheduleResultIcd10s { get; set; }

        //public Specimen scheduleSpecimen {get; set;}



        public DateTime returnSearchFromDate { get; set; }
        public List<DateTime> returnTwoWeekSchedule { get; set; }


        public Patient patient { get; set; }

        public string? HospitalName { get; set; }
        public int? HospitalId { get; set; }

        public string? TestingHospitalName { get; set; }
        public int? TestingHospitalId { get; set; }

        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public string? RoomName { get; set; }
        public int? RoomId { get; set; }
        public int? RoomOccupancy { get; set; }


        public int? appointmentHospitalInpatientAdmissionId { get; set; }
        public DateTime? appointmentStartDateTime { get; set; }
        public DateTime? appointmentEndDateTime { get; set; }
        public int? appointmentLabTypeId {get; set;}

        public string? appointmentLabTypeName { get; set; }

        public int? appointmentAppointmentTypeId { get; set; }

        public string? appointmentAppointmentTypeName { get; set; }

        public int? appointmentAppointmentTypeDuration { get; set; }


        public AppointmentType selectedAppointmentType { get; set; }
        public HospitalDepartmentWorkday departmentWorkday { get; set; }
        public List<HospitalDepartmentRooms> departmentRooms { get; set; }
        public DateTime deptStartTime { get; set; }
        public DateTime deptEndTime { get; set; }

        public Doctor dr { get; set; }

        public IcmsUser user { get; set; }

    }
}

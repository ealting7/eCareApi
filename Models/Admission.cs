using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Admission
    {
        public int hospital_inpatient_admission_id { get; set; }
        public int hospital_inpatient_admission_order_id { get; set; }
        public int hospital_inpatient_admission_order_lab_id { get; set; }
        public Guid member_id { get; set; }
        public string patientName { get; set; }
        public string patientFullName { get; set; }
        public Guid usr { get; set; }
        public int? hospital_id { get; set; }
        public string? registration_number { get; set; }
        public int? order_number { get; set; }
        public string? accession_number { get; set; }
        public int? hospital_department_id { get; set; }

        public string departmentName { get; set; }
        public int? hospital_department_rooms_id { get; set; }
        public string departmentRoomName { get; set; }

        public List<Room> departmentRooms { get; set; }

        public int? admission_diagnosis_code_id {get; set;}

        public string admitIcdCode { get; set; }
        public string admitIcdCodeDescription { get; set; }


        public string reasonForVisit { get; set; }

        public DateTime? admitDate { get; set; }
        public string displayAdmitDate { get; set; }

        public int lengthOfStay { get; set; }

        public string nurseName { get; set; }

        public string physicianName { get; set; }

        public Patient patient { get; set; }
        public List<HospitalInpatientAdmissionOrderDiagnosis> orderDiags { get; set; }
        public List<HospitalInpatientAdmissionOrderCpt> orderCpts { get; set; }
        public List<HospitalInpatientAdmissionOrderHcpcs> orderHcpcs { get; set; }
        public List<HospitalInpatientAdmissionOrderDiagnosis> orderResultDiags { get; set; }


        public HospitalAppointmentSchedule appointment { get; set; }
    }
}

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
        public int? hospital_id { get; set; }
        public string? registration_number { get; set; }
        public int? order_number { get; set; }
        public string? accession_number { get; set; }
        public int? hospital_department_id { get; set; }
        public int? hospital_department_rooms_id { get; set; }

        public Patient patient { get; set; }
        public List<HospitalInpatientAdmissionOrderDiagnosis> orderDiags { get; set; }
        public List<HospitalInpatientAdmissionOrderCpt> orderCpts { get; set; }
        public List<HospitalInpatientAdmissionOrderHcpcs> orderHcpcs { get; set; }
        public List<HospitalInpatientAdmissionOrderDiagnosis> orderResultDiags { get; set; }


        public HospitalAppointmentSchedule appointment { get; set; }
    }
}

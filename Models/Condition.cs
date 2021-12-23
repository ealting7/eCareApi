using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Condition
    {
        public int patientConditionRefId {get; set;}
        public Guid patientId { get; set; }
        public int conditionId { get; set; }
        public string conditionName { get; set; }
        public int? diagnosisCodes10Id { get; set; }

        public int? hospitalInpatientAdmissionId { get; set; }
        public string? registrationNumber { get; set; }

        public DateTime? diagnosisDate { get; set; }
        public string displayDiagnosisDate { get; set; }

        public string diagnosisCode { get; set; }
        public string diagnosisCodeDesc { get; set; }


        public List<MedicalCode> differentialIcds { get; set; }
    }
}

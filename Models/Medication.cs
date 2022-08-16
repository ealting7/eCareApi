using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Medication
    {
        public int admissionId { get; set; }
        public int orderTypeId { get; set; }

        public int admissionMedicationOrderId { get; set; }

        public decimal? sequenceNumber { get; set; }
        public string medicationName { get; set; }
        public string ndc { get; set; }
        public string dose { get; set; }
        public string route { get; set; }
        public DateTime dateGive { get; set; }
        public string displayDateGive { get; set; }
        public string timeGiven { get; set; }
        public string administeredByName { get; set; }

        public MedicationAdministered lastAdministeredData { get; set; }


        public string note { get; set; }
        public DateTime? createDate { get; set; }
        public string displayCreateDate { get; set; }
        public DateTime? updateDate { get; set; }
        public DateTime? dateRemoved { get; set; }
        public string displayDateRemoved { get; set; }
        public Guid? usr { get; set; }
    }
}

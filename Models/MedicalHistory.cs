using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class MedicalHistory
    {
        public int historyId { get; set; }
        public Guid patientId { get; set; }
        public bool isFamilyHistory { get; set; }
        public string history { get; set; }
    }
}

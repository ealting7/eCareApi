using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Isolation
    {
        public int isolationId { get; set; }

        public int admissionId { get; set; }
        public string goal { get; set; }
        public string plan { get; set; }
        public bool? isolateMe { get; set; }
        public string infectionStatus { get; set; }
    }
}

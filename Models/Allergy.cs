using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Allergy
    {
        public int allergyId { get; set; }
        public int admissionId { get; set; }
        public string medicationAllergy { get; set; }
        public string otherAllergies { get; set; }
        public bool? latexAllergy { get; set; }
        public bool? echinaceaAllergy { get; set; }
        public bool? ephedraAllergy { get; set; }
        public bool? garlicAllergy { get; set; }
        public bool? gingkoBilobaAllergy { get; set; }
        public bool? ginkgoAllergy { get; set; }
        public bool? ginsengAllergy { get; set; }
        public bool? kavaAllergy { get; set; }
        public bool? stJohnsWortAllergy { get; set; }
        public bool? valerianAllergy { get; set; }
        public bool? valerianRootAllergy { get; set; }
        public bool? viteAllergy { get; set; }

        public string identifiedAtType { get; set; }

        public string allergyDescr { get; set; }
    }
}

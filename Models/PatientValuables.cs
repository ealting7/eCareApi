using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class PatientValuables
    {
        public int safetyId { get; set; }
        public int admissionId { get; set; }
        public bool? noValuables { get; set; }
        public bool? valuablesSentHome { get; set; }
        public bool? valuablesInHospitalSafe { get; set; }
        public string valuablesList { get; set; }
        public bool? hasGlasses { get; set; }
        public bool? hasHearingAidRight { get; set; }
        public bool? hasHearingAidLeft { get; set; }
        public bool? hasDenturesUpper { get; set; }
        public bool? hasDenturesLower { get; set; }
        public bool? hasContactLenses { get; set; }
        public bool? hasWalkingAid { get; set; }
        public string walkingAid { get; set; }
    }
}

using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class PatientAsset
    {

        public List<HospitalMaritalStatusTypes> maritalStatuses { get; set; }
        public List<Employer> employers { get; set; }
        public List<PhoneType> phoneTypes { get; set; }
        public List<State> states { get; set; }
        public List<HospitalRace> races { get; set; }
        public List<Hospital> hospitals { get; set; }
        public List<Languages> languages { get; set; }
        public List<NewlyIdentifiedCmMemberCaseStatus> cmCaseStatus { get; set; }
        public List<IcmsUser> caseOwners { get; set; }
        public List<CaseType> caseTypes { get; set; }
    }
}

using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Icu
    {
        public List<Hospital> hospitals { get; set; }
        public List<HospitalDepartment> hospitalDepartments { get; set; }
        public IEnumerable<State> states { get; set; }

        public List<SimsErRelationship> relationships { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class PatientContacts
    {
        public int contactsId { get; set; }

        public int? hospital_inpatient_admission_id { get; set; }
        public string fullName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? middleName { get; set; }
        public string? maidenName { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public string? phoneNumber { get; set; }
        public int? relationshipId { get; set; }
        public string relationshipToPatient { get; set; }
    }
}

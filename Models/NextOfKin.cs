using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class NextOfKin
    {
        public int nextOfKinId { get; set; }
        public Guid patientId { get; set; }

        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int relationshipId { get; set; }
        public string relationship { get; set; }
        public string? phoneNumber { get; set; }
        public string? emailAddress { get; set; }
    }
}

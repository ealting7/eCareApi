using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Note
    {
        public Guid memberId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        public DateTime recordDate { get; set; }
        public string noteText { get; set; }

        public int noteSequenceNumber { get; set; }

        public int? billingId { get; set; }

        public int? billingMinutes { get; set; }

        public Guid? caseOwnerId { get; set; }
        public string caseOwnerName { get; set; }
    }
}

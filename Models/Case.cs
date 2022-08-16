using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Case
    {

        public Guid memberId { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }

        public Patient patient { get; set; }

        public List<Note> cmNotes { get; set; }

        public Guid? caseOwnerId { get; set; }

        public string? referralNumber { get; set; }
        public int lcnCaseNumber { get; set; }
        public int lcmFollowupId { get; set; }


        public string reportType { get; set; }


        public DateTime lcmOpenDate { get; set; }
        public DateTime? lcmFinalizedDate { get; set; }
        public DateTime? lcmCloseDate { get; set; }
        public DateTime? lcmNextReportDate { get; set; }
        public bool useHistoricalNotes { get; set; }

        public Guid? completedByUser { get; set; }

        public List<LcmReports> lcmReports { get; set; }


        public Guid usr { get; set; }
        public DateTime creationDate { get; set; }
    }
}

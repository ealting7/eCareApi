using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class UtilizationItem
    {
        public int utilizationItemId { get; set; }

        public Guid memberId { get; set; }
        public string referralNumber { get; set; }

        public string referralType { get; set; }

        public int lineNumber { get; set; }
        public int? typeId { get; set; }
        public string bedType { get; set; }
        public bool surgeryFlag { get; set; }
        public bool surgeryOnFirstDayFlag { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? decisionId { get; set; }
        public string decision { get; set; }
        public int? decisionById { get; set; }
        public string decisionBy { get; set; }
        public DateTime? nextReviewDate { get; set; }
        public int? numberOfDays { get; set; }
        public bool visitsRecurring { get; set; }
        public int? visitsNumPerPeriodRequested { get; set; }
        public int? visitsNumPerPeriodAuthorized { get; set; }
        public string visitsRequested { get; set; }
        public int? visitsPeriodRequested { get; set; }
        public int? visitsPeriodAuthorized { get; set; }
        public int? visitsNumPeriodsRequested { get; set; }
        public int? visitsNumPeriodsAuthorized { get; set; }
        public string visitsAuthorized { get; set; }
        public DateTime? visitsAuthorizedEndDate { get; set; }
        public DateTime? visitsAuthorizedStartDate { get; set; }
        public int? denialReasonId { get; set; }
        public string denialReason { get; set; }
        public DateTime? dateUpdated { get; set; }

        public DateTime? creationDate { get; set; }
        public Guid? usr { get; set; }
    }
}

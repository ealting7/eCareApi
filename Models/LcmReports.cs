using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class LcmReports
    {
        public int lcnCaseNumber { get; set; }
        public int lcmFollowupId { get; set; }
        public int newFollowupId { get; set; }

        public string? referralNumber { get; set; }
        public Guid? memberId { get; set; }

        public DateTime lcmOpenDate { get; set; }
        public string displayLcmOpenDate { get; set; }
        public DateTime lcmFinalizedDate { get; set; }
        public string displayLcmFinalizedDate { get; set; }
        public DateTime? lcmCloseDate { get; set; }
        public string displayLcmCloseDate { get; set; }

        public string cancerRelated { get; set; }
        public string staging { get; set; }
        public string stagingStatus { get; set; }
        public string hospitalized { get; set; }
        public string hospitalFiveDays { get; set; }
        public int? facilityId { get; set; }
        public string facilityName { get; set; }
        public string facilityType { get; set; }
        public DateTime? nextReportDate { get; set; }
        public string displayNextReportDate { get; set; }
        public Guid systemUserId { get; set; }
        public bool senttoadmin { get; set; }
        public bool senttorein { get; set; }
        public bool reportComplete { get; set; }
        public bool umFlag { get; set; }
        public bool cmFlag { get; set; }
        public bool lcmFlag { get; set; }
        public bool triggerFlag { get; set; }
        public string reportType { get; set; }
        public string primaryDiagnosis { get; set; }
        public string secondaryDiagnosis { get; set; }
        public string otherDiagnosis { get; set; }
        public string procedure { get; set; }
        public string authNumber { get; set; }
        public string tpaName { get; set; }
        public string reinsurerName { get; set; }

        public string? stopLoss { get; set; }

        public string acuity { get; set; }
        public string acuityChanged { get; set; }
        public DateTime? acuityDate { get; set; }

        public string prognosis { get; set; }
        public string planOfCare { get; set; }             

        public string previousTreatments { get; set; }
        public string futureTreatments { get; set; }
        public string psychoSocialSummary { get; set; }

        public bool useHistoricalNotes { get; set; }

        public Guid? completedByUser { get; set; }

        public List<MedicalCode> codes { get; set; }
        public List<Saving> savings { get; set; }

        public Note reportNotes { get; set; }

        public Guid usr { get; set; }

        public DateTime creationDate { get; set; }

    }
}

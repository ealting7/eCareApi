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

        public string referralNumber { get; set; }

        public int lineNumber { get; set; }
        public string referralType { get; set; }

        public DateTime recordDate { get; set; }
        public string displayRecordDate { get; set; }

        public string noteText { get; set; }

        public string lcmNoteType { get; set; }
        public int lcnCaseNumber { get; set; }
        public int? lcmFollowupId { get; set; }

        public string currentTreatment { get; set; }
        public string futureTreatment { get; set; }
        public string psychoSocialSummary { get; set; }
        public string nurseSummary { get; set; }
        public string physicianPrognosis { get; set; }
        public string previousTreatment { get; set; }
        public string newlyIdentified { get; set; }


        public int noteSequenceNumber { get; set; }

        public int? billingId { get; set; }

        public int? billingMinutes { get; set; }

        public Guid? caseOwnerId { get; set; }
        public string caseOwnerName { get; set; }


        public bool? onLetter { get; set; }

        public bool? midMonthBill { get; set; }
        public bool notPassedMidMonth { get; set; }


        public int noteId { get; set; }
        public int suspendNoteId { get; set; }
        public int admissionId { get; set; }
        public int hospitalNoteTypeId { get; set; }
        public string hospitalNoteTypeName { get; set; }
        public List<string> noteTypeIds { get; set; }

        public List<DocumentForm> attachments { get; set; }
    }
}

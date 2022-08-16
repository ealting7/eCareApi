using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class MedicalReview
    {
        public int mdReviewRequestId { get; set; }
        public int mdReviewQuestionId { get; set; }
        public int mdReviewDeterminationId { get; set; }

        public string? referralNumber { get; set; }
        public Guid? memberId { get; set; }

        public int? taskId { get; set; }

        public string taskNote { get; set; }

        public string requestNote { get; set; }
        public string questionNote { get; set; }
        public string answerNote { get; set; }
        public string determinationNote { get; set; }

        public DateTime? startActionDate { get; set; }
        public string displayStartActionDate { get; set; }
        public DateTime? endActionDate { get; set; }
        public string displayEndActionDate { get; set; }

        public Guid? assignedToUserId { get; set; }
        public string assignedToEmail { get; set; }

        public Guid? enteredByUserId { get; set; }
        public string enteredByUsername { get; set; }
        public string enteredByEmail { get; set; }


        public DateTime? creationDate { get; set; }
        public string displayCreationDate { get; set; }

        public int? decisionId { get; set; }


        public List<MedicalReview> questions { get; set; }

        public List<MedicalReview> determinations { get; set; }

        public Guid usr { get; set; }
    }
}

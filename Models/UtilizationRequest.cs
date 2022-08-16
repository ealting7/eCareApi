using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class UtilizationRequest
    {
        public int reviewRequestId { get; set; }
        public int reviewQuestionId { get; set; }

        public int reviewDeterminationId { get; set; }

        public string requestNote { get; set; }

        public string requestType { get; set; }

        public string referralNumber { get; set; }
        public Guid memberId { get; set; }

        public int? taskId { get; set; }
        public Guid? assignedToUserId { get; set; }
        public string? assignedToUser { get; set; }

        public DateTime? assignedStartDate { get; set; }
        public DateTime? assignedEndDate { get; set; }
        public DateTime? actualStartDate { get; set; }
        public DateTime? actualEndDate { get; set; }

        public int? decisionId { get; set; }

        public DateTime? createDate { get; set; }
        public string? displayCreateDate { get; set; }
        public string emailAddress { get; set; }
        public Guid? usr { get; set; }
    }
}

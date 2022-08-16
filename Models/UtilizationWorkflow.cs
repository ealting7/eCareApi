using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class UtilizationWorkflow
    {
        public Guid workflowId { get; set; }
        public string? referralNumber { get; set; }

        public int statusId { get; set; }
        public int reasonId { get; set; }

        public Guid? assignToUserId { get; set; }
        public string assignToUserName { get; set; }

        public string workflowDescription { get; set; }
        public string statusDescription { get; set; }
        public string reasonDescription { get; set; }

        public DateTime? creationDate { get; set; }
        public string displayCreationDate { get; set; }

    }
}

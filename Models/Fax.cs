using System;

namespace eCareApi.Models
{
    public class Fax
    {
        public int FaxId { get; set; }

        public string faxName { get; set; }

        public byte[]? FaxImage { get; set; }
        public string? FaxImageBase64 { get; set; }
        public string faxContentType { get; set; }

        public DateTime? CreateDate { get; set; }
        public string displayCreatedDate { get; set; }

        public Guid? MemberId { get; set; }

        public string? ReferralNumber { get; set; }

        public int? FaxQueueId { get; set; }
        public string? FaxQueueName { get; set; }

        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }

        public DateTime? AssignedToUserDate { get; set; }

        public string? emailFileName { get; set; }

        public bool deleted { get; set; }

        public Guid usr { get; set; }
    }
}

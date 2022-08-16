using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Email
    {

        public int emailId { get; set; }

        public int emailTypeId { get; set; }

        public string emailAddress { get; set; }
        public string ccEmailList { get; set; }
        public string emailSubject { get; set; }
        public string emailText { get; set; }
        public string emailAttachmentName { get; set; }
        public byte[] emailAttachment { get; set; }
        public string emailAttachmentName2 { get; set; }
        public byte[] emailAttachment2 { get; set; }
        public string emailAttachmentBase64 { get; set; }
        public string emailAttachmentContentType { get; set; }
        public string attachmentPassword { get; set; }

        public int tpaId { get; set; }

        public string referralNumber { get; set; }
        public Guid memberId { get; set; }

        public DateTime creationDate { get; set; }
        public string displayCreationDate { get; set; }
        public Guid usr { get; set; }
    }
}

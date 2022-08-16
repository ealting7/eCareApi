using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Letter
    {
        public int letterId { get; set; }
        public string letterFileName { get; set; }

        public string letterType { get; set; }

        public string referralNumber { get; set; }
        public Guid memberId { get; set; }

        public int utilizationItemId { get; set; }
        public int? typeId { get; set; }
        public int? decisionId { get; set; }

        public DateTime? creationDate { get; set; }
        public string displayCreationDate { get; set; }

        public string letterFileUrlLocation { get; set; }
        public string letterBase64 { get; set; }
        public byte[] letterPdf { get; set; }
        public string letterContentType { get; set; }

        public Guid? usr { get; set; }
    }
}

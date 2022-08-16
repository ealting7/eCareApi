using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class DocumentForm
    {
        public int admissionId { get; set; }

        public int documentId { get; set; }

        public Guid? memberId { get; set; }


        public byte[] documentImage { get; set; }
        public string documentUrlLocation { get; set; }
        public string documentBase64 { get; set; }
        public byte[] documentPdf { get; set; }
        public string documentContentType { get; set; }
        public string documentFileName { get; set; }

        public DateTime? creationDate { get; set; }
        public string displayCreationDate { get; set; }
        public Guid? usr { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class UnpaidReport
    {
        public int unpaidId { get; set; }
        public int reportId { get; set; }
        public int tpaId { get; set; }

        public Guid memberId { get; set; }

        public string invoiceNumber { get; set; }


        public string groupNum { get; set; }
        public string participant { get; set; }

        public string dept { get; set; }
        public string claimantLastName { get; set; }
        public string claimantFirstName { get; set; }
        public string claim { get; set; }
        public string procedure { get; set; }
        public string provLOC { get; set; }
        public string status { get; set; }
        public string charge { get; set; }
        public string inelig { get; set; }
        public string total { get; set; }
        public string serviceDate { get; set; }
        public string paidDate {get ;set;}
        public string complete { get; set; }




 
        public string reportFileUrlLocation { get; set; }
        public string reportBase64 { get; set; }
        public byte[] reportPdf { get; set; }
        public string reportContentType { get; set; }
        public string reportFileName { get; set; }


    }
}

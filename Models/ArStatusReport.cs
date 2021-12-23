using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ArStatusReport
    {
        public int unArStatusId { get; set; }
        public int reportId { get; set; }
        

        public List<Bill> arBills { get; set; }


        public string reportFileUrlLocation { get; set; }
        public string reportBase64 { get; set; }
        public byte[] reportPdf { get; set; }
        public string reportContentType { get; set; }
        public string reportFileName { get; set; }
    }
}

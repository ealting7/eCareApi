using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class MedicalCode
    {
        public string CodeType { get; set; }
        public int? CodeId { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string MediumDescription { get; set; }
        public string LongDescription { get; set; }

        public string DisplayDescription { get; set; }

        public bool referralCode { get; set; }
        public string referralNumber { get; set; }

        public string searchParam { get; set; }

        public Guid usr { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Utilization
    {
        public string returnMsg { get; set;  }

        public string? referralNumber { get; set; }
        public Guid? memberId { get; set; }

        public int? referralTypeId { get; set; }
        public string? referralType { get; set; }
        public int? referralContextId { get; set; }
        public int? referralCategoryId { get; set; }
        public int? referralReasonId { get; set; }

        public DateTime? startDate { get; set; }
        public string displayStartDate { get; set; }
        public DateTime? endDate { get; set; }
        public string displayEndDate { get; set; }

        public Guid? referredByPcpId { get; set; }
        public string referredByPcpName { get; set; }
        public string referredByPcpNpi { get; set; }
        public bool referredByPcpRemove { get; set; }

        public Guid? referredToPcpId { get; set; }
        public string referredToPcpName { get; set; }
        public string referredToPcpNpi { get; set; }
        public bool referredToPcpRemove { get; set; }


        public string codeType { get; set; }
        public bool removeCode { get; set; }
        public List<MedicalCode> diagnosisCodes { get; set; }
        public List<MedicalCode> cptCodes { get; set; }
        public List<MedicalCode> hcpcsCodes { get; set; }

    }
}

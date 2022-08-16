using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Utilization
    {
        public string returnMsg { get; set; }

        public string? referralNumber { get; set; }
        public Guid? memberId { get; set; }

        public Patient patient { get; set; }        


        public string authNumber { get; set; }

        public int? referralTypeId { get; set; }
        public string? referralType { get; set; }
        public string utilizationType { get; set; }
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

        public int? referredToFacilityId { get; set; }
        public string referredToFacilityName { get; set; }
        public string referredToFacilityNpi { get; set; }
        public bool referredToFacilityRemove { get; set; }


        public string codeType { get; set; }
        public bool removeCode { get; set; }
        public List<MedicalCode> diagnosisCodes { get; set; }
        public List<MedicalCode> cptCodes { get; set; }
        public List<MedicalCode> hcpcsCodes { get; set; }


        public List<UtilizationWorkflow> actions { get; set; }

        public List<UtilizationItem> utilizations { get; set; }

        public List<Letter> letters { get; set; }

        public List<Note> notes { get; set; }

        public List<Saving> savings { get; set; }

        public List<Fax> faxes { get; set; }

        public List<UtilizationRequest> requests { get; set; }

        public HospitalFacility referredToFacility { get; set; }

        public MedicalReview mdReviewRequest { get; set; }



        public Guid userId { get; set; }
        public DateTime creationDate { get; set; }

    }
}

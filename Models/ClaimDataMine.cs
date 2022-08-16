using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class ClaimDataMine
    {
        public List<Tpa> tpas { get; set; }

        public int claimId { get; set; }

        public string diag1 { get; set; }
        public string diag1Desc { get; set; }
        public string diag2 { get; set; }
        public string diag2Desc { get; set; }
        public string diag3 { get; set; }
        public string diag3Desc { get; set; }
        public string diag4 { get; set; }
        public string diag4Desc { get; set; }
        public string diag5 { get; set; }

        public string cpt { get; set; }
        public string hcpcs { get; set; }

        public DateTime? serviceDate { get; set; }
        public string posName { get; set; }

        public decimal? paidAmount { get; set; }

        public string providerTin { get; set; }
        public string providerName { get; set; }


        public Guid? patientId { get; set; }
        public string patientSsn { get; set; }
        public DateTime? patientBirth { get; set; }
        public string claimantFirstName { get; set; }
        public string claimantLastName { get; set; }

        public bool? inLcm { get; set; }
        public bool? inDm { get; set; }


        public string employerName { get; set; }
        public string tpaName { get; set; }


        public int? tpaId { get; set; }
        public DateTime downloadStartDate { get; set; }
        public DateTime downloadEndDate { get; set; }
        public decimal minimumPaidAmount { get; set; }
        public bool removeLcmPatients { get; set; }
        public bool removeLcmTriggers { get; set; }
        public bool removeInactivePatients { get; set; }
        public bool removeOlderThanPatients { get; set; }
        public int removeOlderThanAge { get; set; }


        public Guid? usr { get; set; }
    }
}

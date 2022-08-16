using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Saving
    {
        public int savingsId { get; set; }

        public string savingsType { get; set; }

        public Guid guidSavingsId { get; set; }

        public Guid? memberId { get; set; }
        public string? referralNumber { get; set; }

        public string? referralType { get; set; }

        public int? utilizationLineNumber { get; set; }

        public string? itemDescription { get; set; }
        public int? savingUnitsId { get; set; }
        public string? savingUnits { get; set; }
        public decimal? quantity { get; set; }
        public string? displayQuantity { get; set; }
        public decimal? cost { get; set; }
        public string? displayCost { get; set; }
        public decimal? negotiated { get; set; }
        public string? displayNegotiated { get; set; }
        public decimal? savings { get; set; }
        public string? displaySavings { get; set; }

        public DateTime? createdDate { get; set; }
        public string? displayCreatedDate { get; set; }

        public Guid? usr { get; set; }
    }
}

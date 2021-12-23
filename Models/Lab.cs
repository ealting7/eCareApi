using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Lab
    {
        public int labId { get; set; }
        public string? labName { get; set; }

        public string patient { get; set; }

        public string? accessionNumber { get; set; }

        public DateTime? collectionDate { get; set; }
        public string displayCollectionDate { get; set; }

        public string collectionSite { get; set; }

        public IList<MedicalCode>? labTypeCpts { get; set; }
        public IList<MedicalCode>? labTypeHcpcs { get; set; }
    }
}

using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class LabResult
    {

        public int admissionId { get; set; }

        public int labId { get; set; }

        public int resultId { get; set; }
        public int orderId { get; set; }

        public DateTime? collectionDate { get; set; }

        public string result { get; set; }

        public int? flagId { get; set; }
        public string flagName { get; set; }

        public bool? isPositive { get; set; }


        public string createDate { get; set; }
        public Guid? usr { get; set; }
    }
}

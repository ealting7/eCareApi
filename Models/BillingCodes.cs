using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class BillingCodes
    {
        public int billingCodeId { get; set; }

        public string? billingCode { get; set; }

        public string? billingDescription { get; set; }

        public string displayCodeDescription { get; set; }
    }
}

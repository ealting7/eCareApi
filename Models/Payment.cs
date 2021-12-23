using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Payment
    {
        public int paymentId { get; set; }

        public string? invoiceId { get; set; }

        public string billType { get; set; }

        public Guid? patientId { get; set; }

        public decimal billTotal { get; set; }

        public string? checkNumber { get; set; }

        public DateTime? paymentDate { get; set; }
        public string paymentDateDisplay { get; set; }

        public decimal? paymentAmount { get; set; }

        public Guid? paymentUserId { get; set; }

        public string paymentUserName { get; set; }

        public decimal? balanceAfterPayment { get; set; }

        public byte? billPaidOff { get; set; }

        public DateTime? creationDate { get; set; }

        public DateTime? receivedDate { get; set; }

    }
}

using eCareApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Models
{
    public class Bill
    {
        public Guid billId { get; set; }

        public string? lcmInvoiceNumber { get; set; }


        public int billingBackupId { get; set; }
        public string billingBackupPeriodId { get; set; } 


        public Patient? billingPatient { get; set; }

        public Guid? memberId { get; set; }
        public string? patientFullName { get; set; }
        public string? employer { get; set; }
        public string employerAddress { get; set; }
        public string employerCityStateZip { get; set; }

        public decimal? employerBillingRate { get; set; }
        public string? tpa { get; set; }
        public DateTime? recordDate { get; set; }
        public string displayRecordDate { get; set; }
        public int? billCode { get; set; }
        public string billingCode { get; set; }
        public string billCodeDescription { get; set; }
        public double? billMinutes { get; set; }


        public decimal? billTotalAmount { get; set; }
        public string displayBillTotalAmount { get; set; }
        public decimal? billTotalPaymentAmount { get; set; }
        public decimal? billRemainingBalance { get; set; }

        public string caseOwner { get; set; }

        public Guid? userId { get; set; }

        public string? billNote { get; set; }
        public bool? billDisabled { get; set; }
        public DateTime? billSentDate { get; set; }

        public DateTime? billingStartDate { get; set; }
        public DateTime? billingEndDate { get; set; }



        public List<BillingCodes>? lcmBillingCodes { get; set; }

        public List<BillingBackup>? backupPeriods { get; set; } 

        public List<Payment>? payments { get; set; }



        public string invoiceFileUrlLocation { get; set; }
        public string invoiceBase64 { get; set; }
        public byte[]? invoicePdf { get; set; }
        public string invoiceContentType { get; set; }
        public string invoiceFileName { get; set; }


        public byte[]? invoice2Pdf { get; set; }
        public string invoice2Base64 { get; set; }
        public string invoice2ContentType { get; set; }
        public string invoice2FileName { get; set; }

        public string invoice2Html { get; set; }
    }
}

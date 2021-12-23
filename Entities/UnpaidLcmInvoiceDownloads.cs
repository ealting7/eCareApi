using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("UNPAID_LCM_INVOICE_DOWNLOADS")] 
    public class UnpaidLcmInvoiceDownloads
    {
        public int? unpaid_id { get; set; }
        public int? report_id { get; set; }
        public int? tpa_id { get; set; }
        public int? employer_id { get; set; }
        public string? group_num { get; set; }
        public string? participant_id { get; set; }
        public string? claimant_last_name { get; set; }
        public string? claimant_first_name { get; set; }
        public string? claim_number { get; set; }
        public string? procedure { get; set; }
        public string? prov_loc { get; set; }
        public string? status { get; set; }
        public decimal? charge { get; set; }
        public decimal? total { get; set; }
        public string? service_date { get; set; }
        public string? paid_date { get; set; }
        public string? complete { get; set; }
        public byte? invoice_found { get; set; }
        public int billing_backup_id { get; set; }
        public byte? error { get; set; }
        public string? error_msg { get; set; }

    }
}

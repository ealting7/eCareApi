using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("ACCOUNTS_RECEIVABLE_PAYMENTS")] 
    public class AccountsReceivablePayments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int accounts_receivable_payment_id { get; set; }
        public Guid? member_id { get; set; }
        public string? invoice_id { get; set; }
        public string? bill_type { get; set; }
        public decimal? bill_total { get; set; }
        public string? check_number { get; set; }
        public DateTime? payment_date { get; set; }
        public decimal? payment_amount { get; set; }
        public DateTime? credit_date { get; set; }
        public decimal? credit_amount { get; set; }
        public DateTime? balance_outstanding_date { get; set; }
        public decimal? balance_outstanding { get; set; }
        public Guid? payment_user_id { get; set; }
        public string? comment { get; set; }
        public byte? bill_paid_off { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? um_record_id { get; set; }
        public DateTime? received_date { get; set; }
        public Guid? ccm_record_id { get; set; }
        public decimal? debit_amount { get; set; }
        public DateTime? debit_date { get; set; }
    }
}

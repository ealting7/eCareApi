using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("UM_BILLING_AUTO_GENERATE_INVOICES")] 
    public class UmBillingAutoGenerateInvoices
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int um_billing_auto_generate_invoices_id { get; set; }

         public int? employer_id { get; set; }
        public int? tpa_id { get; set; }
        public string entity_name { get; set; }
        public int? member_count { get; set; }
        public decimal? bill_rate { get; set; }
        public DateTime? last_ran_date { get; set; }
        public Guid? last_ran_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public string last_email_address { get; set; }
        public byte? uses_eligibility_count { get; set; }
        public byte? use_tpa_invoice { get; set; }
        public string email_cc_list { get; set; }
        public Guid? email_cc_list_update_user_id { get; set; }
        public DateTime? email_cc_list_update_date { get; set; }
        public int? employer_id_for_default_address { get; set; }
    }
}

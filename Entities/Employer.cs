using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    
    [Table("EMPLOYER")]
    public class Employer
    {
        [Key]
        public int employer_id { get; set; }

        [Required]
        [StringLength(50)]
        public string employer_name { get; set; }

        [StringLength(50)]
        public string DBA { get; set; }

        [StringLength(50)]
        public string employer_web_site_url { get; set; }

        [StringLength(50)]
        public string employer_code { get; set; }

        [StringLength(50)]
        public string employer_tax_id { get; set; }

        public bool? active_flag { get; set; }

        public Guid? user_updated { get; set; }

        public DateTime? date_updated { get; set; }

        [StringLength(50)]
        public string employer_email { get; set; }

        [StringLength(50)]
        public string rptperiod_schedule { get; set; }

        public decimal? percent_savings { get; set; }

        public int? num_of_employees { get; set; }

        [StringLength(50)]
        public string business_type { get; set; }

        public int? employer_parent_id { get; set; }

        public bool? lcm_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal lcm_billing_rate { get; set; }

        [StringLength(10)]
        public string lcm_bill_by { get; set; }

        public bool? create_data_mined_claim_tasks { get; set; }

        public bool? std_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? std_billing_rate { get; set; }

        [StringLength(10)]
        public string std_bill_by { get; set; }

        public bool? wc_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? wc_billing_rate { get; set; }

        [StringLength(10)]
        public string wc_bill_by { get; set; }

        public bool? use_employer_logo { get; set; }

        [Column(TypeName = "money")]
        public decimal? cr_physician_review_rate { get; set; }

        [Column(TypeName = "money")]
        public decimal? cr_nurse_review_rate { get; set; }

        [Column(TypeName = "money")]
        public decimal? cr_other_markup_percent { get; set; }

        [Column(TypeName = "money")]
        public decimal? std_flat_rate { get; set; }

        public byte? std_flat_rate_billing { get; set; }

        public byte? uses_consult_a_doc { get; set; }

        public byte? um_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? um_billing_rate { get; set; }

        [StringLength(20)]
        public string um_bill_by { get; set; }

        public int? um_bill_count { get; set; }

        public byte? uses_medicare_part_a { get; set; }

        public byte? uses_medicare_part_b { get; set; }

        public byte? uses_medicare_part_d { get; set; }

        public Guid? lastupdate_userid { get; set; }

        public DateTime? lastupdate_date { get; set; }

        public byte? is_test_employer { get; set; }

        public byte? incentive_eligible { get; set; }

        public Guid? dm_manager_user_id { get; set; }

        public Guid? cm_manager_user_id { get; set; }

        public int? incentive_employer_reason_id { get; set; }

        public byte? dm_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? dm_billing_rate { get; set; }

        [StringLength(10)]
        public string dm_bill_by { get; set; }

        public byte? ccm_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? ccm_billing_rate { get; set; }

        [StringLength(20)]
        public string ccm_bill_by { get; set; }

        public int? ccm_bill_count { get; set; }

        public byte? care_coordination_billable { get; set; }

        [Column(TypeName = "money")]
        public decimal? care_coordination_billing_rate { get; set; }

        public byte? um_bill_popup_for_no_um { get; set; }

        public byte? um_24_7_rn { get; set; }
    }
}

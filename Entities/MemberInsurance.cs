using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_INSURANCE")]
    public class MemberInsurance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_insurance_id { get; set; }
        public Guid? member_id { get; set; }
        public string? insurance_name { get; set; }
        public string? insurance_id { get; set; }
        public string? insurance_group_number { get; set; }
        public string? insurance_plan_number { get; set; }
        public DateTime? effective_date { get; set; }
        public DateTime? termination_date { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public string? subscriber_first_name { get; set; }
        public string? subscriber_last_name { get; set; }
        public string? subscriber_middle_name { get; set; }
        public DateTime? subscriber_birth { get; set; }
        public string? subscriber_gender { get; set; }
        public string? subscriber_address { get; set; }
        public string? subscriber_city { get; set; }
        public string? subscriber_state_abbrev { get; set; }
        public string? subscriber_employer_or_school { get; set; }
        public decimal? yearly_dollar_limit { get; set; }
        public decimal? deductible_amount { get; set; }
        public bool? copay { get; set; }
        public decimal? coverage_percentage { get; set; }
        public int? insurance_relationship_id { get; set; }
        public int? subscriber_country_id { get; set; }
        public string? insurance_phone_number { get; set; }
        public bool? self_pay { get; set; }
        public bool? uninsured { get; set; }
        public bool? copay_flat_fee { get; set; }
        public bool? copay_percentage { get; set; }
        public decimal? copay_office_visit_amount { get; set; }
        public decimal? copay_specialist_visit_amount { get; set; }
        public decimal? copay_er_visit_amount { get; set; }
        public decimal? copay_generic_rx_amount { get; set; }
        public decimal? copay_percentage_amount { get; set; }
        public bool? is_medicare { get; set; }
        public bool? is_medicaid { get; set; }
    }
}



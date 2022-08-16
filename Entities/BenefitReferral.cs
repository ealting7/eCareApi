using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("BENEFIT_REFERRAL")] 
    public class BenefitReferral
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int benefit_id { get; set; }

        public int? rx_program_id { get; set; }
        public string? plan_type { get; set; }
        public string? benefit_stoploss { get; set; }
        public string? benefit_penalty { get; set; }
        public DateTime? benefit_renewal { get; set; }
        public string? benefit_limitation { get; set; }
        public string? benefit_prevent { get; set; }
        public string? benefit_notes { get; set; }
        public string? ref_penalty_noref { get; set; }
        public string? ref_penalty_no_network { get; set; }
        public bool? ref_prior_prog { get; set; }
        public string? ref_notes { get; set; }
        public int? employer_id { get; set; }
        public int? reinsurer_id { get; set; }
    }
}

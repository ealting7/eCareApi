using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_UTILIZATION_SAVINGS")] 
    public class rUtilizationSavings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int r_utilization_savings_id { get; set; }

        public Guid utilization_savings_id { get; set; }
        public Guid? member_id { get; set; }
        public string? referral_number { get; set; }
        public string? referral_type { get; set; }
        public int? line_number { get; set; }
        public int? savings_line { get; set; }
        public string? item_description { get; set; }
        public int? saving_units_id { get; set; }
        public decimal? quantity { get; set; }
        public decimal? cost { get; set; }
        public decimal? negotiated { get; set; }
        public decimal? savings { get; set; }
        public string? dollar_or_percent { get; set; }
        public bool? line_item { get; set; }
        public string? cpt_code { get; set; }
        public int? network_id { get; set; }
        public string? notes { get; set; }
        public Guid? system_user_id { get; set; }
        public DateTime? date_updated { get; set; }
        public bool? delete_flag { get; set; }

    }
}

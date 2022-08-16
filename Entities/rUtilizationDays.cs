using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_UTILIZATION_DAYS")] 
    public class rUtilizationDays
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatCaseActID { get; set; }

        public Guid member_id { get; set; }
        public string referral_number { get; set; }
        public string referral_type { get; set; }
        public int line_number { get; set; }
        public int? type_id { get; set; }
        public bool surgery_flag { get; set; }
        public bool surgery_on_first_day_flag { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int? util_decision_id { get; set; }
        public DateTime? next_review_date { get; set; }
        public int? number_of_days { get; set; }
        public bool visits_recurring_flag { get; set; }
        public int? visits_num_per_period_requested { get; set; }
        public int? visits_num_per_period_authorized { get; set; }
        public string? visits_period_requested { get; set; }
        public string? visits_period_authorized { get; set; }
        public int? visits_num_periods_requested { get; set; }
        public int? visits_num_periods_authorized { get; set; }
        public DateTime? visits_authorized_end_date { get; set; }
        public DateTime? visits_authorized_start_date { get; set; }
        public int? denial_reason_id { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? ICM_Units { get; set; }
        public DateTime? Date_Created { get; set; }
        public int? sands_referral_status_code_id { get; set; }
        public byte? std_billed { get; set; }
        public DateTime? std_billed_date { get; set; }
        public bool? removed { get; set; }
        public DateTime? removed_date { get; set; }
        public Guid? removed_user_id { get; set; }

    }
}

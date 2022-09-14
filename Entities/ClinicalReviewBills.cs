using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("CLINICAL_REVIEW_BILLS")]
    public class ClinicalReviewBills
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int cr_bill_id { get; set; }

        public Guid? member_id { get; set; }
        public string? referral_number { get; set; }
        public string? description { get; set; }
        public string? type_of_review { get; set; }
        public string? other_type_of_review { get; set; }
        public decimal? review_cost { get; set; }
        public decimal? other_review_cost { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_user_id { get; set; }
        public byte? is_physician_review { get; set; }
        public byte? is_nurse_review { get; set; }
        public byte? is_other_review { get; set; }
        public byte? is_hospital_plan_bill { get; set; }
        public Guid? system_role_id { get; set; }
        public decimal? markup { get; set; }
    }
}

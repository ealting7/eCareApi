using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("LCM_BILLING_CODES")]
    public class LcmBillingCodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int billing_id { get; set; }
        public string? billing_code { get; set; }
        public string? billing_description { get; set; }
        public int? billing_time_in_min { get; set; }
        public byte? standard_code { get; set; }
        public int? standard_minutes { get; set; }
    }
}

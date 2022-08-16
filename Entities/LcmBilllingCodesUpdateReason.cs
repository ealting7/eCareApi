using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("LCM_BILLING_CODES_UPDATE_REASON")] 
    public class LcmBilllingCodesUpdateReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int lcm_billing_codes_update_reason_id { get; set; }

        public string reason { get; set; }
        public DateTime creation_date { get; set; }
        public Guid creation_user_id { get; set; }

    }
}

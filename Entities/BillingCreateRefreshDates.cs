using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("BILLING_CREATE_REFRESH_DATES")]    
    public class BillingCreateRefreshDates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int billing_create_refresh_dates_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public byte? most_recent_used_date { get; set; }
        public string? update_type { get; set; }
        public string? bill_type { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? system_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public Guid? update_date_userid { get; set; }
        public Guid? system_role_id { get; set; }
    }
}

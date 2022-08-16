using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_PROVIDER")] 
    public class SimsProvider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_provider_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string maiden_name { get; set; }
        public string provider_title { get; set; }
        public int? sims_provider_specialty_id { get; set; }
        public string provider_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public byte? disabled { get; set; }
        public string email_address { get; set; }
        public Guid? last_user_id { get; set; }
        public DateTime? last_update_date { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_NURSE")] 
    public class SimsNurse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_nurse_id { get; set; }

        public string first_name { get; set; }
        public string last_name { get; set; }
        public string maiden_name { get; set; }
        public string nurse_title { get; set; }
        public int? sims_nurse_specialty_id { get; set; }
        public int? nurse_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public byte? disabled { get; set; }
        public string email_address { get; set; }
        public Guid? last_user_id { get; set; }
        public DateTime? last_update_date { get; set; }

    }
}

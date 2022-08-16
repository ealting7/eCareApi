using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("TPA_EMAIL_PASSWORD")] 
    public class TpaEmailPassword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int tpa_email_password_id { get; set; }

        public int? tpa_id { get; set; }
        public string password { get; set; }
        public byte? billing_password { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
    }
}

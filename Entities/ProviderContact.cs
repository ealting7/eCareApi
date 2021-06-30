using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_CONTACT")] 
    public class ProviderContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_contact_id { get; set; }
        public string? contact_name { get; set; }
        public string? contact_title { get; set; }
        public string? contact_dept { get; set; }
        public string? contact_email { get; set; }
        public string? contact_notes { get; set; }
    }
}

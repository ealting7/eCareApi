using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_CONTACT_PHONE")] 
    public class ProviderContactPhone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_contact_phone_id { get; set; }
        
        public int provider_contact_id { get; set; }
        public int provider_phone_id { get; set; }
        public int? phone_type_id { get; set; }

    }
}

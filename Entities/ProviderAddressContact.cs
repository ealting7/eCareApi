using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_ADDRESS_CONTACT")] 
    public class ProviderAddressContact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_address_contact_id { get; set; }
        
        public int provider_address_id { get; set; }
        public int provider_contact_id { get; set; }
        public int? contact_type_id { get; set; }

    }
}

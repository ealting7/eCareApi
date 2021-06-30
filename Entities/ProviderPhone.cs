using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_PHONE")] 
    public class ProviderPhone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_phone_id { get; set; }
        public string? phone_number { get; set; }
        public string? extension { get; set; }
        public DateTime? creation_date { get; set; }
        public byte? auto_add { get; set; }
    }
}

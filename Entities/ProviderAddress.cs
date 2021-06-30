using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_ADDRESS")] 
    public class ProviderAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_address_id { get; set; }
        public Guid? pcp_id { get; set; }
        public string? address_line1 { get; set; }
        public string? address_line2 { get; set; }
        public string? city { get; set; }
        public string? state_abbrev { get; set; }
        public string? zip_code { get; set; }
        public string? address_note { get; set; }
        public int? address_county_id { get; set; }
        public int? address_type_id { get; set; }


    }
}

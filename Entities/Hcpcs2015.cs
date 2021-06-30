using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HCPCS_CODES_2015")]
    public class Hcpcs2015
    {

        [Key]
        public int hcpcs_codes_2015_id { get; set; }

        [StringLength(10)]
        public string? hcp_code { get; set; }

        [StringLength(100)]
        public string? hcpcs_short { get; set; }

        [StringLength(1000)]
        public string? hcpcs_full { get; set; }

        public byte? lcm_trigger { get; set; }

        public DateTime? lcm_trigger_date { get; set; }

        [StringLength(35)]
        public string? hcpcs_coverage { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PCP_TABLE")] 
    public class PcpTable
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pcp_table_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid pcp_id { get; set; }

        [StringLength(20)]
        public string? provider_num { get; set; }

        [StringLength(100)]
        public string? provider_group_name { get; set; }

        [StringLength(20)]
        public string? prov_type { get; set; }
        public bool? disable_flag { get; set; }
        public DateTime? date_updated { get; set; }
        public Guid? user_updated { get; set; }

        [StringLength(50)]
        public string? provider_first_name { get; set; }

        [StringLength(50)]
        public string? provider_middle_name { get; set; }

        [StringLength(50)]
        public string? provider_last_name { get; set; }

        [StringLength(15)]
        public string? provider_tin { get; set; }

        [StringLength(20)]
        public string? npi { get; set; }
        public int? Legacy_id { get; set; }
        public DateTime? npi_updated_date { get; set; }
        public byte? npi_update_via_reference { get; set; }
        public byte? wishard_file_load { get; set; }
        public byte? wishard_contracted_pmp { get; set; }

        [StringLength(9)]
        public string? ihcp { get; set; }

        [StringLength(50)]
        public string? medicaid_number { get; set; }

        [StringLength(200)]
        public string? wishard_physician_id { get; set; }

        [StringLength(1)]
        public string? location_id { get; set; }

        [StringLength(9)]
        public string? billing_ihcp { get; set; }

        [StringLength(10)]
        public string? billing_npi { get; set; }
        public byte? sands_file_load { get; set; }
        public byte? shpg_pmp { get; set; }
        public int? system_user_provider_specialty_id { get; set; }
        public string? shpg_provider_id { get; set; }
        public byte? do_not_delete { get; set; }
        public byte? do_delete { get; set; }

        [StringLength(100)]
        public string? email_address { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_SPECIALTYS")] 
    public class ProviderSpecialtys
    {
        public int specialty_id { get; set; }
        public Guid pcp_id { get; set; }
        public string? prov_type { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PROVIDER_SPECIALTYS")] 
    public class ProviderSpecialtys
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int provider_specialtys_id { get; set; }
        
        public int specialty_id { get; set; }
        public Guid pcp_id { get; set; }
        public string? prov_type { get; set; }
    }
}

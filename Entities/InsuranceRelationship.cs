using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("INSURANCE_RELATIONSHIP")]
    public class InsuranceRelationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int insurance_relationship_id { get; set; }
        public string? relationship_name { get; set; }
    }
}

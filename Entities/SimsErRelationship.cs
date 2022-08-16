using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_ER_RELATIONSHIP")] 
    public class SimsErRelationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_er_relationship_id { get; set; }

        public string? name { get; set; }
        public byte? is_spanish_relationship { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_SAVINGS_UNITS")] 
    public class rSavingsUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int saving_units_id { get; set; }
        public string? units_code { get; set; }
        public string? units_label { get; set; }
    }
}

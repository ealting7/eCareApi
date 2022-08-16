using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_DENIALREASON")] 
    public class rDenialReason
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string code { get; set; }
        public string label { get; set; }
        public int? listorder { get; set; }
    }
}

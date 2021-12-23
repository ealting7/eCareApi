using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_CURRENTSTATUS")]
    public class rCurrentStatus
    {
        public int id { get; set; }

        [Required]
        [StringLength(10)]
        public string code { get; set; }

        [Required]
        [StringLength(50)]
        public string label { get; set; }

        public int? listorder { get; set; }

    }
}

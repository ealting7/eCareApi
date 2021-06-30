using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("PHONE_TYPE")]
    public class PhoneType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int phone_type_id { get; set; }

        [StringLength(10)]
        public string code { get; set; }

        [StringLength(50)]
        public string label { get; set; }

        [StringLength(10)]
        public string? label_abbrev { get; set; }
    }
}

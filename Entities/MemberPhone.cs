using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_PHONE")]
    public class MemberPhone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_phone_id { get; set; }

        public Guid member_id { get; set; }

        public int phone_type_id { get; set; }

        [StringLength(20)]
        public string? phone_number { get; set; }

        [StringLength(500)]
        public string? phone_note { get; set; }
    }
}

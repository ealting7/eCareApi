using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_ADDRESS")]
    public class MemberAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_address_id { get; set; }



        public Guid member_id { get; set; }

        [StringLength(50)]
        public string address_line1 { get; set; }

        [StringLength(50)]
        public string address_line2 { get; set; }

        [StringLength(50)]
        public string apartment_number { get; set; }

        [StringLength(2)]
        public string state_abbrev { get; set; }

        [StringLength(32)]
        public string city { get; set; }

        [StringLength(18)]
        public string zip_code { get; set; }

        [StringLength(500)]
        public string address_note { get; set; }




        public int address_type_id { get; set; }

        public int? address_county_id { get; set; }

        public int? columbia_deptId { get; set; }



        public bool? is_alternate { get; set; }
    }
}

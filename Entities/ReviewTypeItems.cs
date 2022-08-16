using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("REVIEW_TYPE_ITEMS")] 
    public class ReviewTypeItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int review_type_items_id { get; set; }
        public int? review_type_id { get; set; }
        public string? name { get; set; }
        public byte? is_default { get; set; }
        public byte? is_dr { get; set; }
        public byte? is_third_party { get; set; }
        public Guid? icms_system_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? created_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public byte? disabled { get; set; }
        public byte? is_shpg { get; set; }
    }
}

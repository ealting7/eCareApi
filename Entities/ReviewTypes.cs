using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("REVIEW_TYPES")] 
    public class ReviewTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int review_type_id { get; set; }
        public int? name { get; set; }
        public byte? is_default { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? created_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public byte? disabled { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_ETHNICITY")]
    public class MemberEthnicity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_ethnicity_id { get; set; }
        public Guid? member_id { get; set; }
        public string? ethnicity { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }

    }
}

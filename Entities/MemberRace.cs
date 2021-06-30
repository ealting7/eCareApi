using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_RACE")]
    public class MemberRace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_race_id { get; set; }

        public Guid member_id { get; set; }
        public int hospital_race_ID { get; set; }
        public bool? disabled { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
    }
}

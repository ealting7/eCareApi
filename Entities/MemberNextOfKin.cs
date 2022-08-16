using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_NEXT_OF_KIN")]
    public class MemberNextOfKin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_next_of_kin_id { get; set; }
        public Guid member_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int sims_er_relationship_id { get; set; }
        public string? phone_number { get; set; }
        public string? email_address { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }

    }
}

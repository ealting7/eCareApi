using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_MEDS_ALLERGIES")] 
    public class MemberMedsAllergies
    {

        [Key]
        public int member_meds_allergies_id { get; set; }
        public Guid member_id { get; set; }
        public string descr { get; set; }
        public bool deleted_flag { get; set; }
        public Guid? user_deleted { get; set; }
        public DateTime? date_deleted { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public byte? added_via_portal { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public byte? updated_via_portal { get; set; }
        public string issue_type { get; set; }

    }
}

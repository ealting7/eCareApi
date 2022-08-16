using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_PROGRAM")] 
    public class MemberProgram
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_program_id { get; set; }

        public Guid member_id { get; set; }
        public bool? cm_optout { get; set; }
        public bool? dm_optout { get; set; }
        public bool? lcm_optout { get; set; }
        public bool? lifestyle_optout { get; set; }
        public DateTime? cm_optout_date { get; set; }
        public DateTime? dm_optout_date { get; set; }
        public DateTime? lcm_optout_date { get; set; }
        public DateTime? lifestyle_optout_date { get; set; }
        public Guid? last_userid { get; set; }
        public bool? lcm_sr_optout { get; set; }
        public DateTime? lcm_sr_optout_date { get; set; }
        public byte? ccm_optout { get; set; }
        public DateTime? ccm_optout_date { get; set; }
        public byte? care_coordination_optout { get; set; }
        public DateTime? care_coordination_optout_date { get; set; }
}
}

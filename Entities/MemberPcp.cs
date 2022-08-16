using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace eCareApi.Entities
{
    [Table("MEMBER_PCP")] 
    public class MemberPcp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_pcp_id { get; set; }

        public Guid member_id { get; set; }
        public Guid pcp_id { get; set; }
        public DateTime pcp_eff_date { get; set; }
        public DateTime? pcp_term_date { get; set; }

    }
}

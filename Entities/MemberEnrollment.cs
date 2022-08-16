using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_ENROLLMENT")]
    public class MemberEnrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_enrollment_id { get; set; }

        public int claims_system_id { get; set; }
        public Guid member_id { get; set; }
        public string claims_id { get; set; }
        public DateTime member_effective_date { get; set; }
        public int client_id { get; set; }
        public int client_bu_id { get; set; }
        public DateTime? member_disenroll_date { get; set; }
        public int? disenroll_reason_id { get; set; }
        public string? member_type_code { get; set; }
        public bool manual_entry_flag { get; set; }
        public Guid? user_updated { get; set; }
        public DateTime? date_updated { get; set; }
        public string? claims_enrollment_id { get; set; }
        public int? employer_id { get; set; }
        public int? employer_division_id { get; set; }
        public string? DEP_Number { get; set; }
        public int? JUST_ADDED { get; set; }
        public int? hospital_id { get; set; }
        public int? old_employer_id { get; set; }
        public int? eps_id { get; set; }
        public string? columbia_employer_name { get; set; }
        public string? egp_member_id { get; set; }
        public int? employer_type_id { get; set; }

    }
}

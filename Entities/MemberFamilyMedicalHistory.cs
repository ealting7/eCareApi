﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_FAMILY_MEDICAL_HISTORY")] 
    public class MemberFamilyMedicalHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_family_medical_history_id { get; set; }
        public Guid member_id { get; set; }
        public string? family_history { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_ate { get; set; }
        public Guid? last_update_user_id { get; set; }

    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_DISEASE_CONDITION_CAREPLAN")]    
    public class MemberDiseaseConditionCareplan
    {
        [Key]
        public int member_disease_condition_careplan_id { get; set; }
        public int member_disease_condition_reference_id { get; set; }
        public string careplan_name { get; set; } 
        public DateTime? start_date { get; set; }
        public DateTime? completion_date { get; set; }
        public DateTime? creation_date { get; set; }


    }
}

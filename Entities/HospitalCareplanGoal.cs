using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_CAREPLAN_GOAL")]    
    public class HospitalCareplanGoal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_careplan_goal_id { get; set; }
         public string goal_measure { get; set; }
        public DateTime? creation_date { get; set; }
    }
}

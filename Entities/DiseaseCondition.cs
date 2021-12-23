using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("DISEASE_CONDITION")]
    public class DiseaseCondition
    {
        [Key]
        public int disease_condition_id { get; set; }
        public string descr { get; set; }
        public bool disable_flag { get; set; }
        public Guid? user_updated { get; set; }
        public DateTime? date_updated { get; set; }

    }
}

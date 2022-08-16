using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_UTILIZATION_VISIT_PERIOD")] 
    public class rUtilizationVisitPeriod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int r_utilization_visit_period_id { get; set; }
        public string? label { get; set; }
        public string? visit_period_abbrev { get; set; }
    }
}

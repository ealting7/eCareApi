using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CHART")] 
    public class HospitalInpatientAdmissionChart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_chart_id { get; set; }
        public string chart_name { get; set; }
        public int? display_order { get; set; }
        public string chart_table_name { get; set; }
        public string chart_type { get; set; }
        public string rationale { get; set; }


    }
}

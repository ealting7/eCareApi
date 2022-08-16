using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CHART_SOURCE_ANSWER")] 
    public class HospitalInpatientAdmissionChartSourceAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_chart_source_answer_id { get; set; }

        public int hospital_inpatient_admission_chart_source_id { get; set; }
        public int hospital_inpatient_admission_id { get; set; }
        public DateTime creation_date { get; set; }
        public Guid creation_user_id { get; set; }
        public int hour { get; set; }
        public DateTime date { get; set; }
        public decimal? answer_decimal { get; set; }
        public int? answer_int { get; set; }
        public DateTime? answer_datetime { get; set; }
        public bool? answer_bit { get; set; }
        public string answer_text_small { get; set; }
        public string answer_text_medium { get; set; }
        public string answer_text_large { get; set; }

    }
}

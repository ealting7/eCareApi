using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CHART_SOURCE")]
    public class HospitalInpatientAdmissionChartSource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_chart_source_id { get; set; } 
        public int hospital_inpatient_admission_chart_id { get; set; }
        public string source_name { get; set; }
        public string? source_name_abbrev { get; set; }        
        public int? display_order { get; set; }
        public string source_control_type { get; set; }
        public string source_control_class { get; set; }
        public string source_model_variable { get; set; }
        public string source_model_type { get; set; }
        public string source_control_loader_table { get; set; }
        public string source_control_loader_id_column { get; set; }
        public string source_control_loader_description_column { get; set; }
        public int? max_length { get; set; }
        public string? pattern { get; set; }
        public string? placeholder { get; set; }
        public int? textarea_rows { get; set; }
        public int? textarea_cols { get; set; }
        public string source_control_groupname { get; set; }
        public string source_control_label { get; set; }
        public bool? source_control_checked { get; set; }
        public string source_control_value { get; set; }
        public int? source_control_min { get; set; }
        public int? source_control_max { get; set; }
        public decimal? source_control_step { get; set; }

    }
}

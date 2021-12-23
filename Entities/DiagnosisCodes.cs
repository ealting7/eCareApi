using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("DIAGNOSIS_CODES")] 
    public class DiagnosisCodes
    {
        [Key]
        public int Dx_ID { get; set; }
        public string Diagnosis_Code { get; set; }
        public string? Diagnosis_Descr { get; set; }
        public bool? Surgical_Code { get; set; }
        public bool? disable_code { get; set; }
        public bool? lcm_trigger { get; set; }
        public DateTime? lcm_trigger_date { get; set; }
        public string? icd9_group_description { get; set; }
        public bool? stop_loss_trigger { get; set; }
        public DateTime? stop_loss_trigger_date { get; set; }
        public byte? auto_approval { get; set; }
        public Guid? code_added_by { get; set; }
        public DateTime? code_added_by_date { get; set; }

    }
}

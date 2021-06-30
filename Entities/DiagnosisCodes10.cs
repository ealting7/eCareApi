using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("DIAGNOSIS_CODES_10")]
    public class DiagnosisCodes10
    {
        [Key]
        public int diagnosis_codes_10_id { get; set; }

        [StringLength(10)]
        public string diagnosis_code { get; set; }

        [StringLength(50)]
        public string short_description { get; set; }

        [StringLength(100)]
        public string medium_description { get; set; }

        [StringLength(500)]
        public string long_description { get; set; }

        public byte? lcm_trigger { get; set; }

        public DateTime? lcm_trigger_date { get; set; }

        [StringLength(100)]
        public string icd10_group_description { get; set; }

        public byte? stop_loss_trigger { get; set; }

        public DateTime? stop_loss_trigger_date { get; set; }

        public byte? auto_approval { get; set; }

        public Guid? code_added_by { get; set; }

        public DateTime? code_added_date { get; set; }

        public int? icd_9_cross_reference_id { get; set; }
    }
}

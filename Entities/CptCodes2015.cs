using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("CPT_CODES_2015")]
    public class CptCodes2015
    {

        [Key]
        public int cpt_codes_2015_id { get; set; }

        [StringLength(10)]
        public string? cpt_code { get; set; }

        [StringLength(35)]
        public string? short_descr { get; set; }

        [StringLength(55)]
        public string? medium_descr { get; set; }

        [StringLength(2000)]
        public string? cpt_descr { get; set; }

        public byte? lcm_trigger { get; set; }

        public DateTime? lcm_trigger_date { get; set; }

        [StringLength(100)]
        public string? cpt_group_description { get; set; }

        public Guid? code_added_by { get; set; }

        public DateTime? code_added_date { get; set; }
    }
}

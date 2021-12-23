using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("TPA_EMPLOYER")]
    public partial class TpaEmployer
    {
        public int employer_id { get; set; }

        public int tpa_id { get; set; }

        public DateTime effective_date { get; set; }

        public DateTime? termination_date { get; set; }

        public bool? disabled { get; set; }
    }
}

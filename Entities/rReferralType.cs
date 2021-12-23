using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_REFERRALTYPE")]
    public partial class rReferralType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string code { get; set; }

        public string? label { get; set; }

        public int? listorder { get; set; }

        public byte? disabled { get; set; }

        public byte? allow_discharge { get; set; }

        public string? spanish_name { get; set; }

        public string? inpatient_outpatient_type { get; set; }
    }

}

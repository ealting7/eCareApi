using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_REFERRALCONTEXT")]
    public partial class rReferralContext
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        public string code { get; set; }

        public string label { get; set; }

        public int? listorder { get; set; }

        public string? spanish_name { get; set; }

        public byte? allow_retro_review { get; set; }
    }

    

}

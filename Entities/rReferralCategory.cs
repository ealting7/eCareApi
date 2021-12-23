using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("r_REFERRALCATEGORY")]
    public partial class rReferralCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int referral_category { get; set; }

        public string? code { get; set; }

        public string? label { get; set; }

        public string? spanish_name { get; set; }
    }
}

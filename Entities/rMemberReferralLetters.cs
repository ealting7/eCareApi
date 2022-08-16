using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_MEMBER_REFERRAL_LETTERS")] 
    public class rMemberReferralLetters
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
         public Guid? member_id { get; set; }
        public string referral_number { get; set; }
        public string file_identifier { get; set; }
        public byte[]? file_blob { get; set; }
        public DateTime? letter_created { get; set; }
        public Guid? create_user_id { get; set; }

        public bool? removed { get; set; }
        public DateTime? removed_date { get; set; }
        public Guid? removed_user_id { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("EMAILS_OUTBOUND")] 
    public class EmailsOutbound
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int email_outbound_id { get; set; }

        public Guid? user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public string email_to { get; set; }
        public string email_subject { get; set; }
        public string email_address { get; set; }
        public string email_message { get; set; }
        public int? email_type_id { get; set; }
        public byte? email_sent { get; set; }
        public DateTime? email_sent_date { get; set; }
        public byte[]? file_blob { get; set; }
        public string file_identifier { get; set; }
        public string zip_file_password { get; set; }
        public string zip_file_name { get; set; }
        public int? lcn_case_number { get; set; }
        public int? lcm_followup_id { get; set; }
        public string email_cc_list { get; set; }
        public string referral_number { get; set; }
        public Guid? member_id { get; set; }
        public string notice_type { get; set; }
    }
}

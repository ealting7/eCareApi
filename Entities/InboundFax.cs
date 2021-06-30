using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_INBOUND_FAX")]
    public class InboundFax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? fax_handle { get; set; }
        public int? fax_docfiles_handle { get; set; }
        public Guid? fax_serverguid { get; set; }
        public int? fax_boardsrv { get; set; }
        public int? fax_notifychannel { get; set; }
        public DateTime? fax_creationtime { get; set; }
        public string? fax_remoteid { get; set; }
        public string? fax_uniqueid { get; set; }
        public string? fax_bodyfilename { get; set; }
        public int? fax_numpages { get; set; }
        public byte[]? fax_image { get; set; }
        public bool? ready_flag { get; set; }
        public int? faxqueue_id { get; set; }
        public DateTime? opened_by_date { get; set; }
        public Guid? opened_by_user_id { get; set; }
        public Guid? assigned_to_user_id { get; set; }
        public DateTime? assigned_to_user_date { get; set; }
        public int? priority_level { get; set; }
        public DateTime? assigned_to_user_opened_date { get; set; }
        public DateTime? to_be_completed_date { get; set; }
        public DateTime? completed_date { get; set; }
        public string? queue_dummy { get; set; }
        public Guid? assigned_by_user_id { get; set; }
        public Guid? completed_by_user_id { get; set; }
        public Guid? member_id { get; set; }
        public string? referral_number { get; set; }
        public bool deleted_flag { get; set; }
        public string? error_description { get; set; }
        public string? email_filename { get; set; }
        public int? email_faxhandle { get; set; }
        public string? inbound_member_name { get; set; }
        public int? fax_type_id { get; set; }
        public DateTime? fax_type_date { get; set; }
        public DateTime? fax_dos_date { get; set; }
        public DateTime? fax_dos { get; set; }

    }
}

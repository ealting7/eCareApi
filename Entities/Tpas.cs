using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("TPAS")]
    public partial class Tpas
    {
        [Key]
        public int tpa_id { get; set; }

        [StringLength(255)]
        public string tpa_name { get; set; }

        [StringLength(255)]
        public string tpa_dba { get; set; }

        [StringLength(50)]
        public string tpa_code { get; set; }

        [StringLength(50)]
        public string tpa_work_comp { get; set; }

        [StringLength(15)]
        public string tpa_tax_id { get; set; }

        public int? precert_type { get; set; }

        [StringLength(255)]
        public string precert_folder { get; set; }

        public int? precert_range { get; set; }

        [StringLength(255)]
        public string precert_ftp_url { get; set; }

        [StringLength(50)]
        public string precert_ftp_username { get; set; }

        [StringLength(50)]
        public string precert_ftp_pwd { get; set; }

        [StringLength(255)]
        public string precert_ftp_remote { get; set; }

        [StringLength(255)]
        public string precert_pgpkey_name { get; set; }

        [StringLength(500)]
        public string tpa_notes { get; set; }

        public bool? disable_flag { get; set; }

        public DateTime? date_updated { get; set; }

        public Guid? user_updated { get; set; }

        public int? PrecertParentCarrierID { get; set; }

        public bool? UsePGP { get; set; }

        public bool? UseFTP { get; set; }

        [StringLength(50)]
        public string PGPHexKey { get; set; }

        [StringLength(255)]
        public string PGPPubRing { get; set; }

        [StringLength(255)]
        public string PGPPrivKey { get; set; }

        [StringLength(15)]
        public string UserName { get; set; }

        [StringLength(15)]
        public string Password { get; set; }

        [StringLength(255)]
        public string auto_report_folder { get; set; }

        [StringLength(255)]
        public string auto_report_ftp_directory { get; set; }

        public bool? create_data_mined_claim_tasks { get; set; }

        [StringLength(255)]
        public string tpa_email { get; set; }

        public byte? disable_faxing { get; set; }

        public byte? uses_consult_a_doc { get; set; }

        public byte? send_automated_precert_file_created_emails { get; set; }

        [StringLength(100)]
        public string quickconnect_portal_email_address { get; set; }

        public byte? send_email_for_cm_note { get; set; }

        public byte? invoice_order_by_group_number { get; set; }
    }
}

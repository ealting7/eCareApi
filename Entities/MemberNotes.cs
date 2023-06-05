using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("MEMBER_NOTES")]
    public class MemberNotes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_notes_id { get; set; }

        public Guid member_id { get; set; }

        public DateTime record_date { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int record_seq_num { get; set; }

        public Guid? user_updated { get; set; }

        [StringLength(512)]
        public string evaluation_text { get; set; }

        public bool? lcn { get; set; }

        public int? billing_id { get; set; }

        public DateTime end_time { get; set; }

        public int? RN_notes { get; set; }

        public bool? telephone { get; set; }

        public bool? employer { get; set; }

        public bool? member_call { get; set; }

        public bool? adhoc { get; set; }

        public bool? web_client_note { get; set; }

        public bool? care_plan_note { get; set; }

        public bool? std_note { get; set; }

        public bool? wc_note { get; set; }

        public bool? radiology { get; set; }

        public bool? lab { get; set; }

        public bool? internal_patient { get; set; }

        public DateTime? date_lcm_report_generated { get; set; }

        public int? lcn_case_number { get; set; }

        public int? lcm_followup_id { get; set; }

        public byte? note_billed { get; set; }

        public DateTime? note_billed_marked_date { get; set; }

        public Guid? updated_user_id { get; set; }

        public DateTime? updated_date { get; set; }

        public DateTime? date_lcm_activity_report_generated { get; set; }

        public int? lcm_activity_followup_id { get; set; }

        public byte? lcm_activity_report { get; set; }

        public byte? note_sent_in_file { get; set; }

        public DateTime? note_sent_in_file_date { get; set; }

        public byte? override_date { get; set; }

        public bool? care_coordination { get; set; }

        public byte? entered_via_web { get; set; }

    }
}

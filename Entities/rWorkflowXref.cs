using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_WORKFLOW_XREF")]
    public class rWorkflowXref
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int currentstatus_id { get; set; }
        public int pendreason_id { get; set; }
        public int eventtype_id { get; set; }
        public string evaluation_text { get; set; }
    }
}

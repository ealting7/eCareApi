using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RPT_NEXT_UNIQUEID")] 
    public class RptNextUniqueId
    {
        [Key]
        public int rpt_next_uniqueId { get; set; }
        public int? nxt_uniqueID { get; set; }
    }
}

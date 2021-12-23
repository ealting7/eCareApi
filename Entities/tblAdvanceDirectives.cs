using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("tblAdvanceDirectives")] 
    public class tblAdvanceDirectives
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long entryId { get; set; }
        public Guid userId { get; set; }
        public bool Declaration { get; set; }
        public bool DNR { get; set; }
        public bool PowerOfAttourney { get; set; }
        public Guid? userEntryID { get; set; }
        public Guid? member_id { get; set; }
        public string? note { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public byte? disabled { get; set; }

    }
}

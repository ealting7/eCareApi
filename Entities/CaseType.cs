using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("CASE_TYPE")] 
    public class CaseType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int case_type_id { get; set; }
        public string case_type_code { get; set; }
        public string case_type_descr { get; set; }
        public bool disable_flag { get; set; }
        public DateTime? date_updated { get; set; }
        public Guid? user_updated { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("r_FAXQUEUE")]
    public class FaxQueue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int? parent_id { get; set; }
        public string queue_name { get; set; }
        public int? listorder { get; set; }
        public bool fax_destination_flag { get; set; }
    }
}

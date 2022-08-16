using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_ER_STATUS")] 
    public class SimsErStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_er_status_id { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public byte? is_spanish_status { get; set; }

    }
}

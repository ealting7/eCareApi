using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_ER_ROOM")] 
    public class SimsErRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_er_room_id { get; set; }

        public string name { get; set; }
        public string description { get; set; }
        public byte? is_spanish_room { get; set; }
        public int? occupancy { get; set; }
        public string room_prefix { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }

    }
}

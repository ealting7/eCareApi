using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SIMS_ER")] 
    public class SimsEr
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sims_er_id { get; set; }

        public Guid? member_id { get; set; }
        public DateTime? check_in_date { get; set; }
        public Guid? check_in_user_id { get; set; }
        public DateTime? check_out_date { get; set; }
        public Guid? check_out_user_id { get; set; }
        public byte? ambulance_used { get; set; }
        public byte? do_not_publish { get; set; }
        public string reason_for_visit { get; set; }
        public int? sims_er_status_id { get; set; }
        public int? sims_er_room_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
        public int? sims_er_pain_level_id { get; set; }
        public int? member_address_id { get; set; }
        public int? member_phone_id { get; set; }
        public int? current_nurse_id { get; set; }
        public int? current_physician_id { get; set; }
        public int? paying_eps_id { get; set; }
        public byte? patient_paying { get; set; }
        public int? colombia_web_usertypes_id { get; set; }
        public int hospital_id { get; set; }


    }
}

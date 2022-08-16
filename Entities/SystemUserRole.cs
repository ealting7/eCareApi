using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("SYSTEM_USER_ROLE")] 
    public class SystemUserRole
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SYSTEM_USER_ROLE { get; set; }
        
        public Guid system_role_id { get; set; }
        public Guid system_user_id { get; set; }
        public DateTime date_updated { get; set; }
        public Guid? user_update { get; set; }

    }
}

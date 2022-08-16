using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("ICMS_USER_REFERENCE")] 
    public class IcmsUserReference
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int icms_user_reference_id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? icms_system_user_id { get; set; }
    }
}

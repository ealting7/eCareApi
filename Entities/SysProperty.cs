using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{

    [Table("sys_Property")] 
    public class SysProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sys_property_id { get; set; }
        
        public string propertyname { get; set; }
        public string propertyvalue { get; set; }
    }
}

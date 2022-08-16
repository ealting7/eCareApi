using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_MARITAL_STATUS_TYPES")] 
    public class HospitalMaritalStatusTypes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_marital_status_types_id { get; set; }

        public string marital_type_name { get; set; }

    }
}

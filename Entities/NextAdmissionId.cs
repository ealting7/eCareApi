using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("NEXT_ADMISSIONID")] 
    public class NextAdmissionId
    {
        public int? next_admission_id { get; set; }
    }
}

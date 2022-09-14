using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("CLINICAL_REVIEW_BILL_DESCRIPTIONS")] 
    public class ClinicalReviewBillDescriptions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int description_id { get; set; }
         
        public string description { get; set; }
        public int? frequency { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? created_user_id { get; set; }


    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_DOCUMENT_FORM")] 
    public class HospitalInpatientAdmissionDocumentForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_document_form_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public string document_name { get; set; }
        public string content_type { get; set; }
        public byte[] document_image { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }

    }
}

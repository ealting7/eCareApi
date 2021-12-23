using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ALLERGIES")] 
    public class HospitalInpatientAdmissionAllergies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_allergies_id { get; set; }
        public int? hospital_inpatient_admission_id { get; set; }
        public string? medication_allergy { get; set; }
        public string? other_allergy { get; set; }
        public bool? latex_allergy { get; set; }
        public bool? echinacea { get; set; }
        public bool? ephedra { get; set; }
        public bool? garlic { get; set; }
        public bool? gingko_biloba { get; set; }
        public bool? ginkgo { get; set; }
        public bool? ginseng { get; set; }
        public bool? kava { get; set; }
        public bool? st_johns_wort { get; set; }
        public bool? valerian { get; set; }
        public bool? valerian_root { get; set; }
        public bool? vite { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }

    }
}

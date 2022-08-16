using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_MEDICATION_ADMINISTERED")] 
    public class HospitalInpatientAdmissionOrderMedicationAdministered
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_order_medication_administered_id { get; set; }
        public int hospital_inpatient_admission_order_medication_id { get; set; }
        public DateTime administered_date { get; set; }
        public Guid administered_by_user_id { get; set; }

        public int? hospital_medication_route_of_administration_id { get; set; }
        public int? hospital_medication_dosage_forms_id { get; set; }
        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public bool? delete_flag { get; set; }
        public DateTime? delete_date { get; set; }
        public Guid? delete_user_id { get; set; }

    }
}

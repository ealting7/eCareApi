using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_ORDER_MEDICATION")] 
    public class HospitalInpatientAdmissionOrderMedication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_order_medication_id { get; set; }

        public int hospital_inpatient_admission_order_id { get; set; }
        public string medication_name { get; set; }
        public string ndc { get; set; }
        public decimal? gcn_seqno { get; set; }
        public string strength_unit { get; set; }
        public int? hospital_medication_strength_unit_id { get; set; }
        public int? hospital_medication_dosage_forms_id { get; set; }
        public string dose { get; set; }
        public int? hospital_medication_dose_unit_id { get; set; }
        public int? hospital_medication_frequency_administration_id { get; set; }
        public string time_of_administration { get; set; }
        public int? hospital_medication_route_of_administration_id { get; set; }
        public decimal? quantity { get; set; }
        public string duration_of_therapy { get; set; }
        public int? hospital_medication_duration_of_therapy_unit_id { get; set; }
        public bool? refill_authorization { get; set; }
        public bool? prn_use_as_needed { get; set; }
        public string prn_indicator { get; set; }
        public string prescriber_name { get; set; }
        public DateTime? prescribed_date { get; set; }
        public bool? deleted_flag { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public int? administered { get; set; }
        public Guid? administered_user_id { get; set; }
        public DateTime? administered_date { get; set; }
        public string reason_for_medication { get; set; }
        public Guid? prescribing_provider_pcp_id { get; set; }
        public bool? is_followup_order { get; set; }
        public string administered_name { get; set; }
        public string administered_title { get; set; }
        public int? hospital_inpatient_admission_nursing_assessment_invasive_cardiology_surgery_id { get; set; }
        public bool? is_re_assessment { get; set; }
        public int? hospital_inpatient_admission_nursing_assessment_newborn_mother_history_antenatal_followup_id { get; set; }
        public int? hospital_inpatient_admission_nursing_assessment_newborn_labor_history_id { get; set; }
        public int? hospital_inpatient_admission_nursing_assessment_pain_intervention_pharmacological_id { get; set; }

    }
}

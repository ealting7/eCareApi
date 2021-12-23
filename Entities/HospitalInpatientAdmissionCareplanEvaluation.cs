﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_CAREPLAN_EVALUATION")]    
    public class HospitalInpatientAdmissionCareplanEvaluation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_inpatient_admission_careplan_evaluation_id { get; set; }

        public int member_disease_condition_careplan_id { get; set; }
        public int hospital_inpatient_admission_id { get; set; }
        public string evaluation { get; set; }
        public int? hospital_inpatient_admission_careplan_intervention_id { get; set; }
        public bool? deleted { get; set; }
        public DateTime? deleted_date { get; set; }
        public Guid? deleted_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public Guid? last_update_user_id { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("HOSPITAL_INPATIENT_ADMISSION_MDT_NOTE_BLOOD_GAS_REFERENCE")] 
    public class HospitalInpatientAdmissionMdtNoteBloodGasReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_inpatient_admission_mdt_note_blood_gas_reference_id { get; set; }

        public int hospital_inpatient_admission_id { get; set; }
        public int hosptial_inpatient_admission_mdt_note { get; set; }
        public int hosptial_inpatient_admission_blood_gas_id { get; set; }
        public int hospital_inpatient_admission_chart_source_id { get; set; }

        public DateTime creation_date { get; set; }
        public Guid? creation_user_id { get; set; }

        public bool? highlight { get; set; }
        public string highlight_color { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("r_MEMBER_ADMISSION_CONTACTS")] 
    public class rMemberAdmissionContacts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int member_admission_contacts_id { get; set; }
        public string? admission_number { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? middle_name { get; set; }
        public string? maiden_name { get; set; }
        public string? address1 { get; set; }
        public string? city { get; set; }
        public string? phone1 { get; set; }
        public string? phone2 { get; set; }
        public int? relationship { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? creation_user_id { get; set; }
        public Guid? member_id { get; set; }
        public Guid? last_update_user_id { get; set; }
        public DateTime? last_update_date { get; set; }
        public byte? disabled { get; set; }
        public int? hospital_inpatient_admission_id { get; set; }

    }
}

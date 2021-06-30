using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_ORDER_TEST")]
    public class HospitalOrderTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hospital_order_test_id { get; set; }
        public string? test_name { get; set; }
        public bool? disabled { get; set; }
        public Guid? creation_user_id { get; set; }
        public DateTime? creation_date { get; set; }
        public Guid? update_user_id { get; set; }
        public DateTime? update_date { get; set; }
        public bool? is_labq_test { get; set; }

    }
}

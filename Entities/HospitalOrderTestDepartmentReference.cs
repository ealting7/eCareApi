using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("HOSPITAL_ORDER_TEST_DEPARTMENT_REFERENCE")] 
    public class HospitalOrderTestDepartmentReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int hosptial_order_test_department_reference_id { get; set; }
        public int hospital_order_test_id { get; set; }
        public int hospital_department_id { get; set; }
    }
}

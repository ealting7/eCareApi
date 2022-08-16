using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace eCareApi.Entities
{
    [Table("EMPLOYER_ADDRESS")] 
    public class EmployerAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int employer_address_id { get; set; }
        
        public int? employer_id { get; set; }
        public string address_line_one { get; set; }
        public string address_line_two { get; set; }
        public string city { get; set; }
        public string state_abbrev { get; set; }
        public string zip_code { get; set; }
        public string address_note { get; set; }
        public int? address_county_id { get; set; }
        public int? address_type_id { get; set; }


    }
}

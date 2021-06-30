using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("STATE")]
    public class State
    {
        public string state_abbrev { get; set; }
        public string state_name { get; set; }
        public int? country_id { get; set; }
    }
}

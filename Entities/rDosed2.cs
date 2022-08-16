using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RDOSED2")]
    public class rDosed2
    {
        public string GCDF { get; set; }
        public string DOSE { get; set; }
        public string GCDF_DESC { get; set; }
    }
}

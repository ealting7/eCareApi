using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RROUTED3")] 
    public class rRouted3
    {
        public string GCRT { get; set; }
        public string RT { get; set; }
        public string GCRT2 { get; set; }
        public string GCRT_DESC { get; set; }
        public string SYSTEMIC { get; set; }
    }
}

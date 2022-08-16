using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RGCNSEQ4")] 
    public class rGcnseq4
    {
        public decimal GCN_SEQNO { get; set; }
        public string HIC3 { get; set; }
        public decimal? HICL_SEQNO { get; set; }
        public string GCDF { get; set; }
        public string GCRT { get; set; }
        public string STR { get; set; }
        public decimal? GTC { get; set; }
        public decimal? TC { get; set; }
        public string DCC { get; set; }
        public string GCNSEQ_GI { get; set; }
        public string GENDER { get; set; }
        public decimal? HIC3_SEQN { get; set; }
        public string STR60 { get; set; }

    }
}

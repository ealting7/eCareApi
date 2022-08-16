using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("RNDC14")]
    public class rNdc14
    {
        public string NDC { get; set; }
        public string LBLRID { get; set; }
        public decimal? GCN_SEQNO { get; set; }
        public decimal? PS { get; set; }
        public string DF { get; set; }
        public string AD { get; set; }
        public string LN { get; set; }
        public string BN { get; set; }
        public string PNDC { get; set; }
        public string REPNDC { get; set; }
        public string NDCFI { get; set; }
        public string DADDNC { get; set; }
        public string DUPDC { get; set; }
        public string DESI { get; set; }
        public string DESDTEC { get; set; }
        public string DESI2 { get; set; }
        public string DES2DTEC { get; set; }
        public string DEA { get; set; }
        public string CL { get; set; }
        public string GPI { get; set; }
        public string HOSP { get; set; }
        public string INNOV { get; set; }
        public string IPI { get; set; }
        public string MINI { get; set; }
        public string MAINT { get; set; }
        public string OBC { get; set; }
        public string OBSDTEC { get; set; }
        public string PPI { get; set; }
        public string STPK { get; set; }
        public string REPACK { get; set; }
        public string TOP200 { get; set; }
        public string UD { get; set; }
        public decimal? CSP { get; set; }
        public decimal? NDL_GDGE { get; set; }
        public decimal? NDL_LNGTH { get; set; }
        public decimal? SYR_CPCTY { get; set; }
        public decimal? SHLF_PCK { get; set; }
        public decimal? SHIPPER { get; set; }
        public string HCFA_FDA { get; set; }
        public string HCFA_UNIT { get; set; }
        public decimal? HCFA_PS { get; set; }
        public string HCFA_APPC { get; set; }
        public string HCFA_MRKC { get; set; }
        public string HCFA_TRMC { get; set; }
        public string HCFA_TYP { get; set; }
        public string HCFA_DESC1 { get; set; }
        public string HCFA_DESI1 { get; set; }
        public string UU { get; set; }
        public string PD { get; set; }
        public string LN25 { get; set; }
        public string LN25I { get; set; }
        public string GPIDC { get; set; }
        public string BBDC { get; set; }
        public string HOME { get; set; }
        public string INPCKI { get; set; }
        public string OUTPCKI { get; set; }
        public string OBC_EXP { get; set; }
        public decimal? PS_EQUIV { get; set; }
        public string PLBLR { get; set; }
        public string TOP50GEN { get; set; }
        public string OBC3 { get; set; }
        public string GMI { get; set; }
        public string GNI { get; set; }
        public string GSI { get; set; }
        public string GTI { get; set; }
        public string NDCGI1 { get; set; }
        public string HCFA_DC { get; set; }
        public string LN60 { get; set; }
        public byte? high_dollar_trigger { get; set; }

    }
}

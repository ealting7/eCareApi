using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class MedicalCodeService : IMedicalCode
    {
        private readonly IcmsContext _icmsContext;

        public MedicalCodeService(IcmsContext icmsContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
        }


        public IEnumerable<MedicalCode> getMedicalCode(string CodeType, string SearchText)
        {
            IEnumerable<MedicalCode> MedCode = null;

            switch (CodeType)
            {
                case "ICD10":
                    MedCode = getIcd10CodeSearch(SearchText);
                    break;

                case "CPT":
                    MedCode = getCptCodeSearch(SearchText);
                    break;

                case "HCPCS":
                    MedCode = getHcpcsCodeSearch(SearchText);
                    break;
            }

            return MedCode;
        }

        public MedicalCode getMedicalCodeById(string CodeType, int CodeId)
        {
            MedicalCode MedCode = null;

            switch (CodeType)
            {
                case "ICD10":
                    MedCode = getIcd10CodeById(CodeId);
                    break;

                case "CPT":
                    MedCode = getCptCodeById(CodeId);
                    break;

                case "HCPCS":
                    MedCode = getHcpcsCodeById(CodeId);
                    break;
            }

            return MedCode;
        }



        public IEnumerable<MedicalCode> getIcd10CodeSearch(string SearchText)
        {
            IEnumerable<MedicalCode> MedCode = (
                                                from diagCode in _icmsContext.DiagnosisCodes

                                                where diagCode.diagnosis_code.Contains(SearchText)
                                                || diagCode.short_description.Contains(SearchText)
                                                || diagCode.medium_description.Contains(SearchText)
                                                || diagCode.long_description.Contains(SearchText)
                                                && !string.IsNullOrEmpty(diagCode.diagnosis_code)
                                                select new MedicalCode
                                                {
                                                    CodeType = "ICD10",
                                                    Code = diagCode.diagnosis_code,
                                                    CodeId = diagCode.diagnosis_codes_10_id,
                                                    ShortDescription = diagCode.short_description,
                                                    MediumDescription = diagCode.medium_description,
                                                    LongDescription = diagCode.long_description
                                                }).Take(50)
                                                .OrderBy(diagnosisCode => diagnosisCode.Code);

            return MedCode;
        }

        public IEnumerable<MedicalCode> getCptCodeSearch(string SearchText)
        {
            IEnumerable<MedicalCode> MedCode = (
                                                    from cptCode in _icmsContext.CptCodes
                                                    where cptCode.cpt_code.Contains(SearchText)
                                                    || cptCode.short_descr.Contains(SearchText)
                                                    || cptCode.medium_descr.Contains(SearchText)
                                                    || cptCode.cpt_descr.Contains(SearchText)
                                                    && !string.IsNullOrEmpty(cptCode.cpt_code)
                                                    select new MedicalCode
                                                    {
                                                        CodeType = "CPT",
                                                        Code = cptCode.cpt_code,
                                                        CodeId = cptCode.cpt_codes_2015_id,
                                                        ShortDescription = cptCode.short_descr,
                                                        MediumDescription = cptCode.medium_descr,
                                                        LongDescription = cptCode.cpt_descr
                                                    }).Take(50)
                                                    .OrderBy(cptCode => cptCode.Code);

            return MedCode;
        }

        public IEnumerable<MedicalCode> getHcpcsCodeSearch(string SearchText)
        {
            IEnumerable<MedicalCode> MedCode = (
                                                    from hcpcsCode in _icmsContext.HcpcsCodes
                                                    where hcpcsCode.hcp_code.Contains(SearchText)
                                                    || hcpcsCode.hcpcs_short.Contains(SearchText)
                                                    || hcpcsCode.hcpcs_full.Contains(SearchText)
                                                    && !string.IsNullOrEmpty(hcpcsCode.hcp_code)
                                                    select new MedicalCode
                                                    {
                                                        CodeType = "HCPCS",
                                                        Code = hcpcsCode.hcp_code,
                                                        CodeId = hcpcsCode.hcpcs_codes_2015_id,
                                                        ShortDescription = hcpcsCode.hcpcs_short,
                                                        MediumDescription = "",
                                                        LongDescription = hcpcsCode.hcpcs_full
                                                    }).Take(50)
                                                    .OrderBy(hcpcCode => hcpcCode.Code);

            return MedCode;
        }


        public MedicalCode getIcd10CodeById(int CodeId)
        {
            MedicalCode MedCode = (
                                    from diagCode in _icmsContext.DiagnosisCodes
                                    where diagCode.diagnosis_codes_10_id.Equals(CodeId)
                                    select new MedicalCode
                                    {
                                        CodeType = "ICD10",
                                        Code = diagCode.diagnosis_code,
                                        CodeId = diagCode.diagnosis_codes_10_id,
                                        ShortDescription = diagCode.short_description,
                                        MediumDescription = diagCode.medium_description,
                                        LongDescription = diagCode.long_description,
                                    }).SingleOrDefault();

            return MedCode;
        }

        public MedicalCode getCptCodeById(int CodeId)
        {
            MedicalCode MedCode = (
                                    from cptCode in _icmsContext.CptCodes
                                    where cptCode.cpt_codes_2015_id.Equals(CodeId)
                                    select new MedicalCode
                                    {
                                        CodeType = "CPT",
                                        Code = cptCode.cpt_code,
                                        CodeId = cptCode.cpt_codes_2015_id,
                                        ShortDescription = cptCode.short_descr,
                                        MediumDescription = cptCode.medium_descr,
                                        LongDescription = cptCode.cpt_descr
                                    }).SingleOrDefault();

            return MedCode;
        }

        public MedicalCode getHcpcsCodeById(int CodeId)
        {
            MedicalCode MedCode = (
                                    from hcpcsCode in _icmsContext.HcpcsCodes
                                    where hcpcsCode.hcpcs_codes_2015_id.Equals(CodeId)
                                    select new MedicalCode
                                    {
                                        CodeType = "HCPCS",
                                        Code = hcpcsCode.hcp_code,
                                        CodeId = hcpcsCode.hcpcs_codes_2015_id,
                                        ShortDescription = hcpcsCode.hcpcs_short,
                                        MediumDescription = "",
                                        LongDescription = hcpcsCode.hcpcs_full
                                    }).SingleOrDefault();

            return MedCode;
        }
    }
}

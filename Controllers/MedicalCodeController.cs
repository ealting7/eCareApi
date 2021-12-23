using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MedicalCodeController : ControllerBase
    {
        private readonly IMedicalCode _medicalcodeInterface;

        public MedicalCodeController(IMedicalCode medicalcodeInterface)
        {
            _medicalcodeInterface = medicalcodeInterface ?? throw new ArgumentNullException(nameof(medicalcodeInterface));
        }



        [HttpGet("labqsearch/codes/{codetype}/{search}")]
        public IActionResult getMedicalCodeSearch(string codeType, string search)
        {
            var medCodes = _medicalcodeInterface.getMedicalCode(codeType, search);

            if (medCodes == null)
            {
                return NoContent();
            }

            var results = new List<MedicalCode>();

            foreach (var code in medCodes)
            {
                results.Add(new MedicalCode
                {
                    CodeType = code.CodeType,
                    CodeId = code.CodeId,
                    Code = code.Code,
                    ShortDescription = code.ShortDescription,
                    MediumDescription = code.MediumDescription,
                    LongDescription = code.LongDescription,
                    DisplayDescription = !string.IsNullOrEmpty(code.MediumDescription) ? code.MediumDescription :
                                            ((!string.IsNullOrEmpty(code.LongDescription) ? code.LongDescription :
                                                (!string.IsNullOrEmpty(code.ShortDescription) ? code.ShortDescription : "")
                                             ))
                });
            }


            return Ok(results);
        }


        [HttpGet("labqsearch/codes/{codetype}/code/{id}")]
        public IActionResult getMedicalCodeById(string codeType, int id)
        {
            var medCodes = _medicalcodeInterface.getMedicalCodeById(codeType, id);

            if (medCodes == null)
            {
                return NoContent();
            }

            MedicalCode returnCode = new MedicalCode();

            returnCode.CodeType = medCodes.CodeType;
            returnCode.CodeId = medCodes.CodeId;
            returnCode.Code = medCodes.Code;
            returnCode.ShortDescription = medCodes.ShortDescription;
            returnCode.MediumDescription = medCodes.MediumDescription;
            returnCode.LongDescription = medCodes.LongDescription;
            returnCode.DisplayDescription = !string.IsNullOrEmpty(medCodes.MediumDescription) ? medCodes.MediumDescription :
                                    ((!string.IsNullOrEmpty(medCodes.LongDescription) ? medCodes.LongDescription :
                                        (!string.IsNullOrEmpty(medCodes.ShortDescription) ? medCodes.ShortDescription : "")
                                        ));


            return Ok(returnCode);
        }


        [HttpGet("codes/{codetype}/code/{id}")]
        public IActionResult getBasicMedicalCodeById(string codeType, int id)
        {
            var medCodes = _medicalcodeInterface.getMedicalCodeById(codeType, id);

            if (medCodes == null)
            {
                return NoContent();
            }

            MedicalCode returnCode = new MedicalCode();

            returnCode.CodeType = medCodes.CodeType;
            returnCode.CodeId = medCodes.CodeId;
            returnCode.Code = medCodes.Code;
            returnCode.ShortDescription = medCodes.ShortDescription;
            returnCode.MediumDescription = medCodes.MediumDescription;
            returnCode.LongDescription = medCodes.LongDescription;
            returnCode.DisplayDescription = !string.IsNullOrEmpty(medCodes.MediumDescription) ? medCodes.MediumDescription :
                                    ((!string.IsNullOrEmpty(medCodes.LongDescription) ? medCodes.LongDescription :
                                        (!string.IsNullOrEmpty(medCodes.ShortDescription) ? medCodes.ShortDescription : "")
                                        ));


            return Ok(returnCode);
        }


        [HttpGet("um/medicalcodes/search/{codetype}/{search}")]
        public IActionResult getMedicalCodeUmSearch(string codeType, string search)
        {
            var medCodes = _medicalcodeInterface.getMedicalCode(codeType, search);

            if (medCodes == null)
            {
                return NoContent();
            }

            var results = new List<MedicalCode>();

            foreach (var code in medCodes)
            {
                results.Add(new MedicalCode
                {
                    CodeType = code.CodeType,
                    CodeId = code.CodeId,
                    Code = code.Code,
                    ShortDescription = code.ShortDescription,
                    MediumDescription = code.MediumDescription,
                    LongDescription = code.LongDescription,
                    DisplayDescription = !string.IsNullOrEmpty(code.MediumDescription) ? code.MediumDescription :
                                            ((!string.IsNullOrEmpty(code.LongDescription) ? code.LongDescription :
                                                (!string.IsNullOrEmpty(code.ShortDescription) ? code.ShortDescription : "")
                                             ))
                });
            }


            return Ok(results);
        }
    }
}

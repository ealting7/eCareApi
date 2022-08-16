using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IMedicalCode
    {
        IEnumerable<MedicalCode> getMedicalCode(string CodeType, string SearchText);
        MedicalCode getMedicalCodeById(string CodeType, int CodeId);

        public MedicalCode addMedicalCode(MedicalCode addCode);
    }
}

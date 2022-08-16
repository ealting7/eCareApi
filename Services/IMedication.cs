using eCareApi.Entities;
using eCareApi.Models;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IMedication
    {
        public List<Medication> medicationSearch(Medication medSearch);

        public List<Medication> updateAdmissionMedications(Medication med);

        public List<Medication> reOrderAdmissionMedications(Medication med);

        public List<Medication> removeAdmissionMedications(Medication med); 
    }
}

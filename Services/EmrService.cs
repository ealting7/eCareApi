using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Services
{
    public class EmrService : IEmr
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _emrContext;

        public EmrService(IcmsContext icmsContext, AspNetContext emrContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _emrContext = emrContext ?? throw new ArgumentNullException(nameof(emrContext));
        }


        public Patient getPatientAllergies(string patientId)
        {

            Patient patientAllergies = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patientId, out memberId))
            {

                List<Allergy> inpatAdmitAllergies = getHospitalInpatientAdmissionAllergies(memberId);
                List<Allergy> memMedAllergies = getMemberMedicationAllergies(memberId);
                //List<Allergy> memAdmissionAllergies = getMemberAdmissionAllergies(memberId);
            }            

            return patientAllergies;
        }



        private List<Allergy> getHospitalInpatientAdmissionAllergies(Guid memberId)
        {

            List<Allergy> allergies = null;

            allergies = (

                    from hospInptAdmit in _icmsContext.HospitalInpatientAdmissions

                    join hospAllergy in _icmsContext.HospitalInpatientAdmissionAllergys
                    on hospInptAdmit.hospital_inpatient_admission_id equals hospAllergy.hospital_inpatient_admission_id

                    where hospInptAdmit.member_id.Equals(memberId)
                    select new Allergy
                    {
                        allergyId = hospAllergy.hospital_inpatient_admission_allergies_id,
                        identifiedAtType = "Inpatient Admission",
                        medicationAllergy = hospAllergy.medication_allergy,
                        otherAllergies = hospAllergy.other_allergy,
                        echinaceaAllergy = hospAllergy.echinacea,
                        ephedraAllergy = hospAllergy.ephedra,
                        garlicAllergy = hospAllergy.garlic,
                        gingkoBilobaAllergy = hospAllergy.gingko_biloba,
                        ginkgoAllergy = hospAllergy.ginkgo,
                        ginsengAllergy = hospAllergy.ginseng,
                        kavaAllergy = hospAllergy.kava,
                        latexAllergy = hospAllergy.latex_allergy,
                        stJohnsWortAllergy = hospAllergy.st_johns_wort,
                        valerianAllergy = hospAllergy.valerian,
                        valerianRootAllergy = hospAllergy.valerian_root,
                        viteAllergy = hospAllergy.vite
                    }
                )
                .ToList();                                

            return allergies;
        }
        private List<Allergy> getMemberMedicationAllergies(Guid memberId)
        {

            List<Allergy> allergies = null;

            allergies = (

                    from memMedsAllergy in _icmsContext.MemberMedsAllergieses
                    where memMedsAllergy.member_id.Equals(memberId)
                    select new Allergy
                    {
                        allergyId = memMedsAllergy.member_meds_allergies_id,
                        identifiedAtType = "Medication",
                        medicationAllergy = memMedsAllergy.descr,
                    }
                )
                .ToList();

            return allergies;
        }
        private List<Allergy> getMemberAdmissionAllergies(Guid memberId)
        {

            List<Allergy> allergies = null;

            //allergies = (

            //        from rMemAdmisAllergy in _icmsContext.rMemberAdmissionAllergieses
            //        where rMemAdmisAllergy.member_id.Equals(memberId)
            //        select new Allergy
            //        {
            //            allergyId = rMemAdmisAllergy.member_meds_allergies_id,
            //            identifiedAtType = "Medication",
            //            medicationAllergy = rMemAdmisAllergy.descr,
            //        }
            //    )
            //    .ToList();

            return allergies;
        }
    }
}

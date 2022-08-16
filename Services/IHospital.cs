using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface IHospital
    {
        public IcmsUser getLoggedInUser(Guid userId);

        public List<Admission> getHospitalDepartments(string hospitalId);

        public List<Room> getHospitalDepartmentRooms(string departmentId);

        public List<ErVisit> getHospitalErVisits(string hospitalId);

        public List<Admission> getHospitalInpatientAdmissions(Admission search);

        public Admission getInpatientAdmission(Admission admit);

        public InpatientChart getInpatientChart(InpatientChart admit);

        public InpatientChartSource getAdmissionChartSourceItem(InpatientChart chart);

        public List<InpatientChartSource> getAdmissionChartSources(InpatientChart chartId);

        public InpatientChartSource getAdmissionChartSourceHighlightNote(InpatientChart source);




        public Admission getInpatientAdmissionUserDashboardDefaults(Admission admit);

        public List<Admission> getPatientInpatientAdmissions(string patientId);

        public Admission getPatientCurrentAdmission(string patientId);

        public List<Hospital> GetHospitals();

        IEnumerable<Hospital> GetCollectionFacilities();

        IEnumerable<HospitalFacility> GetLaboratoryFacilities();

        public List<Specimen> getSpecimenTypes();

        public List<Specimen> getSpecimenVolumes();

        public List<Note> getHospitalNoteTypes();

        public CareplanAssessItem getCareplanAssessFormItems();

        public CareplanAssessItem getCareplanDiagnosisDomainClasses(string domainId);

        public List<AssessItem> getCareplanAssessBasicGenerals(int inpatientAdmissionId);



        public Admission getUserInpatientAdmissionDashboardDefaults(string admitId, string userId);

        public Admission updateUserInpatientAdmissionDashboardDefaults(Admission defaults);



        public Admission getAdmissionMedications(string admitId);

        public Admission getRemovedAdmissionMedications(string admitId);

        public Admission updateAdmissionMedicationAllergies(Admission medication);


        public Admission getAdmissionLabs(string admitId);


        public List<Note> getAdmissionNotesMdt(Note note);


        public Admission getAdmissionDocuments(string admitId);

        public DocumentForm getAdmissionDocument(string documentId);

        public Admission uploadAdmissionDocument(DocumentForm doc);



        public Admission insertAdmissionVitalSign(Admission admit);


        public InpatientChart updateAllChartSources(InpatientChart chart);

        public InpatientChart updateChartSource(InpatientChart chart);

        public Patient insertHospitalInpatientAdmission(Patient patient);

        public List<Note> updateAdmissionNotesMdt(Note mdtNote, bool getReturnNotes);

        
        public bool disableFacility(HospitalFacility facility);
    }
}

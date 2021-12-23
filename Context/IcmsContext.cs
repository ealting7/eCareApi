using eCareApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Context
{
    public class IcmsContext : DbContext
    {
        public DbSet<AccountsReceivablePayments> AccountsReceivablePayments { get; set; }
        public DbSet<BillingCreateRefreshDates> BillingCreateRefreshDates { get; set; }
        public DbSet<CaseOwner> CaseOwners { get; set; }
        public DbSet<CptCodes2015> CptCodes { get; set; }
        public DbSet<DiagnosisCodes10> DiagnosisCodes10 { get; set; }
        public DbSet<DiagnosisCodes> DiagnosisCodes { get; set; }
        public DbSet<DiseaseCondition> DiseaseConditions { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<FaxQueue> FaxQueues { get; set; }
        public DbSet<InboundFax> FaxPoolFaxes { get; set; }
        public DbSet<Hcpcs2015> HcpcsCodes { get; set; }
        public DbSet<HospitalOrderTest> LabTypes { get; set; }
        public DbSet<HospitalOrderTestCpt> LabTypeCpts { get; set; }
        public DbSet<HospitalAbdominalContour> HospitalAbdominalContours { get; set; }
        public DbSet<HospitalAlertnessState> HospitalAlertnessStates { get; set; }
        public DbSet<HospitalAppointmentSchedule> HospitalAppointmentSchedules { get; set; }
        public DbSet<HospitalBreathingRate> HospitalBreathingRates { get; set; }
        public DbSet<HospitalBreathingType> HospitalBreathingTypes { get; set; }
        public DbSet<HospitalCareplanGoal> HospitalCareplanGoals { get; set; }
        public DbSet<HospitalCareplanInterventionType> HospitalCareplanInterventionTypes { get; set; }
        public DbSet<HospitalChronologicalDevelopmentAppearance> HospitalChronologicalDevelopmentAppearances { get; set; }
        public DbSet<HospitalDepartmentAppointmentTypes> HospitalDepartmentAppointmentTypes { get; set; }
        public DbSet<HospitalDepartmentAppointmentTypesDurationReference> HospitalDepartmentAppointmentTypesDurationReferences { get; set; }
        public DbSet<HospitalDepartment> HospitalDepartments { get; set; }
        public DbSet<HospitalDepartmentRooms> HospitalDepartmentRooms { get; set; }
        public DbSet<HospitalDepartmentWorkday> HospitalDepartmentWorkdays { get; set; }
        public DbSet<HospitalHeadProportionToBody> HospitalHeadProportionToBodies { get; set; }
        public DbSet<HospitalHeadSkinColor> HospitalHeadSkinColors { get; set; }
        public DbSet<HospitalInpatientAdmissionAllergies> HospitalInpatientAdmissionAllergys { get; set; }
        public DbSet<HospitalInpatientAdmission> HospitalInpatientAdmissions { get; set; }        
        public DbSet<HospitalInpatientAdmissionCareplanAssess> HospitalInpatientAdmissionCareplanAssesses { get; set; }
        public DbSet<HospitalInpatientAdmissionCareplanDiagnosis> HospitalInpatientAdmissionCareplanDiagnoses { get; set; }
        public DbSet<HospitalInpatientAdmissionCareplanEvaluation> HospitalInpatientAdmissionCareplanEvaluations { get; set; }
        public DbSet<HospitalInpatientAdmissionCareplanInterventionAdministered> HospitalInpatientAdmissionCareplanInterventionAdministereds { get; set; }
        public DbSet<HospitalInpatientAdmissionCareplanIntervention> HospitalInpatientAdmissionCareplanInterventions { get; set; }
        public DbSet<HospitalInpatientAdmissionCareplanOutcome> HospitalInpatientAdmissionCareplanOutcomes { get; set; }
        public DbSet<HospitalInpatientAdmissionNursingProcessAssessmentBasicGeneral> HospitalInpatientAdmissionNursingProcessAssessmentBasicGenerals { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderCpt> HospitalInpatientAdmissionOrderCpts { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderDiagnosis> HospitalInpatientAdmissionDiagnoses { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderHcpcs> HospitalInpatientAdmissionOrderHcpcses { get; set; }
        public DbSet<HospitalInpatientAdmissionOrder> HospitalInpatientAdmissionOrders { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderLab> HospitalInpatientAdmissionOrderLabs { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderResult> HospitalInpatientAdmissionOrderResults { get; set; }        
        public DbSet<HospitalInpatientAdmissionReasonForVisit> HospitalInpatientAdmissionReasonForVisits { get; set; }
        public DbSet<HospitalMedicationFrequencyAdministration> HospitalMedicationFrequencyAdministrations { get; set; }
        public DbSet<HospitalMentalStatus> HospitalMentalStatuses { get; set; }
        public DbSet<HospitalNursingDiagnosisClass> HospitalNursingDiagnosisClasses { get; set; }
        public DbSet<HospitalNursingDiagnosisDomain> HospitalNursingDiagnosisDomains { get; set; }
        public DbSet<HospitalOrderTest> HospitalOrderTests { get; set; }
        public DbSet<HospitalOrderTestDiagnosis> HospitalOrderTestDiagnosis { get; set; }        
        public DbSet<HospitalPainLevel> HospitalPainLevels { get; set; }
        public DbSet<HospitalPalpateArteryStrength> HospitalPalpateArteryStrengths { get; set; }
        public DbSet<HospitalPulseIntensity> HospitalPulseIntensities { get; set; }
        public DbSet<HospitalPulsePositionForReading> HospitalPulsePositionForReadings { get; set; }
        public DbSet<HospitalPulseRhythm> HospitalPulseRhythms { get; set; }
        public DbSet<HospitalRace> HospitalRaces { get; set; }
        public DbSet<HospitalRespirationDepth> HospitalRespirationDepths { get; set; }        
        public DbSet<HospitalRespirationRegularity> HospitalRespirationRegularities { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<HospitalSpecialty> HospitalSpecialtys { get; set; }
        public DbSet<HospitalSqueezePushStrength> HospitalSqueezePushStrengths { get; set; }
        public DbSet<HospitalTemperatureSite> HospitalTemperatureSites { get; set; }
        public DbSet<SystemUser> IcmsUsers { get; set; }
        public DbSet<InsuranceRelationship> InsuranceRelationships { get; set; }
        public DbSet<HospitalOrderTestHcpcs> LabTypeHcpcs { get; set; }
        public DbSet<LcmBillingCodes> LcmBillingCodes { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }
        public DbSet<MemberDiseaseConditionAssessment> MemberDiseaseConditionAssessments { get; set; }
        public DbSet<MemberDiseaseConditionCareplan> MemberDiseaseConditionCareplans { get; set; }
        public DbSet<MemberDiseaseConditionAssessmentTemplate> MemberDiseaseConditionAssessmentTemplates { get; set; }
        public DbSet<MemberDiseaseConditionAssessmentTemplateAnswers> MemberDiseaseConditionAssessmentTemplateAnswers { get; set; }        
        public DbSet<MemberDiseaseConditionAssessmentTemplateQuestion> MemberDiseaseConditionAssessmentTemplateQuestions { get; set; }
        public DbSet<MemberDiseaseConditionAssessmentTemplateQuestionAnswer> MemberDiseaseConditionAssessmentTemplateQuestionAnswers { get; set; }
        public DbSet<MemberDiseaseConditionDifferentialIcd> MemberDiseaseConditionDifferentialIcds { get; set; }        
        public DbSet<MemberDiseaseConditionReference> MemberDiseaseConditionReferences { get; set; }        
        public DbSet<MemberEnrollment> MemberEnrollments { get; set; }
        public DbSet<MemberEthnicity> MemberEthnicity { get; set; }
        public DbSet<MemberInsurance> MemberInsurances { get; set; }            
        public DbSet<MemberNotes> MemberNotes { get; set; }
        public DbSet<MemberPhone> MemberPhoneNumbers { get; set; }
        public DbSet<MemberRace> MemberRaces { get; set; }
        public DbSet<rMemberReferral> MemberReferrals { get; set; }
        public DbSet<rMemberReferralCpts> MemberReferralCpts { get; set; }
        public DbSet<rMemberReferralDiags> MemberReferralDiags { get; set; }
        public DbSet<rMemberReferralHcpcs> MemberReferralHcpcss { get; set; }
        public DbSet<MergedMemberReference> MergedMemberReferences { get; set; }
        public DbSet<NextAdmissionId> NextAdmissionIds { get; set; }        
        public DbSet<Member> Patients { get; set; }
        public DbSet<ProviderAddress> PcpAddresses { get; set; }
        public DbSet<ProviderAddressContact> PcpAddressContacts { get; set; }
        public DbSet<ProviderContact>PcpContacts { get; set; }
        public DbSet<ProviderContactPhone> PcpContactPhones { get; set; }
        public DbSet<ProviderPhone> PcpPhoneNumbers { get; set; }
        public DbSet<PcpTable> Pcps { get; set; }
        public DbSet<ProviderSpecialtys> PcpSpecialtys { get; set; }
        public DbSet<PhoneType> PhoneTypes { get; set; }          
        public DbSet<rReferralCategory> ReferralCategories { get; set; }
        public DbSet<rReferralContext> ReferralContexts { get; set; }
        public DbSet<rReferralPendReason> ReferralActionReasons { get; set; }
        public DbSet<rCurrentStatus> ReferralCurrentStatus { get; set; }
        public DbSet<rReferralReason> ReferralReasons { get; set; }
        public DbSet<rReferralType> ReferralTypes { get; set; }
        public DbSet<RptNextUniqueId> RptNextUniqueIds { get; set; }
        public DbSet<Specialtys> Specialtys { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<TpaEmployer> TpaEmployers { get; set; }
        public DbSet<Tpas> Tpas { get; set; }

        public IcmsContext(DbContextOptions<IcmsContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SystemUser>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<State>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<PcpTable>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<ProviderSpecialtys>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<ProviderAddressContact>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<ProviderContactPhone>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<NextAdmissionId>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<MemberNotes>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<CaseOwner>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<TpaEmployer>(eb =>
                {
                    eb.HasNoKey();
                });
        }
    }
}

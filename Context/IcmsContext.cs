using eCareApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Context
{
    public class IcmsContext : DbContext
    {
        public DbSet<CptCodes2015> CptCodes { get; set; }
        public DbSet<DiagnosisCodes10> DiagnosisCodes { get; set; }
        public DbSet<FaxQueue> FaxQueues { get; set; }
        public DbSet<InboundFax> FaxPoolFaxes { get; set; }
        public DbSet<Hcpcs2015> HcpcsCodes { get; set; }
        public DbSet<HospitalOrderTest> LabTypes { get; set; }
        public DbSet<HospitalOrderTestCpt> LabTypeCpts { get; set; }

        public DbSet<HospitalInpatientAdmission> HospitalInpatientAdmissions { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderCpt> HospitalInpatientAdmissionOrderCpts { get; set; }        
        public DbSet<HospitalInpatientAdmissionOrderDiagnosis> HospitalInpatientAdmissionDiagnoses { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderHcpcs> HospitalInpatientAdmissionOrderHcpcses { get; set; }        
        public DbSet<HospitalInpatientAdmissionOrder> HospitalInpatientAdmissionOrders { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderLab> HospitalInpatientAdmissionOrderLabs { get; set; }
        public DbSet<HospitalInpatientAdmissionOrderResult> HospitalInpatientAdmissionOrderResults { get; set; }        


        public DbSet<HospitalOrderTestDiagnosis> HospitalOrderTestDiagnosis { get; set; }
        public DbSet<HospitalAppointmentSchedule> HospitalAppointmentSchedules { get; set; }
        public DbSet<HospitalDepartmentAppointmentTypes> HospitalDepartmentAppointmentTypes { get; set; }
        public DbSet<HospitalDepartmentAppointmentTypesDurationReference> HospitalDepartmentAppointmentTypesDurationReferences { get; set; }        
        public DbSet<HospitalDepartment> HospitalDepartments { get; set; }
        public DbSet<HospitalDepartmentRooms> HospitalDepartmentRooms { get; set; }
        public DbSet<HospitalDepartmentWorkday> HospitalDepartmentWorkdays { get; set; }        
        public DbSet<HospitalRace> HospitalRaces { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<HospitalSpecialty> HospitalSpecialtys { get; set; }
        public DbSet<SystemUser> IcmsUsers { get; set; }
        public DbSet<InsuranceRelationship> InsuranceRelationships { get; set; }
        public DbSet<HospitalOrderTestHcpcs> LabTypeHcpcs { get; set; }
        public DbSet<MemberAddress> MemberAddresses { get; set; }
        public DbSet<MemberEthnicity> MemberEthnicity { get; set; }
        public DbSet<MemberInsurance> MemberInsurances { get; set; }       
        public DbSet<MemberPhone> MemberPhoneNumbers { get; set; }
        public DbSet<MemberRace> MemberRaces { get; set; }
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
        public DbSet<Specialtys> Specialtys { get; set; }
        public DbSet<State> States { get; set; }

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
                });
        }
    }
}

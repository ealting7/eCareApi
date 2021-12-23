using eCareApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Context
{
    public class IcmsDataStagingContext : DbContext
    {

        public DbSet<LcmBillingWorktable> LcmBillingWorktables { get; set; }
        public DbSet<BillingBackup> BillingBackups { get; set; }
        public DbSet<UnpaidLcmInvoiceDownloads> UnpaidLcmInvoiceDownloads { get; set; }



        public IcmsDataStagingContext(DbContextOptions<IcmsDataStagingContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder                
                .Entity< UnpaidLcmInvoiceDownloads>(eb => 
                { 
                    eb.HasNoKey(); 
                });
        }
    }
}


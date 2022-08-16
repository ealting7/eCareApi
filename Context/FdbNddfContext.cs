using eCareApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Context
{
    public class FdbNddfContext : DbContext
    {

        public DbSet<rDosed2> rDosed2s { get; set; }
        public DbSet<rGcnseq4> rGcnseq4s { get; set; }        
        public DbSet<rNdc14> rNdc14s { get; set; }
        public DbSet<rRouted3> rRouted3s { get; set; }
        

        public FdbNddfContext(DbContextOptions<FdbNddfContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<rNdc14>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<rDosed2>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<rGcnseq4>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<rRouted3>(eb =>
                {
                    eb.HasNoKey();
                });
        }
    }
}

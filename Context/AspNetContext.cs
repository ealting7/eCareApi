using eCareApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCareApi.Context
{
    public class AspNetContext : DbContext
    {

        public DbSet<AspNetMembership> AspNetMemberships { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<tblAdvanceDirectives> AdvancedDirectives { get; set; }
        public DbSet<IcmsUserReference> IcmsUserReferences { get; set; }
        


        public AspNetContext(DbContextOptions<AspNetContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<AspNetMembership>(eb =>
                {
                    eb.HasNoKey();
                })
                .Entity<AspNetUsers>(eb =>
                {
                    eb.HasNoKey();
                });
        }
    }
}

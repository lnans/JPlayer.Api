using JPlayer.Data.Dao.Configuration;
using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Data.Dao
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        // USR_ tables
        public DbSet<UsrUserDao> Users { get; set; }
        public DbSet<UsrUserProfileDao> UserProfiles { get; set; }
        public DbSet<UsrProfileDao> Profiles { get; set; }
        public DbSet<UsrProfileFunctionDao> ProfileFUnctions { get; set; }
        public DbSet<UsrFunctionDao> Functions { get; set; }

        // SYS_ tables
        public DbSet<SysInfoHistory> SystemInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USR_ Schema init
            modelBuilder.ApplyConfiguration(new UsrUserModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrUserProfileModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrProfileModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrProfileFunctionModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrFunctionModelConfiguration());

            // SYS_ schema init
            modelBuilder.ApplyConfiguration(new SysInfoHistoryModelConfiguration());

            // Factory data
            modelBuilder.Entity<UsrFunctionDao>().HasData(ApplicationFactoryData.Functions());
            modelBuilder.Entity<UsrProfileDao>().HasData(ApplicationFactoryData.Profiles());
            modelBuilder.Entity<UsrUserDao>().HasData(ApplicationFactoryData.Users());
            modelBuilder.Entity<UsrProfileFunctionDao>().HasData(ApplicationFactoryData.ProfileFunctions());
            modelBuilder.Entity<UsrUserProfileDao>().HasData(ApplicationFactoryData.UserProfiles());
        }
    }
}
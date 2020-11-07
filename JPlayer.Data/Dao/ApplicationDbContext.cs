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

        public DbSet<UsrUserDao> Users { get; set; }
        public DbSet<UsrProfileDao> Profiles { get; set; }
        public DbSet<UsrFunctionDao> Functions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsrUserModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrProfileModelConfiguration());
            modelBuilder.ApplyConfiguration(new UsrFunctionModelConfiguration());
        }
    }
}
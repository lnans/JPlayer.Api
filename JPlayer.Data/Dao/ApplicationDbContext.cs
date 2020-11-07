using JPlayer.Data.Dao.Configuration;
using JPlayer.Data.Dao.Model.User;
using Microsoft.EntityFrameworkCore;

namespace JPlayer.Data.Dao
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserDao> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserModelConfiguration());
        }
    }
}
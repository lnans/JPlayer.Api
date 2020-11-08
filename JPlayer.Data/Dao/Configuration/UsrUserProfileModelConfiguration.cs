using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    public class UsrUserProfileModelConfiguration : IEntityTypeConfiguration<UsrUserProfileDao>
    {
        public void Configure(EntityTypeBuilder<UsrUserProfileDao> builder)
        {
            builder.ToTable("USR_USER_PROFILE");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.UserId)
                .HasColumnName("USER_ID")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(table => table.ProfileId)
                .HasColumnName("PROFILE_ID")
                .HasColumnType("INT")
                .IsRequired();

            builder.HasAlternateKey(table => new {table.UserId, table.ProfileId});

            builder
                .HasOne(up => up.User)
                .WithMany(u => u.UserProfiles)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne(up => up.Profile)
                .WithMany(p => p.UserProfiles)
                .HasForeignKey(up => up.ProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
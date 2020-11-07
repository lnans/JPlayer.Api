using JPlayer.Data.Dao.Model.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    internal class UserModelConfiguration : IEntityTypeConfiguration<UserDao>
    {
        public void Configure(EntityTypeBuilder<UserDao> builder)
        {
            builder.ToTable("USR_USER");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.Login)
                .HasColumnName("LOGIN")
                .HasColumnType("NVARCHAR(20)")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(table => table.Deactivated)
                .HasColumnName("DEACTIVATED")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(table => table.CreationDate)
                .HasColumnName("CREATION_DATE")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(table => table.LastConnectionDate)
                .HasColumnName("LAST_CONNECTION_DATE")
                .HasColumnType("DATETIME");

            builder.HasAlternateKey(table => table.Login);
        }
    }
}
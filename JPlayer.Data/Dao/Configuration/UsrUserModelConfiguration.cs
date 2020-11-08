using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    internal class UsrUserModelConfiguration : IEntityTypeConfiguration<UsrUserDao>
    {
        public void Configure(EntityTypeBuilder<UsrUserDao> builder)
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

            builder.Property(table => table.ReadOnly)
                .HasColumnName("READONLY")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasAlternateKey(table => table.Login);
        }
    }
}
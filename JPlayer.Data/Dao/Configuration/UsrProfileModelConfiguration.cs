using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    public class UsrProfileModelConfiguration : IEntityTypeConfiguration<UsrProfileDao>
    {
        public void Configure(EntityTypeBuilder<UsrProfileDao> builder)
        {
            builder.ToTable("USR_PROFILE");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.Name)
                .HasColumnName("NAME")
                .HasColumnType("NVARCHAR(50)")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(table => table.ReadOnly)
                .HasColumnName("READONLY")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder.HasAlternateKey(table => table.Name);
        }
    }
}
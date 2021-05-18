using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    public class SysInfoHistoryModelConfiguration : IEntityTypeConfiguration<SysInfoHistory>
    {
        public void Configure(EntityTypeBuilder<SysInfoHistory> builder)
        {
            builder.ToTable("SYS_INFO_HISTORY");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.UnixTimeSeconds)
                .HasColumnName("UNIX_TIME_SECONDS")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(table => table.Total)
                .HasColumnName("TOTAL")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(table => table.Free)
                .HasColumnName("FREE")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(table => table.Used)
                .HasColumnName("USED")
                .HasColumnType("INT")
                .HasDefaultValue(false)
                .IsRequired();

            builder
                .HasIndex(table => table.UnixTimeSeconds)
                .IsUnique();
        }
    }
}
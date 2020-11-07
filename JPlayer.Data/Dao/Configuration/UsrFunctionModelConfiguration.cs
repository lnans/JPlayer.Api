using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    public class UsrFunctionModelConfiguration : IEntityTypeConfiguration<UsrFunctionDao>
    {
        public void Configure(EntityTypeBuilder<UsrFunctionDao> builder)
        {
            builder.ToTable("USR_FUNCTION");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.FunctionCode)
                .HasColumnName("FUNCTION_CODE")
                .HasColumnType("NVARCHAR(20)")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(table => table.Name)
                .HasColumnName("NAME")
                .HasColumnType("NVARCHAR(20)")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasAlternateKey(table => table.FunctionCode);
        }
    }
}
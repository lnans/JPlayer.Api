using JPlayer.Data.Dao.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JPlayer.Data.Dao.Configuration
{
    public class UsrProfileFunctionModelConfiguration : IEntityTypeConfiguration<UsrProfileFunctionDao>
    {
        public void Configure(EntityTypeBuilder<UsrProfileFunctionDao> builder)
        {
            builder.ToTable("USR_PROFILE_FUNCTION");

            builder.Property(table => table.Id)
                .HasColumnName("ID");

            builder.Property(table => table.ProfileId)
                .HasColumnName("PROFILE_ID")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(table => table.FunctionId)
                .HasColumnName("FUNCTION_ID")
                .HasColumnType("INT")
                .IsRequired();

            builder.HasAlternateKey(table => new {table.ProfileId, table.FunctionId});

            builder
                .HasOne(pf => pf.Profile)
                .WithMany(p => p.ProfileFunctions)
                .HasForeignKey(pf => pf.ProfileId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne(pf => pf.Function)
                .WithMany(f => f.ProfileFunctions)
                .HasForeignKey(pf => pf.FunctionId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
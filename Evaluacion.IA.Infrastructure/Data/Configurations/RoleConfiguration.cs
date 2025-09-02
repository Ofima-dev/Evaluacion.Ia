using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evaluacion.IA.Domain.Entities;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Name")
                .HasConversion(
                    description => description.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Description.Create(value));

            builder.HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);
        }
    }
}

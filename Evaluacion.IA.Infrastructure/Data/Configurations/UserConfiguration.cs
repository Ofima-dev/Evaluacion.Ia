using Evaluacion.IA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email).HasColumnName("email")
                .IsRequired()
                .HasMaxLength(150)
                .HasConversion(
                    email => email.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Email.Create(value));

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash).HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.RoleId).HasColumnName("role_id")
                .IsRequired();

            builder.Property(u => u.CreateAt).HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evaluacion.IA.Domain.Entities;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .ValueGeneratedOnAdd();

            builder.Property(pi => pi.ProductId)
                .IsRequired();

            builder.Property(pi => pi.ImageUrl)
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    url => url.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Url.Create(value));

            builder.Property(pi => pi.Alt)
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    alt => alt.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Description.Create(value));

            builder.Property(pi => pi.Order)
                .IsRequired();

            builder.Property(pi => pi.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pi => pi.CreateAt)
                .IsRequired();

            builder.Property(pi => pi.UpdateAt)
                .IsRequired(false);

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pi => new { pi.ProductId, pi.Order })
                .IsUnique();

            builder.HasIndex(pi => new { pi.ProductId, pi.IsPrimary })
                .HasFilter("IsPrimary = 1");
        }
    }
}

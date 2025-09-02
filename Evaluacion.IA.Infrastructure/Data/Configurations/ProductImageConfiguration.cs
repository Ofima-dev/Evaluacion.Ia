using Evaluacion.IA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("product_image");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .ValueGeneratedOnAdd();

            builder.Property(pi => pi.ProductId).HasColumnName("product_id")
                .IsRequired();

            builder.Property(pi => pi.ImageUrl).HasColumnName("url")
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    url => url.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Url.Create(value));

            builder.Property(pi => pi.Alt).HasColumnName("alt_text")
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    alt => alt.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Description.Create(value));

            builder.Property(pi => pi.Order).HasColumnName("sort_order")
                .IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pi => new { pi.ProductId, pi.Order })
                .IsUnique();
        }
    }
}

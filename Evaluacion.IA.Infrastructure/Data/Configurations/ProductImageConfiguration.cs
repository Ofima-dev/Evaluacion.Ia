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

            builder.Property(pi => pi.Url)
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    url => url.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Url.Create(value));

            builder.Property(pi => pi.AltText)
                .HasMaxLength(150)
                .IsRequired(false);

            builder.Property(pi => pi.SortOrder)
                .IsRequired();

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

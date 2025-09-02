using Evaluacion.IA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("product");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Sku).HasColumnName("sku")
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(
                    sku => sku.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Sku.Create(value));

            builder.Property(p => p.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(
                    name => name.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Name.Create(value));

            builder.Property(p => p.Description).HasColumnName("description")
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    description => description.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Description.Create(value));

            builder.Property(p => p.Price).HasColumnName("price")
                .IsRequired()
                .HasPrecision(18, 2)
                .HasConversion(
                    price => price.Amount,
                    value => Evaluacion.IA.Domain.ValueObjects.Money.Create(value, "USD"));

            builder.Property(p => p.CategoryId).HasColumnName("category_id")
                .IsRequired(false);

            builder.Property(p => p.IsActive).HasColumnName("is_active")
                .IsRequired();

            builder.Property(p => p.CreateAt).HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId);
        }
    }
}

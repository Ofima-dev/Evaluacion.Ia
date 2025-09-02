using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evaluacion.IA.Domain.Entities;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(50)
                .HasConversion(
                    sku => sku.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Sku.Create(value));

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200)
                .HasConversion(
                    name => name.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Name.Create(value));

            builder.Property(p => p.Description)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    description => description.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Description.Create(value));

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2)
                .HasConversion(
                    price => price.Amount,
                    value => Evaluacion.IA.Domain.ValueObjects.Money.Create(value, "USD"));

            builder.Property(p => p.CategoryId)
                .IsRequired(false);

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.CreateAt)
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

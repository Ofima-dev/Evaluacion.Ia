using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Evaluacion.IA.Domain.Entities;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(
                    name => name.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Name.Create(value));

            builder.Property(c => c.ParentCategoryId)
                .IsRequired(false);

            builder.Property(c => c.IsActive)
                .IsRequired();

            builder.Property(c => c.CreateAt)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}

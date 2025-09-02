using Evaluacion.IA.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evaluacion.IA.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("category");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();


            builder.Property(c => c.Name).HasColumnName("name")
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(
                    name => name.Value,
                    value => Evaluacion.IA.Domain.ValueObjects.Name.Create(value));

            builder.Property(c => c.ParentCategoryId).HasColumnName("parent_category_id")
                .IsRequired(false);

            builder.Property(c => c.IsActive).HasColumnName("is_active")
                .IsRequired();

            builder.Property(c => c.CreateAt).HasColumnName("created_at")
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

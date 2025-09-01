using System;
using System.Collections.Generic;

namespace Evaluacion.IA.Domain.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }
        private readonly List<Category> _subCategories = new();
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();
        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }
        private readonly List<Product> _products = [];
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category() { Name = string.Empty; }

        public Category(string name, bool isActive, int? parentCategoryId = null)
        {
            Name = name;
            IsActive = isActive;
            ParentCategoryId = parentCategoryId;
            CreateAt = DateTime.UtcNow;
        }

        public void AddSubCategory(Category category)
        {
            if (!_subCategories.Contains(category))
                _subCategories.Add(category);
        }

        public void AddProduct(Product product)
        {
            if (!_products.Contains(product))
                _products.Add(product);
        }
    }
}

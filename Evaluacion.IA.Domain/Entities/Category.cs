using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class Category : Entity
    {
        public Name Name { get; private set; }
        public int? ParentCategoryId { get; private set; }
        public Category? ParentCategory { get; private set; }
        private readonly List<Category> _subCategories = [];
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();
        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }
        private readonly List<Product> _products = [];
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private Category()
        {
            Name = Name.Create("Temp");
        }

        public Category(Name name, bool isActive = true, int? parentCategoryId = null)
        {
            Name = name;
            IsActive = isActive;
            ParentCategoryId = parentCategoryId;
            CreateAt = DateTime.UtcNow;
        }

        public void UpdateDetails(Name name, bool isActive)
        {
            Name = name;
            IsActive = isActive;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
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

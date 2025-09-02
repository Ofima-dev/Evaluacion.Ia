using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class Product : Entity
    {
        public Sku Sku { get; private set; }
        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public Money Price { get; private set; }
        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }
        private readonly List<ProductImage> _productImages = [];
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

        private Product()
        {
            Sku = Sku.Create("TEMP-001");
            Name = Name.Create("Temp");
            Description = Description.Create("Temp");
            Price = Money.Create(0, "USD");
        }


        public Product(Sku sku, Name name, Description description, Money price, int? categoryId, bool isActive = true)
        {
            Sku = sku;
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            IsActive = isActive;
            CreateAt = DateTime.UtcNow;
        }

        public void UpdateDetails(Name name, Description description, Money price, int? categoryId, bool isActive)
        {
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            IsActive = isActive;
        }

        public void UpdatePrice(Money newPrice)
        {
            Price = newPrice;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void SetCategory(Category category)
        {
            Category = category;
            CategoryId = category.Id;
        }

        public void AddProductImage(ProductImage image)
        {
            if (!_productImages.Contains(image))
                _productImages.Add(image);
        }

        public void RemoveProductImage(ProductImage image)
        {
            _productImages.Remove(image);
        }
    }
}

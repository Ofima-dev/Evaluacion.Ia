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
        public int CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }
        private readonly List<ProductImage> _productImages = [];
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

        private Product() { 
            Sku = Sku.Create("TEMP-001"); 
            Name = Name.Create("Temp"); 
            Description = Description.Create("Temp"); 
            Price = Money.Create(0);
        }

        public Product(Sku sku, Name name, Description description, Money price, int categoryId, bool isActive)
        {
            Sku = sku;
            Name = name;
            Description = description;
            Price = price;
            CategoryId = categoryId;
            IsActive = isActive;
            CreateAt = DateTime.UtcNow;
        }

        public void AddProductImage(ProductImage image)
        {
            if (!_productImages.Contains(image))
                _productImages.Add(image);
        }
    }
}

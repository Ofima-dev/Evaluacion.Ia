using System;
using System.Collections.Generic;

namespace Evaluacion.IA.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Sku { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreateAt { get; private set; }
        private readonly List<ProductImage> _productImages = new();
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

    private Product() { Sku = string.Empty; Name = string.Empty; Description = string.Empty; }

        public Product(string sku, string name, string description, decimal price, int categoryId, bool isActive)
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

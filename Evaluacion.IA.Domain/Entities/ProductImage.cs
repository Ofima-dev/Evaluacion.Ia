using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class ProductImage : Entity
    {
        public int ProductId { get; private set; }
        public Product? Product { get; private set; }
        public Url ImageUrl { get; private set; }
        public Description Alt { get; private set; }
        public int Order { get; private set; }

        private ProductImage() { 
            ImageUrl = Url.Create("https://temp.com"); 
            Alt = Description.Create("Temp alt text"); 
        }

        public ProductImage(Url imageUrl, Description alt, int order, int productId, bool isPrimary = false)
        {
            ImageUrl = imageUrl;
            Alt = alt;
            Order = order;
            ProductId = productId;
        }

        public void UpdateDetails(Url imageUrl, Description alt, int order, bool isPrimary)
        {
            ImageUrl = imageUrl;
            Alt = alt;
            Order = order;
        }

        public void SetProduct(Product product)
        {
            Product = product;
            ProductId = product.Id;
        }
    }
}

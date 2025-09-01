using Evaluacion.IA.Domain.Primitives;
using Evaluacion.IA.Domain.ValueObjects;

namespace Evaluacion.IA.Domain.Entities
{
    public class ProductImage : Entity
    {
        public int ProductId { get; private set; }
        public Product? Product { get; private set; }
        public Url Url { get; private set; }
        public string AltText { get; private set; }
        public int SortOrder { get; private set; }

        private ProductImage() { Url = Url.Create("https://temp.com"); AltText = string.Empty; }

        public ProductImage(Url url, string altText, int sortOrder, int productId)
        {
            Url = url;
            AltText = altText;
            SortOrder = sortOrder;
            ProductId = productId;
        }
    }
}

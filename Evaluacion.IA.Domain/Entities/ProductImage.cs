namespace Evaluacion.IA.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; private set; }
        public int ProductId { get; private set; }
        public Product? Product { get; private set; }
        public string Url { get; private set; }
        public string AltText { get; private set; }
        public int SortOrder { get; private set; }

    private ProductImage() { Url = string.Empty; AltText = string.Empty; }

        public ProductImage(string url, string altText, int sortOrder, int productId)
        {
            Url = url;
            AltText = altText;
            SortOrder = sortOrder;
            ProductId = productId;
        }
    }
}

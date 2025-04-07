using techXpress.Services.DTOs.Products;

namespace techXpress.UI.VMs.Products
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
    }

    public static class ProductDetailsMapping
    {
        public static ProductDetailsVM ToProductDetailsVM(this ProductDTO product)
        {
            return new ProductDetailsVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.Image,
                StockQuantity = product.StockQuantity,
                Price = product.Price
            };
        }
    }
}

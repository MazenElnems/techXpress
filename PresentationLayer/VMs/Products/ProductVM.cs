using BusinessLogicLayer.DTOs.Products;

namespace PresentationLayer.VMs.Products
{
    public class ProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    public static class ProductDtoToProductVM
    {
        public static ProductVM ToProductVM(this ProductDTO dto)
        {
            return new ProductVM
            {
                Id = dto.Id,
                Price = dto.Price,
                Description = dto.Description,
                Image = dto.Image,
                Name = dto.Name
            };
        }
    }
}

using BusinessLogicLayer.DTOs.Product;

namespace PresentationLayer.VMs.Products
{
    public static class ProductDtoToProductVM
    {
        public static ProductVM ToProductVM(this ProductDTO dto)
        {
            return new ProductVM
            {
                Price = dto.Price,
                Description = dto.Description,  
                Image = dto.Image,
                Name = dto.Name 
            };
        }
    }
}

using techXpress.Services.DTOs.Products;

namespace techXpress.UI.VMs.Products
{
    public class ProductReviewVM
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }    
    }
    public static class ProductReviewVMExtensions
    {
        public static ProductReviewDTO ToDto(this ProductReviewVM productReviewDTO)
        {
            return new ProductReviewDTO
            {
                Comment = productReviewDTO.Comment,
                CreatedAt = productReviewDTO.CreatedAt,
                Rating = productReviewDTO.Rating,
                UserId = productReviewDTO.UserId,
                UserName = productReviewDTO.UserName,
            };
        }
    }
}

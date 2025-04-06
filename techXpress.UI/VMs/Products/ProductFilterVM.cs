using System.ComponentModel.DataAnnotations;

namespace techXpress.UI.VMs.Products
{
    public class ProductFilterVM
    {
        [Range(0, double.MaxValue)]
        public decimal MinPrice { get; set; } = default;
        [Range(0, double.MaxValue)]
        public decimal MaxPrice { get; set; } = default;
        public string? SearchTerm { get; set; }
        public string? SearchBy { get; set; }
        public int? CategoryId { get; set; }
    }
}

namespace PresentationLayer.VMs.ShoppingCart
{
    public class ShoppingCartItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal SubTotal { get; set; }
        public int Quantity { get; set; }
    }
}

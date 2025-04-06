namespace techXpress.UI.VMs.ShoppingCart
{
    public class ShoppingCartVM
    {
        public int ShoppingCartId { get; set; }
        public List<ShoppingCartItemVM> CartItems { get; set; } = new();
        public decimal Total { get; set; }
    }
}

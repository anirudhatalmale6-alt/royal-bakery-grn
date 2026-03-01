namespace RoyalBakeryGrn.Data.ViewModels
{
    public class GRNItemViewModel
    {
        public int MenuItemId { get; set; }      // ✅ Add this
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

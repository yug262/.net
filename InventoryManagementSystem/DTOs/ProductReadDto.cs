namespace InventoryAPI.DTOs
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty; // included per prompt "include category name in product list response"
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

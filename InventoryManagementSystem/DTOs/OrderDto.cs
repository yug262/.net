using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class OrderCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        public int? CustomerId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class OrderReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public decimal UnitPurchasePrice { get; set; }
        public decimal TotalRevenue => Quantity * UnitSellingPrice;
        public decimal TotalProfit => Quantity * (UnitSellingPrice - UnitPurchasePrice);
        public DateTime CreatedAt { get; set; }
    }
}

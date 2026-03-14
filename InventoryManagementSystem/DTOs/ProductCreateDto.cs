using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class ProductCreateDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "PurchasePrice must be >= 0")]
        public decimal PurchasePrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "SellingPrice must be > 0")]
        public decimal SellingPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be >= 0")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}

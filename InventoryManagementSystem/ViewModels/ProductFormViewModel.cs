using System.ComponentModel.DataAnnotations;
using InventoryAPI.DTOs;

namespace InventoryAPI.ViewModels
{
    public class ProductFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU is required")]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be >= 0")]
        public decimal PurchasePrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be > 0")]
        public decimal SellingPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be >= 0")]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Dropdown data
        public List<CategoryReadDto> Categories { get; set; } = new();

        public bool IsEditMode => Id.HasValue;
    }
}

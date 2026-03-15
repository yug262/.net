using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class SupplierCreateDto
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [MaxLength(200)]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? CompanyName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }
}

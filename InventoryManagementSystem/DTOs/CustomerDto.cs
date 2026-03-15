using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class CustomerCreateDto
    {
        [Required(ErrorMessage = "Customer name is required")]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }
    }

    public class CustomerUpdateDto
    {
        [Required(ErrorMessage = "Customer name is required")]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

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

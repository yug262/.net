using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.ViewModels
{
    public class SupplierFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [MaxLength(200)]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Phone]
        [MaxLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(150)]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [MaxLength(500)]
        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}

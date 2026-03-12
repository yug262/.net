using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;
    }
}

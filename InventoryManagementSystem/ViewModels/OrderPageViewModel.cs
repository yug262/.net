using InventoryAPI.DTOs;

namespace InventoryAPI.ViewModels
{
    public class OrderPageViewModel
    {
        public List<OrderReadDto> Orders { get; set; } = new();
        public List<ProductReadDto> AvailableProducts { get; set; } = new();
        public List<CustomerReadDto> AvailableCustomers { get; set; } = new();
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

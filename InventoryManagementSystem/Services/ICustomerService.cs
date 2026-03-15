using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface ICustomerService
    {
        Task<List<CustomerReadDto>> GetAllAsync(int userId);
        Task<CustomerReadDto?> GetByIdAsync(int id, int userId);
        Task<CustomerReadDto> CreateAsync(CustomerCreateDto dto, int userId);
        Task<CustomerReadDto?> UpdateAsync(int id, CustomerUpdateDto dto, int userId);
        Task<(bool Success, string? Error)> DeleteAsync(int id, int userId);
        Task<List<CustomerReadDto>> SearchAsync(string query, int userId);
        Task<List<OrderReadDto>> GetOrdersForCustomerAsync(int customerId, int userId);
    }
}

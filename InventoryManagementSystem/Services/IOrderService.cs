using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IOrderService
    {
        Task<List<OrderReadDto>> GetAllAsync(int userId);
        Task<(OrderReadDto? Order, string? Error)> CreateAsync(OrderCreateDto dto, int userId);
        Task<(bool Success, string? Error)> DeleteAsync(int id, int userId);
    }
}

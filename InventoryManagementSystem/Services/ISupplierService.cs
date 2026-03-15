using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierReadDto>> GetAllAsync(int userId);
        Task<SupplierReadDto?> GetByIdAsync(int id, int userId);
        Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto, int userId);
        Task<SupplierReadDto?> UpdateAsync(int id, SupplierUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<List<SupplierReadDto>> SearchAsync(string query, int userId);
    }
}

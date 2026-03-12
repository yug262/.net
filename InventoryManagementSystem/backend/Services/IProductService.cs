using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IProductService
    {
        Task<List<ProductReadDto>> GetAllAsync(int userId);
        Task<ProductReadDto?> GetByIdAsync(int id, int userId);
        Task<ProductReadDto> CreateAsync(ProductCreateDto dto, int userId);
        Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
        Task<List<ProductReadDto>> SearchAsync(string query, int userId);
        Task<List<ProductReadDto>> GetLowStockAsync(int userId);
    }
}

using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IProductService
    {
        Task<List<ProductReadDto>> GetAllAsync();
        Task<ProductReadDto?> GetByIdAsync(int id);
        Task<ProductReadDto> CreateAsync(ProductCreateDto dto);
        Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<ProductReadDto>> SearchAsync(string query);
        Task<List<ProductReadDto>> GetLowStockAsync();
    }
}

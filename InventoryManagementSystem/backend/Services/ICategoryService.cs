using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryReadDto>> GetAllAsync(int userId);
        Task<CategoryReadDto?> GetByIdAsync(int id, int userId);
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, int userId);
        Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto, int userId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}

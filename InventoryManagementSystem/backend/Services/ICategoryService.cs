using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryReadDto>> GetAllAsync();
        Task<CategoryReadDto?> GetByIdAsync(int id);
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto);
        Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}

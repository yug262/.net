using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryReadDto>> GetAllAsync()
        {
            return await _context.Categories
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CategoryReadDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryReadDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            
            return new CategoryReadDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();
            
            return new CategoryReadDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return false;

            if (category.Products.Any())
                throw new InvalidOperationException("Cannot delete category with existing products.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        private static ProductReadDto MapToDto(Product p) => new ProductReadDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            SKU = p.SKU,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.CategoryName ?? string.Empty,
            PurchasePrice = p.PurchasePrice,
            SellingPrice = p.SellingPrice,
            Quantity = p.Quantity,
            Description = p.Description ?? string.Empty,
            CreatedAt = p.CreatedAt
        };

        public async Task<List<ProductReadDto>> GetAllAsync(int userId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id, int userId)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, int userId)
        {
            // Verify category belongs to this user
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
            if (!categoryExists)
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            // Verify unique SKU per user
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU == dto.SKU && p.UserId == userId);
            if (skuExists)
                throw new ArgumentException($"Product with SKU '{dto.SKU}' already exists.");

            var product = new Product
            {
                ProductName = dto.ProductName,
                SKU = dto.SKU,
                CategoryId = dto.CategoryId,
                PurchasePrice = dto.PurchasePrice,
                SellingPrice = dto.SellingPrice,
                Quantity = dto.Quantity,
                Description = dto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return (await GetByIdAsync(product.Id, userId))!;
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto, int userId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (product == null) return null;

            // Verify category belongs to this user
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
            if (!categoryExists)
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            // Verify unique SKU (excluding self) per user
            var skuExists = await _context.Products
                .AnyAsync(p => p.SKU == dto.SKU && p.Id != id && p.UserId == userId);
            if (skuExists)
                throw new ArgumentException($"Product with SKU '{dto.SKU}' already exists.");

            product.ProductName = dto.ProductName;
            product.SKU = dto.SKU;
            product.CategoryId = dto.CategoryId;
            product.PurchasePrice = dto.PurchasePrice;
            product.SellingPrice = dto.SellingPrice;
            product.Quantity = dto.Quantity;
            product.Description = dto.Description;

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(product.Id, userId))!;
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductReadDto>> SearchAsync(string query, int userId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId &&
                    (p.ProductName.Contains(query) || p.SKU.Contains(query)))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductReadDto>> GetLowStockAsync(int userId)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.UserId == userId && p.Quantity < 5)
                .OrderBy(p => p.Quantity)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }
    }
}

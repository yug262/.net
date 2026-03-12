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

        public async Task<List<ProductReadDto>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (product == null) return null;
            return MapToDto(product);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto)
        {
            // Verify category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            // Verify unique SKU
            var skuExists = await _context.Products.AnyAsync(p => p.SKU == dto.SKU);
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
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Reload with Category navigation property for accurate output mapping
            return (await GetByIdAsync(product.Id))!;
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            // Verify category exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"Category with Id {dto.CategoryId} does not exist.");

            // Verify unique SKU excluding current product
            var skuExists = await _context.Products.AnyAsync(p => p.SKU == dto.SKU && p.Id != id);
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

            return (await GetByIdAsync(product.Id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductReadDto>> SearchAsync(string query)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.ProductName.Contains(query) || p.SKU.Contains(query))
                .OrderBy(p => p.ProductName)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductReadDto>> GetLowStockAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Quantity < 5)
                .OrderBy(p => p.Quantity)
                .ToListAsync();
                
            return products.Select(MapToDto).ToList();
        }
    }
}

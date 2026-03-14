using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderReadDto>> GetAllAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderReadDto
                {
                    Id = o.Id,
                    ProductId = o.ProductId,
                    ProductName = o.Product!.ProductName,
                    SKU = o.Product!.SKU,
                    Quantity = o.Quantity,
                    UnitSellingPrice = o.UnitSellingPrice,
                    UnitPurchasePrice = o.UnitPurchasePrice,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<(OrderReadDto? Order, string? Error)> CreateAsync(OrderCreateDto dto, int userId)
        {
            // Only allow ordering from the user's own products
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.UserId == userId);

            if (product == null)
                return (null, "Product not found.");

            if (product.Quantity <= 0)
                return (null, $"'{product.ProductName}' is out of stock.");

            if (product.Quantity < dto.Quantity)
                return (null, $"Only {product.Quantity} unit(s) of '{product.ProductName}' available.");

            // Snapshot prices at order time
            var order = new Order
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitSellingPrice = product.SellingPrice,
                UnitPurchasePrice = product.PurchasePrice,
                CreatedAt = DateTime.UtcNow
            };

            // Deduct stock
            product.Quantity -= dto.Quantity;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Reload with product navigation
            await _context.Entry(order).Reference(o => o.Product).LoadAsync();

            return (new OrderReadDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                ProductName = order.Product!.ProductName,
                SKU = order.Product!.SKU,
                Quantity = order.Quantity,
                UnitSellingPrice = order.UnitSellingPrice,
                UnitPurchasePrice = order.UnitPurchasePrice,
                CreatedAt = order.CreatedAt
            }, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return (false, "Order not found.");

            // Restore stock
            if (order.Product != null)
                order.Product.Quantity += order.Quantity;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}

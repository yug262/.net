using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsDto> GetDashboardAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            // Inventory value: sum of (quantity * purchase price) for all current products
            var inventoryValue = await _context.Products
                .Where(p => p.UserId == userId)
                .SumAsync(p => (decimal)p.Quantity * p.PurchasePrice);

            // Total items sold (sum of all order quantities)
            var totalSold = await _context.Orders
                .Where(o => o.UserId == userId)
                .SumAsync(o => (int?)o.Quantity) ?? 0;

            // Today's profit: sum of (selling - purchase) * qty for today's orders
            var todayProfit = await _context.Orders
                .Where(o => o.UserId == userId && o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)(o.Quantity * (o.UnitSellingPrice - o.UnitPurchasePrice))) ?? 0m;

            return new DashboardStatsDto
            {
                TotalCategories = await _context.Categories.CountAsync(c => c.UserId == userId),
                TotalProducts = await _context.Products.CountAsync(p => p.UserId == userId),
                LowStockProducts = await _context.Products.CountAsync(p => p.UserId == userId && p.Quantity > 0 && p.Quantity < 5),
                OutOfStockProducts = await _context.Products.CountAsync(p => p.UserId == userId && p.Quantity == 0),
                InventoryValue = inventoryValue,
                TotalSold = totalSold,
                TodayProfit = todayProfit
            };
        }
    }
}

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

        public async Task<DashboardStatsDto> GetDashboardAsync()
        {
            return new DashboardStatsDto
            {
                TotalCategories = await _context.Categories.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                LowStockProducts = await _context.Products.CountAsync(p => p.Quantity > 0 && p.Quantity < 5),
                OutOfStockProducts = await _context.Products.CountAsync(p => p.Quantity == 0)
            };
        }
    }
}

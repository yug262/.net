using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardAsync(int userId);
        Task<decimal> GetProfitAsync(DateTime startDate, DateTime endDate, int userId);
    }
}

using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardAsync();
    }
}

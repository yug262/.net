using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /
        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetDashboardAsync(GetUserId());
            return View(stats);
        }

        // GET: /Home/GetProfit
        [HttpGet]
        public async Task<IActionResult> GetProfit(DateTime startDate, DateTime endDate)
        {
            var profit = await _dashboardService.GetProfitAsync(startDate, endDate, GetUserId());
            return Json(new { profit });
        }
    }
}

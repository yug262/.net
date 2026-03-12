using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private int? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim) : null;
        }

        // GET: api/dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var stats = await _dashboardService.GetDashboardAsync(userId.Value);
            return Ok(stats);
        }
    }
}

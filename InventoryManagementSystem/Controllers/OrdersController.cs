using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim) : null;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var orders = await _orderService.GetAllAsync(userId.Value);
            return Ok(orders);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var (order, error) = await _orderService.CreateAsync(dto, userId.Value);

            if (error != null)
                return BadRequest(new { message = error });

            return Ok(order);
        }

        // DELETE: api/orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var (success, error) = await _orderService.DeleteAsync(id, userId.Value);

            if (!success)
                return NotFound(new { message = error });

            return Ok(new { message = "Order cancelled. Stock has been restored." });
        }
    }
}

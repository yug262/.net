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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        private int? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim) : null;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var products = await _productService.GetAllAsync(userId.Value);
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var product = await _productService.GetByIdAsync(id, userId.Value);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            try
            {
                var product = await _productService.CreateAsync(dto, userId.Value);
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            try
            {
                var product = await _productService.UpdateAsync(id, dto, userId.Value);
                if (product == null)
                    return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var result = await _productService.DeleteAsync(id, userId.Value);
            if (!result)
                return NotFound(new { message = "Product not found" });

            return Ok(new { message = "Product deleted successfully" });
        }

        // GET: api/products/search?query=abc
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { message = "Search query is required" });

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var products = await _productService.SearchAsync(query, userId.Value);
            return Ok(products);
        }

        // GET: api/products/lowstock
        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStock()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var products = await _productService.GetLowStockAsync(userId.Value);
            return Ok(products);
        }
    }
}
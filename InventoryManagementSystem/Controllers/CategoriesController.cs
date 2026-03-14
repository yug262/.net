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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private int? TryGetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim) : null;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var categories = await _categoryService.GetAllAsync(userId.Value);
            return Ok(categories);
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var category = await _categoryService.GetByIdAsync(id, userId.Value);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var category = await _categoryService.CreateAsync(dto, userId.Value);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            var category = await _categoryService.UpdateAsync(id, dto, userId.Value);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var userId = TryGetUserId();
            if (userId == null) return Unauthorized(new { message = "Session expired. Please log in again." });

            try
            {
                var result = await _categoryService.DeleteAsync(id, userId.Value);
                if (!result)
                    return NotFound(new { message = "Category not found" });

                return Ok(new { message = "Category deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

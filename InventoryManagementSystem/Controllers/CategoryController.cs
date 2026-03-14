using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync(GetUserId());
            return View(categories);
        }

        // GET: /Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _categoryService.CreateAsync(dto, GetUserId());
            TempData["Message"] = "Category created successfully";
            return RedirectToAction("Index");
        }

        // GET: /Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id, GetUserId());
            if (category == null) return NotFound();

            var dto = new CategoryUpdateDto { CategoryName = category.CategoryName };
            ViewBag.CategoryId = id;
            return View(dto);
        }

        // POST: /Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = id;
                return View(dto);
            }

            var result = await _categoryService.UpdateAsync(id, dto, GetUserId());
            if (result == null) return NotFound();

            TempData["Message"] = "Category updated successfully";
            return RedirectToAction("Index");
        }

        // POST: /Category/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteAsync(id, GetUserId());
                TempData["Message"] = result
                    ? "Category deleted successfully"
                    : "Category not found";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}

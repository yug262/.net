using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;
using InventoryAPI.ViewModels;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Product?query=abc
        public async Task<IActionResult> Index(string? query)
        {
            var userId = GetUserId();
            List<ProductReadDto> products;

            if (!string.IsNullOrWhiteSpace(query))
                products = await _productService.SearchAsync(query, userId);
            else
                products = await _productService.GetAllAsync(userId);

            ViewBag.SearchQuery = query;
            return View(products);
        }

        // GET: /Product/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync(GetUserId());
            var vm = new ProductFormViewModel { Categories = categories };
            return View(vm);
        }

        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel vm)
        {
            vm.Categories = await _categoryService.GetAllAsync(GetUserId());

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var dto = new ProductCreateDto
                {
                    ProductName = vm.ProductName,
                    SKU = vm.SKU,
                    CategoryId = vm.CategoryId,
                    PurchasePrice = vm.PurchasePrice,
                    SellingPrice = vm.SellingPrice,
                    Quantity = vm.Quantity,
                    Description = vm.Description
                };
                await _productService.CreateAsync(dto, GetUserId());
                TempData["Message"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        // GET: /Product/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();
            var product = await _productService.GetByIdAsync(id, userId);
            if (product == null) return NotFound();

            var categories = await _categoryService.GetAllAsync(userId);
            var vm = new ProductFormViewModel
            {
                Id = id,
                ProductName = product.ProductName,
                SKU = product.SKU,
                CategoryId = product.CategoryId,
                PurchasePrice = product.PurchasePrice,
                SellingPrice = product.SellingPrice,
                Quantity = product.Quantity,
                Description = product.Description,
                Categories = categories
            };
            return View(vm);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel vm)
        {
            vm.Id = id;
            vm.Categories = await _categoryService.GetAllAsync(GetUserId());

            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var dto = new ProductUpdateDto
                {
                    ProductName = vm.ProductName,
                    SKU = vm.SKU,
                    CategoryId = vm.CategoryId,
                    PurchasePrice = vm.PurchasePrice,
                    SellingPrice = vm.SellingPrice,
                    Quantity = vm.Quantity,
                    Description = vm.Description
                };
                var result = await _productService.UpdateAsync(id, dto, GetUserId());
                if (result == null) return NotFound();

                TempData["Message"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        // POST: /Product/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id, GetUserId());
            TempData["Message"] = result ? "Product deleted successfully" : "Product not found";
            return RedirectToAction("Index");
        }

        // GET: /Product/LowStock
        public async Task<IActionResult> LowStock()
        {
            var products = await _productService.GetLowStockAsync(GetUserId());
            return View(products);
        }
    }
}

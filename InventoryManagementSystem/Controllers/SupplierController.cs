using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;
using InventoryAPI.ViewModels;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Supplier
        public async Task<IActionResult> Index(string? searchQuery)
        {
            var userId = GetUserId();
            List<SupplierReadDto> suppliers;

            if (!string.IsNullOrWhiteSpace(searchQuery))
                suppliers = await _supplierService.SearchAsync(searchQuery, userId);
            else
                suppliers = await _supplierService.GetAllAsync(userId);

            ViewData["CurrentFilter"] = searchQuery;
            return View(suppliers);
        }

        // GET: /Supplier/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id, GetUserId());
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // GET: /Supplier/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SupplierFormViewModel());
        }

        // POST: /Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new SupplierCreateDto
            {
                SupplierName = vm.SupplierName,
                CompanyName = vm.CompanyName,
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address
            };
            await _supplierService.CreateAsync(dto, GetUserId());
            TempData["SuccessMessage"] = "Supplier added successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Supplier/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id, GetUserId());
            if (supplier == null) return NotFound();

            var vm = new SupplierFormViewModel
            {
                Id = id,
                SupplierName = supplier.SupplierName,
                CompanyName = supplier.CompanyName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address
            };
            return View(vm);
        }

        // POST: /Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierFormViewModel vm)
        {
            vm.Id = id;

            if (!ModelState.IsValid)
                return View(vm);

            var dto = new SupplierUpdateDto
            {
                SupplierName = vm.SupplierName,
                CompanyName = vm.CompanyName,
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address
            };
            var result = await _supplierService.UpdateAsync(id, dto, GetUserId());
            if (result == null) return NotFound();

            TempData["SuccessMessage"] = "Supplier updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.DeleteAsync(id, GetUserId());
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Supplier deleted successfully." : "Supplier not found.";
            return RedirectToAction("Index");
        }
    }
}

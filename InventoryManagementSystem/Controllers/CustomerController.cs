using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;
using InventoryAPI.ViewModels;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Customer
        public async Task<IActionResult> Index(string? searchQuery)
        {
            var userId = GetUserId();
            List<CustomerReadDto> customers;

            if (!string.IsNullOrWhiteSpace(searchQuery))
                customers = await _customerService.SearchAsync(searchQuery, userId);
            else
                customers = await _customerService.GetAllAsync(userId);

            ViewData["CurrentFilter"] = searchQuery;
            return View(customers);
        }

        // GET: /Customer/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();
            var customer = await _customerService.GetByIdAsync(id, userId);
            if (customer == null) return NotFound();

            var orders = await _customerService.GetOrdersForCustomerAsync(id, userId);
            ViewBag.CustomerOrders = orders;
            return View(customer);
        }

        // GET: /Customer/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerFormViewModel());
        }

        // POST: /Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CustomerCreateDto
            {
                CustomerName = vm.CustomerName,
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address
            };
            await _customerService.CreateAsync(dto, GetUserId());
            TempData["SuccessMessage"] = "Customer added successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Customer/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetByIdAsync(id, GetUserId());
            if (customer == null) return NotFound();

            var vm = new CustomerFormViewModel
            {
                Id = id,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address
            };
            return View(vm);
        }

        // POST: /Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerFormViewModel vm)
        {
            vm.Id = id;

            if (!ModelState.IsValid)
                return View(vm);

            var dto = new CustomerUpdateDto
            {
                CustomerName = vm.CustomerName,
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address
            };
            var result = await _customerService.UpdateAsync(id, dto, GetUserId());
            if (result == null) return NotFound();

            TempData["SuccessMessage"] = "Customer updated successfully.";
            return RedirectToAction("Index");
        }

        // POST: /Customer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _customerService.DeleteAsync(id, GetUserId());
            TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Customer deleted successfully." : error;
            return RedirectToAction("Index");
        }
    }
}

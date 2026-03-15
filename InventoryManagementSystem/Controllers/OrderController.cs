using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InventoryAPI.DTOs;
using InventoryAPI.Services;
using InventoryAPI.ViewModels;

namespace InventoryAPI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;

        public OrderController(IOrderService orderService, IProductService productService, ICustomerService customerService)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
        }

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Order
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetAllAsync(userId);
            var allProducts = await _productService.GetAllAsync(userId);
            var allCustomers = await _customerService.GetAllAsync(userId);

            var vm = new OrderPageViewModel
            {
                Orders = orders,
                AvailableProducts = allProducts.Where(p => p.Quantity > 0).ToList(),
                AvailableCustomers = allCustomers,
                SuccessMessage = TempData["SuccessMessage"]?.ToString(),
                ErrorMessage = TempData["ErrorMessage"]?.ToString()
            };
            return View(vm);
        }

        // POST: /Order/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(int ProductId, int? CustomerId, int Quantity)
        {
            var dto = new OrderCreateDto { ProductId = ProductId, CustomerId = CustomerId, Quantity = Quantity };
            var (order, error) = await _orderService.CreateAsync(dto, GetUserId());

            if (error != null)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = $"Order placed! {order!.Quantity}x {order.ProductName} sold for ₹{order.TotalRevenue:F2}";

            return RedirectToAction("Index");
        }

        // POST: /Order/CancelOrder/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var (success, error) = await _orderService.DeleteAsync(id, GetUserId());

            if (success)
                TempData["SuccessMessage"] = "Order cancelled. Stock has been restored.";
            else
                TempData["ErrorMessage"] = error ?? "Order not found.";

            return RedirectToAction("Index");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerReadDto>> GetAllAsync(int userId)
        {
            return await _context.Customers
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CustomerReadDto
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CreatedAt = c.CreatedAt,
                    TotalOrders = c.Orders.Count,
                    TotalSpent = c.Orders.Sum(o => (decimal?)o.Quantity * o.UnitSellingPrice) ?? 0
                })
                .ToListAsync();
        }

        public async Task<CustomerReadDto?> GetByIdAsync(int id, int userId)
        {
            return await _context.Customers
                .Where(c => c.Id == id && c.UserId == userId)
                .Select(c => new CustomerReadDto
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CreatedAt = c.CreatedAt,
                    TotalOrders = c.Orders.Count,
                    TotalSpent = c.Orders.Sum(o => (decimal?)o.Quantity * o.UnitSellingPrice) ?? 0
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerReadDto> CreateAsync(CustomerCreateDto dto, int userId)
        {
            var customer = new Customer
            {
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerReadDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                CreatedAt = customer.CreatedAt,
                TotalOrders = 0,
                TotalSpent = 0
            };
        }

        public async Task<CustomerReadDto?> UpdateAsync(int id, CustomerUpdateDto dto, int userId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (customer == null) return null;

            customer.CustomerName = dto.CustomerName;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;

            await _context.SaveChangesAsync();
            return (await GetByIdAsync(id, userId))!;
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id, int userId)
        {
            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

            if (customer == null) return (false, "Customer not found.");

            if (customer.Orders.Any())
                return (false, $"Cannot delete '{customer.CustomerName}' — this customer has {customer.Orders.Count} order(s). Remove their orders first.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<CustomerReadDto>> SearchAsync(string query, int userId)
        {
            return await _context.Customers
                .Where(c => c.UserId == userId &&
                    (c.CustomerName.Contains(query) ||
                     (c.Phone != null && c.Phone.Contains(query)) ||
                     (c.Email != null && c.Email.Contains(query))))
                .OrderBy(c => c.CustomerName)
                .Select(c => new CustomerReadDto
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    CreatedAt = c.CreatedAt,
                    TotalOrders = c.Orders.Count,
                    TotalSpent = c.Orders.Sum(o => (decimal?)o.Quantity * o.UnitSellingPrice) ?? 0
                })
                .ToListAsync();
        }

        public async Task<List<OrderReadDto>> GetOrdersForCustomerAsync(int customerId, int userId)
        {
            return await _context.Orders
                .Include(o => o.Product)
                .Where(o => o.CustomerId == customerId && o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderReadDto
                {
                    Id = o.Id,
                    ProductId = o.ProductId,
                    ProductName = o.Product!.ProductName,
                    SKU = o.Product!.SKU,
                    Quantity = o.Quantity,
                    UnitSellingPrice = o.UnitSellingPrice,
                    UnitPurchasePrice = o.UnitPurchasePrice,
                    CreatedAt = o.CreatedAt,
                    CustomerName = o.Customer != null ? o.Customer.CustomerName : "—"
                })
                .ToListAsync();
        }
    }
}

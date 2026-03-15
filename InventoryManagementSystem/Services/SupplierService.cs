using Microsoft.EntityFrameworkCore;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;

        public SupplierService(AppDbContext context)
        {
            _context = context;
        }

        private static SupplierReadDto MapToDto(Supplier s) => new SupplierReadDto
        {
            Id = s.Id,
            SupplierName = s.SupplierName,
            CompanyName = s.CompanyName,
            Phone = s.Phone,
            Email = s.Email,
            Address = s.Address,
            CreatedAt = s.CreatedAt
        };

        public async Task<List<SupplierReadDto>> GetAllAsync(int userId)
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return suppliers.Select(MapToDto).ToList();
        }

        public async Task<SupplierReadDto?> GetByIdAsync(int id, int userId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (supplier == null) return null;
            return MapToDto(supplier);
        }

        public async Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto, int userId)
        {
            var supplier = new Supplier
            {
                SupplierName = dto.SupplierName,
                CompanyName = dto.CompanyName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task<SupplierReadDto?> UpdateAsync(int id, SupplierUpdateDto dto, int userId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (supplier == null) return null;

            supplier.SupplierName = dto.SupplierName;
            supplier.CompanyName = dto.CompanyName;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;

            await _context.SaveChangesAsync();

            return MapToDto(supplier);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (supplier == null) return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SupplierReadDto>> SearchAsync(string query, int userId)
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.UserId == userId &&
                    (s.SupplierName.Contains(query) ||
                     (s.CompanyName != null && s.CompanyName.Contains(query)) ||
                     (s.Email != null && s.Email.Contains(query))))
                .OrderBy(s => s.SupplierName)
                .ToListAsync();

            return suppliers.Select(MapToDto).ToList();
        }
    }
}

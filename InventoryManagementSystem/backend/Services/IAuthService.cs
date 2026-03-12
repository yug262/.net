using InventoryAPI.DTOs;

namespace InventoryAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterDto registerDto);
    }
}

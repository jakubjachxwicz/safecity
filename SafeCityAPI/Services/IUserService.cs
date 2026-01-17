using System.Security.Claims;
using SafeCityAPI.DTOs;

namespace SafeCityAPI.Services;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request);
    Task<AuthResponse> LoginAsync(LoginUserRequest request);
    Task<UserResponse?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
}

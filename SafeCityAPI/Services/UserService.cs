using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SafeCityAPI.DTOs;
using SafeCityAPI.Helpers;
using SafeCityAPI.Data;
using SafeCityAPI.Models;

namespace SafeCityAPI.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<UserService> logger,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request)
    {
        _logger.LogInformation("Attempting to register user {Username}", request.Username);
        
        if (!ValidationHelper.ValidateNickname(request.Username, out string nicknameError))
        {
            _logger.LogWarning("Registration failed - invalid nickname: {Username}, Error: {Error}", 
                request.Username, nicknameError);
            throw new ArgumentException(nicknameError);
        }
        
        if (!ValidationHelper.ValidateEmail(request.Email, out string emailError))
        {
            _logger.LogWarning("Registration failed - invalid email: {Email}, Error: {Error}", 
                request.Email, emailError);
            throw new ArgumentException(emailError);
        }
        
        if (!ValidationHelper.ValidatePassword(request.Password, out string passwordError))
        {
            _logger.LogWarning("Registration failed - weak password for user {Username}, Error: {Error}", 
                request.Username, passwordError);
            throw new ArgumentException(passwordError);
        }
        
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

        if (existingUser != null)
        {
            if (existingUser.Username == request.Username)
            {
                _logger.LogWarning("Registration failed - username already exists: {Username}", request.Username);
                throw new InvalidOperationException("USERNAME_EXISTS: Username already taken");
            }
            else
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
                throw new InvalidOperationException("EMAIL_EXISTS: Email already registered");
            }
        }
        
        var passwordHash = _passwordHasher.HashPassword(request.Password);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLower(),
            PasswordHash = passwordHash,
            Role = "user",
            IsBanned = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} registered successfully", user.Id);
        
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginUserRequest request)
    {
        _logger.LogInformation("Login attempt for user {Username}", request.Username);
        
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        
        if (user.IsBanned)
        {
            _logger.LogWarning("Login failed - user is banned: {UserId}", user.Id);
            throw new UnauthorizedAccessException("User account is banned");
        }
        
        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed - invalid password for user {Username}", request.Username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);
        
        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<UserResponse?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
    {
        // Wyciągnij ID użytkownika z JWT tokenu
        var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier) 
                         ?? userPrincipal.FindFirst(JwtRegisteredClaimNames.Sub);
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("Failed to extract user ID from JWT token");
            return null;
        }

        // Pobierz użytkownika z bazy danych
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            _logger.LogWarning("User not found in database: {UserId}", userId);
            return null;
        }

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsBanned = user.IsBanned
        };
    }
    
    // Token zawiera zakodowane dane uzytkownika. Uzywany do werifykacji kto wysyła requesty do API.
    private string GenerateJwtToken(User user)
    {
        // Pobierz secret key z konfiguracji
        var secretKey = _configuration["Jwt:SecretKey"] 
                        ?? throw new InvalidOperationException("JWT SecretKey not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims - dane w tokenie
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("IsBanned", user.IsBanned.ToString())
        };

        // Token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(31), // Token ważny 31 dni
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

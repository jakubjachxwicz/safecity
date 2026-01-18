namespace SafeCityMobile.User.Models;

public class UserAuthDto
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}

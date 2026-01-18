namespace SafeCityMobile.User.Models;

public class LoginRequestBodyDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}


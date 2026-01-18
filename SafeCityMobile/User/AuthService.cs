using SafeCityMobile.User.Models;

namespace SafeCityMobile.User;

public class AuthService
{
    public Guid? UserId { get; set; } = null;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public AuthService()
    {
        StartupLogin();
    }

    public async Task<bool> LoginAsync(UserAuthDto dto)
    {
        try
        {
            await SecureStorage.Default.SetAsync("access_token", dto.Token);

            UserId = dto.UserId;
            Username = dto.Username;
            Email = dto.Email;

            Preferences.Set("is_logged_in", true);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public void Logout()
    {
        Preferences.Set("is_logged_in", false);

        SecureStorage.Remove("access_token");
    }

    private void StartupLogin()
    {
        // check saved token and handle login

        Preferences.Set("is_logged_in", false);
    }

    public bool IsLoggedIn()
    {
        return Preferences.Get("is_logged_in", false);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await SecureStorage.GetAsync("access_token");
    }
}
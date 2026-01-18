using SafeCityMobile.Common;
using SafeCityMobile.User.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SafeCityMobile.User;

public class UserService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly AuthService _authService;
    private readonly AppState _appState;

    public UserService(HttpClient client,
        JsonSerializerOptions jsonSerializerOptions,
        AuthService authService,
        AppState appState)
    {
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _authService = authService;
        _appState = appState;
    }


    public async Task<LoginStatus> LoginUserAsync(LoginRequestBodyDto body)
    {
        var response = await _client.PostAsJsonAsync("api/users/login", body);
        var stringResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var dto = JsonSerializer.Deserialize<UserAuthDto>(stringResponse, _jsonSerializerOptions);

            var loginSuccessful = await _authService.LoginAsync(dto!);

            if (!loginSuccessful)
            {
                _appState.ErrorMessage = "Wystąpił nieznany błąd, nie udało się zalogować";
                return LoginStatus.Failure;
            }

            _appState.ClearMessages();
            return LoginStatus.Success;
        }

        _appState.ErrorMessage = "Niepoprawna nazwa użytkownika lub hasło";
        return LoginStatus.Failure;
    }

    public async Task<RegisterStatus> RegisterUserAsync(RegisterRequestBodyDto body)
    {
        var response = await _client.PostAsJsonAsync("api/users/register", body);
        var stringResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var dto = JsonSerializer.Deserialize<UserAuthDto>(stringResponse, _jsonSerializerOptions);

            var loginSuccessful = await _authService.LoginAsync(dto!);

            if (!loginSuccessful)
            {
                _appState.ErrorMessage = "Użytkownik zarejestrowany, problem z logowaniem";
                return RegisterStatus.Failure;
            }

            _appState.ClearMessages();
            return RegisterStatus.Success;
        }

        var errorDto = ApiResponse<UserAuthDto>.HandleFailedResponse(stringResponse, _jsonSerializerOptions);
        if (errorDto.Error!.Message.Contains("USERNAME_EXISTS"))
        {
            _appState.ErrorMessage = "Nazwa użytkownika jest już zajęta";
        }
        else if (errorDto.Error!.Message.Contains("EMAIL_EXISTS"))
        {
            _appState.ErrorMessage = "Adres e-mail jest już zajęty";
        }
        else if (errorDto.Error!.Message.Contains("WEAK_PASSWORD"))
        {
            _appState.ErrorMessage = "Hasło nie spełnia wymagań";
        }
        else
        {
            _appState.ErrorMessage = "Wystąpił nieznany błąd, nie udało się zarejestrować";
        }

        return RegisterStatus.Failure;
    }

    public void LogoutUser()
    {
        _authService.Logout();
    }
}


public enum LoginStatus
{
    Success,
    Failure
}

public enum RegisterStatus
{
    Success,
    Failure
}
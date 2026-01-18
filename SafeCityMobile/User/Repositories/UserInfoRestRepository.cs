using System.Net.Http.Headers;
using System.Text.Json;

namespace SafeCityMobile.User.Repositories;

public class UserInfoRestRepository : IUserInfoRepository
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly AuthService _authService;

    public UserInfoRestRepository(HttpClient client,
        JsonSerializerOptions jsonSerializerOptions,
        AuthService authService)
    {
        _authService = authService;
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task<UserInfo?> GetUserInfoAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/reports/my/count");

            var token = await _authService.GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var dto = JsonSerializer.Deserialize<DroneCount>(stringResponse, _jsonSerializerOptions);
                if (dto is not null)
                {
                    return new(_authService.Username, _authService.Email, dto.Count);
                }
                throw new("Failed to deserialize");
            }

            throw new("Failed to fetch drone count data");
        }
        catch (Exception)
        {
            return null;
        }
    }

    private record DroneCount(int Count);
}
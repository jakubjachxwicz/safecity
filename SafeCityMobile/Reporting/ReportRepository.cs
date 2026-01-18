using SafeCityMobile.Common;
using SafeCityMobile.User;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SafeCityMobile.Reporting;

public interface IReportRepository
{
    Task<ApiResponse<Report>> SaveReportAsync(ReportRequestDto report);
    Task<ApiResponse<List<Report>>> GetAllReportsAsync();
}

public class ReportRepository : IReportRepository
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly AuthService _authService;

    public ReportRepository(HttpClient client,
        JsonSerializerOptions jsonSerializerOptions,
        AuthService authService)
    {
        _client = client;
        _jsonSerializerOptions = jsonSerializerOptions;
        _authService = authService;
    }

    public async Task<ApiResponse<Report>> SaveReportAsync(ReportRequestDto report)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/reports");
        request.Content = JsonContent.Create(report);

        var token = await _authService.GetTokenAsync();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);
        var stringResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var dto = JsonSerializer.Deserialize<Report>(stringResponse, _jsonSerializerOptions);
            if (dto is not null)
            {
                return ApiResponse<Report>.Ok(dto);
            }
            return ApiResponse<Report>.Fail(new FailedResponse() { Message = "Failed to deserialize API response" });
        }

        return ApiResponse<Report>.Fail(new FailedResponse() { Message = "Failed to save report data" });
    }

    public async Task<ApiResponse<List<Report>>> GetAllReportsAsync()
    {
        var response = await _client.GetAsync("/api/reports/active");
        var stringResponse = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var dto = JsonSerializer.Deserialize<IEnumerable<Report>>(stringResponse, _jsonSerializerOptions)?.ToList();
            if (dto is not null)
            {
                return ApiResponse<List<Report>>.Ok(dto);
            }
            return ApiResponse<List<Report>>.Fail(new FailedResponse() { Message = "Failed to deserialize API response" });
        }

        return ApiResponse<List<Report>>.Fail(new FailedResponse() { Message = "Failed to retrieve reports data" });
    }
}
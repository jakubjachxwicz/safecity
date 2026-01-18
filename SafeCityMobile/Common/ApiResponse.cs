using System.Text.Json;

namespace SafeCityMobile.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public FailedResponse? Error { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(FailedResponse response) => new() { Success = false, Error = response };

    public static ApiResponse<T> HandleFailedResponse(string response, JsonSerializerOptions jsonSerializerOptions)
    {
        ApiError? error = null;
        try
        {
            error = JsonSerializer.Deserialize<ApiError>(response, jsonSerializerOptions);
        }
        catch { }

        return ApiResponse<T>.Fail(new FailedResponse
        {
            Message = error is not null ? error.Error : "Unexpected error occured"
        });
    }
}

public class ApiError
{
    public string Error { get; set; } = string.Empty;
}

public class FailedResponse
{
    public string Message { get; set; } = string.Empty;
}
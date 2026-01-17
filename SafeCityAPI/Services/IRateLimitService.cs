namespace SafeCityAPI.Services;

public interface IRateLimitService
{
    Task<(bool canReport, int secondsRemaining)> CanReportAsync(Guid? userId, string ipAddress);
}
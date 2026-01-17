using Microsoft.EntityFrameworkCore;
using SafeCityAPI.Data;

namespace SafeCityAPI.Services;

public class RateLimitService : IRateLimitService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RateLimitService> _logger;
    private const int RATE_LIMIT_SECONDS = 5;

    public RateLimitService(
        ApplicationDbContext context,
        ILogger<RateLimitService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool canReport, int secondsRemaining)> CanReportAsync(Guid? userId, string ipAddress)
    {
        DateTime? lastReportTime = null;
        string identifier;

        if (userId.HasValue)
        {
            identifier = $"User:{userId.Value}";
            
            lastReportTime = await _context.Reports
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.ReportedAt)
                .Select(r => r.ReportedAt)
                .FirstOrDefaultAsync();
        }
        else
        {
            identifier = $"IP:{ipAddress}";
            
            lastReportTime = await _context.Reports
                .Where(r => r.UserId == null && r.IpAddress == ipAddress)
                .OrderByDescending(r => r.ReportedAt)
                .Select(r => r.ReportedAt)
                .FirstOrDefaultAsync();
        }
        
        if (lastReportTime == null)
        {
            _logger.LogInformation("No previous reports for {Identifier}, allowing report", identifier);
            return (true, 0);
        }

        var timeSinceLastReport = DateTime.UtcNow - lastReportTime.Value;
        var secondsSinceLastReport = (int)timeSinceLastReport.TotalSeconds;

        if (secondsSinceLastReport >= RATE_LIMIT_SECONDS)
        {
            _logger.LogInformation("{Identifier} can report (last report was {Seconds}s ago)", 
                identifier, secondsSinceLastReport);
            return (true, 0);
        }

        var secondsRemaining = RATE_LIMIT_SECONDS - secondsSinceLastReport;
        
        _logger.LogWarning("{Identifier} is rate limited, {SecondsRemaining}s remaining", 
            identifier, secondsRemaining);
        
        return (false, secondsRemaining);
    }
}
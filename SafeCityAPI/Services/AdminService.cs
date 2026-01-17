using Microsoft.EntityFrameworkCore;
using SafeCityAPI.Data;
using SafeCityAPI.DTOs;

namespace SafeCityAPI.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        ApplicationDbContext context,
        ILogger<AdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> DeleteReportAsync(Guid reportId)
    {
        _logger.LogInformation("Admin deleting report {ReportId}", reportId);

        var report = await _context.Reports.FindAsync(reportId);
        
        if (report == null)
        {
            _logger.LogWarning("Report {ReportId} not found for deletion", reportId);
            return false;
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Report {ReportId} deleted successfully", reportId);
        return true;
    }

    public async Task<List<AdminUserResponse>> GetUsersAsync(int limit = 50, int offset = 0)
    {
        _logger.LogInformation("Admin fetching users: limit={Limit}, offset={Offset}", limit, offset);
        
        limit = Math.Min(limit, 100);

        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .Select(u => new
            {
                User = u,
                ReportsCount = _context.Reports.Count(r => r.UserId == u.Id)
            })
            .ToListAsync();

        _logger.LogInformation("Found {Count} users", users.Count);

        return users.Select(u => new AdminUserResponse
        {
            Id = u.User.Id,
            Username = u.User.Username,
            Email = u.User.Email,
            Role = u.User.Role,
            IsBanned = u.User.IsBanned,
            CreatedAt = u.User.CreatedAt,
            ReportsCount = u.ReportsCount
        }).ToList();
    }

    public async Task<AdminStatsResponse> GetStatsAsync()
    {
        _logger.LogInformation("Admin fetching statistics");

        var now = DateTime.UtcNow;
        var last24h = now.AddHours(-24);

        // Podstawowe statystyki
        var totalReports = await _context.Reports.CountAsync();
        var reportsLast24h = await _context.Reports
            .Where(r => r.ReportedAt >= last24h)
            .CountAsync();

        //falseReports i verifiedReports zawsze 0
        var falseReports = 0;
        var verifiedReports = 0;

        // Top 10 najbardziej aktywnych obszarów (pogrupowane po zaokrąglonych koordynatach)
        var topAreas = await _context.Reports
            .GroupBy(r => new
            {
                LatRounded = Math.Round(r.Latitude, 2),
                LonRounded = Math.Round(r.Longitude, 2)
            })
            .Select(g => new
            {
                Lat = g.Key.LatRounded,
                Lon = g.Key.LonRounded,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();
        
        var reportsByHour = await _context.Reports
            .GroupBy(r => r.ReportedAt.Hour)
            .Select(g => new HourlyStat
            {
                Hour = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Hour)
            .ToListAsync();
        
        var allHours = Enumerable.Range(0, 24)
            .Select(hour => new HourlyStat
            {
                Hour = hour,
                Count = reportsByHour.FirstOrDefault(x => x.Hour == hour)?.Count ?? 0
            })
            .ToList();

        _logger.LogInformation("Statistics calculated: {TotalReports} total, {Last24h} last 24h", 
            totalReports, reportsLast24h);

        return new AdminStatsResponse
        {
            TotalReports = totalReports,
            ReportsLast24h = reportsLast24h,
            FalseReports = falseReports,
            VerifiedReports = verifiedReports,
            TopAreas = topAreas.Select(a => new TopAreaStat
            {
                Name = $"Area {a.Lat:F2}, {a.Lon:F2}",
                Count = a.Count
            }).ToList(),
            ReportsByHour = allHours
        };
    }
}
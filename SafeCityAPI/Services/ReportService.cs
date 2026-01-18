using Microsoft.EntityFrameworkCore;
using SafeCityAPI.Data;
using SafeCityAPI.DTOs;
using SafeCityAPI.Models;

namespace SafeCityAPI.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        ApplicationDbContext context,
        ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReportResponse> CreateReportAsync(
        CreateReportRequest request, 
        string ipAddress, 
        Guid? userId = null)
    {
        _logger.LogInformation("Creating new report at {Lat}, {Lon}", 
            request.Latitude, request.Longitude);

        // Walidacja podstawowa
        if (request.Latitude is < -90 or > 90)
            throw new ArgumentException("Latitude must be between -90 and 90");

        if (request.Longitude is < -180 or > 180)
            throw new ArgumentException("Longitude must be between -180 and 180");
        
        var report = new Report
        {
            Id = Guid.NewGuid(),
            ReportedAt = DateTime.UtcNow,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Category = request.Category,
            Message = request.Description?.Trim(),
            IpAddress = ipAddress,
            UserId = userId
        };
        
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Report {Id} created successfully by user {UserId} from IP {IpAddress}", 
            report.Id, userId?.ToString() ?? "anonymous", ipAddress);
        
        return new ReportResponse
        {
            Id = report.Id,
            ReportedAt = report.ReportedAt,
            Latitude = report.Latitude,
            Longitude = report.Longitude,
            Category = report.Category,
            Message = report.Message,
            UserId = report.UserId,      
            IpAddress = report.IpAddress 
        };
    }

    public async Task<List<ReportResponse>> GetActiveReportsAsync()
    {
        _logger.LogInformation("Fetching active reports");

        var reports = await _context.Reports
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();

        _logger.LogInformation("Found {Count} active reports", reports.Count);
        
        return reports.Select(r => new ReportResponse
        {
            Id = r.Id,
            ReportedAt = r.ReportedAt,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Category = r.Category,
            Message = r.Message,
            UserId = r.UserId,      
            IpAddress = r.IpAddress 
        }).ToList();
    }

    public async Task<int> GetUserReportCountAsync(Guid userId)
    {
        _logger.LogInformation("Fetching report count for user {UserId}", userId);

        var count = await _context.Reports
            .Where(r => r.UserId == userId)
            .CountAsync();

        _logger.LogInformation("User {UserId} has {Count} reports", userId, count);
        
        return count;
    }

    public async Task<List<ReportResponse>> GetUserReportHistoryAsync(Guid userId)
    {
        _logger.LogInformation("Fetching report history for user {UserId}", userId);

        var reports = await _context.Reports
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();

        _logger.LogInformation("Found {Count} reports for user {UserId}", reports.Count, userId);
        
        return reports.Select(r => new ReportResponse
        {
            Id = r.Id,
            ReportedAt = r.ReportedAt,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Category = r.Category,
            Message = r.Message,
            UserId = r.UserId,
            IpAddress = r.IpAddress
        }).ToList();
    }

    public async Task<ReportResponse?> GetReportByIdAsync(Guid reportId)
    {
        _logger.LogInformation("Fetching report {ReportId}", reportId);

        var report = await _context.Reports.FindAsync(reportId);

        if (report == null)
        {
            _logger.LogWarning("Report {ReportId} not found", reportId);
            return null;
        }

        return new ReportResponse
        {
            Id = report.Id,
            ReportedAt = report.ReportedAt,
            Latitude = report.Latitude,
            Longitude = report.Longitude,
            Category = report.Category,
            Message = report.Message,
            UserId = report.UserId,
            IpAddress = report.IpAddress
        };
    }

    public async Task<List<ReportResponse>> SearchReportsAsync(
        DateTime? from = null, 
        DateTime? to = null, 
        double? minLat = null, 
        double? maxLat = null, 
        double? minLon = null, 
        double? maxLon = null)
    {
        _logger.LogInformation(
            "Searching reports: from={From}, to={To}, minLat={MinLat}, maxLat={MaxLat}, minLon={MinLon}, maxLon={MaxLon}",
            from, to, minLat, maxLat, minLon, maxLon);

        var query = _context.Reports.AsQueryable();

        // Filtr czasowy
        if (from.HasValue)
        {
            query = query.Where(r => r.ReportedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(r => r.ReportedAt <= to.Value);
        }

        // Filtr geograficzny
        if (minLat.HasValue)
        {
            query = query.Where(r => r.Latitude >= minLat.Value);
        }

        if (maxLat.HasValue)
        {
            query = query.Where(r => r.Latitude <= maxLat.Value);
        }

        if (minLon.HasValue)
        {
            query = query.Where(r => r.Longitude >= minLon.Value);
        }

        if (maxLon.HasValue)
        {
            query = query.Where(r => r.Longitude <= maxLon.Value);
        }

        var reports = await query
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();

        _logger.LogInformation("Search found {Count} reports", reports.Count);

        return reports.Select(r => new ReportResponse
        {
            Id = r.Id,
            ReportedAt = r.ReportedAt,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Category = r.Category,
            Message = r.Message,
            UserId = r.UserId,
            IpAddress = r.IpAddress
        }).ToList();
    }
}
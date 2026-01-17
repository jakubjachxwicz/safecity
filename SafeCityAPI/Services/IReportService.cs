using SafeCityAPI.DTOs;

namespace SafeCityAPI.Services;

public interface IReportService
{
    Task<ReportResponse> CreateReportAsync(CreateReportRequest request, string ipAddress, Guid? userId = null);
    Task<List<ReportResponse>> GetActiveReportsAsync();
    Task<int> GetUserReportCountAsync(Guid userId);
    Task<List<ReportResponse>> GetUserReportHistoryAsync(Guid userId);
    Task<ReportResponse?> GetReportByIdAsync(Guid reportId);
    Task<List<ReportResponse>> SearchReportsAsync(
        DateTime? from = null, 
        DateTime? to = null, 
        double? minLat = null, 
        double? maxLat = null, 
        double? minLon = null, 
        double? maxLon = null);
}
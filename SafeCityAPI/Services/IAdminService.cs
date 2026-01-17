using SafeCityAPI.DTOs;

namespace SafeCityAPI.Services;

public interface IAdminService
{
    Task<bool> DeleteReportAsync(Guid reportId);
    Task<List<AdminUserResponse>> GetUsersAsync(int limit = 50, int offset = 0);
    Task<AdminStatsResponse> GetStatsAsync();
}
namespace SafeCityMobile.User.Repositories;

public interface IUserInfoRepository
{
    Task<UserInfo?> GetUserInfoAsync();
}

public record UserInfo(string Username, string Email, int TotalReportCount);
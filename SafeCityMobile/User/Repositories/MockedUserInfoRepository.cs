namespace SafeCityMobile.User.Repositories;

public class MockedUserInfoRepository : IUserInfoRepository
{
    public async Task<UserInfo?> GetUserInfoAsync()
    {
        return new("Krzysztof", "krzysztof.kowalski@gmail.com", 12);
    }
}

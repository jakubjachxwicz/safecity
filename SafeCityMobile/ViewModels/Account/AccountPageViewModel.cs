using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.User;
using SafeCityMobile.User.Repositories;
using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Map;
using System.ComponentModel;
using System.Diagnostics;

namespace SafeCityMobile.ViewModels.Account;

public class AccountPageViewModel : INotifyPropertyChanged
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly UserService _userService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IAsyncRelayCommand ReportHistoryCommand { get; }
    public IAsyncRelayCommand ChangePassworkCommand { get; }
    public IAsyncRelayCommand LogoutCommand { get; }

    public AccountPageViewModel(IUserInfoRepository userInfoRepository, UserService userService)
    {
        _userInfoRepository = userInfoRepository;
        _userService = userService;

        ReportHistoryCommand = new AsyncRelayCommand(ReportHistoryHandler);
        ChangePassworkCommand = new AsyncRelayCommand(ChangePasswordHandler);
        LogoutCommand = new AsyncRelayCommand(LogoutHandler);
    }

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged(nameof(Email));
        }
    }

    private int _totalReportCount = 0;
    public int TotalReportCount
    {
        get => _totalReportCount;
        set
        {
            _totalReportCount = value;
            OnPropertyChanged(nameof(TotalReportCount));
        }
    }

    private string _errorText = string.Empty;
    public string ErrorText
    {
        get => _errorText;
        set
        {
            _errorText = value;
            OnPropertyChanged(nameof(ErrorText));
        }
    }

    public async Task InitializeDataAsync()
    {
        var userInfo = await _userInfoRepository.GetUserInfoAsync();

        if (userInfo is null)
        {
            ErrorText = "Nie udało się pobrać danych użytkownika";
            return;
        }

        Username = userInfo.Username;
        Email = userInfo.Email;
        TotalReportCount = userInfo.TotalReportCount;
    }

    private async Task ReportHistoryHandler()
    {
        await Shell.Current.GoToAsync(nameof(ReportHistoryPage));
    }

    private async Task ChangePasswordHandler()
    {
        await Shell.Current.GoToAsync(nameof(ChangePasswordPage));
    }

    private async Task LogoutHandler()
    {
        _userService.LogoutUser();

        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Common;
using SafeCityMobile.User;
using SafeCityMobile.User.Models;
using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Account.ForgotPassword;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Account;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    public AppState AppState { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand RegisterCommand { get; }
    public IAsyncRelayCommand ForgotPasswordCommand { get; }

    private string _username = string.Empty;
    private string _password = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password != value)
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
    }

    public LoginViewModel(UserService userService, AppState appState)
    {
        _userService = userService;

        AppState = appState;
        AppState.ClearMessages();

        LoginCommand = new AsyncRelayCommand(Login);
        RegisterCommand = new AsyncRelayCommand(Register);
        ForgotPasswordCommand = new AsyncRelayCommand(ForgotPasswordHandler);
    }

    private async Task Login()
    {
        AppState.ClearMessages();

        var loginRequest = new LoginRequestBodyDto()
        {
            Username = Username,
            Password = Password
        };

        var loginResult = await _userService.LoginUserAsync(loginRequest);

        if (loginResult == LoginStatus.Success)
        {
            await Shell.Current.GoToAsync(nameof(AccountPage));
        }
    }

    private async Task Register()
    {
        await Shell.Current.GoToAsync(nameof(RegisterPage));
    }

    private async Task ForgotPasswordHandler()
    {
        await Shell.Current.GoToAsync(nameof(SendCodePage));
    }

    void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Common;
using SafeCityMobile.User;
using SafeCityMobile.User.Models;
using SafeCityMobile.Views.Account;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Account;

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly UserService _userSevice;
    private readonly Validator _validator;
    public AppState AppState { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _username;
    private string _email;
    private string _password;
    private string _repeatedPassword;

    public IAsyncRelayCommand RegisterCommand { get; }
    public IAsyncRelayCommand GoToLoginPageCommand { get; }

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

    public string Email
    {
        get => _email;
        set
        {
            if (_email != value)
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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
    public string RepeatedPassword
    {
        get => _repeatedPassword;
        set
        {
            if (_repeatedPassword != value)
            {
                _repeatedPassword = value;
                OnPropertyChanged(nameof(RepeatedPassword));
            }
        }
    }

    public RegisterViewModel(UserService userService, AppState appState, Validator validator)
    {
        _userSevice = userService;
        _validator = validator;

        AppState = appState;
        AppState.ClearMessages();

        RegisterCommand = new AsyncRelayCommand(Register);
        GoToLoginPageCommand = new AsyncRelayCommand(GoToLoginPage);
    }

    private async Task Register()
    {
        AppState.ClearMessages();

        if (!_validator.ValidateEmail(Email))
        {
            AppState.ErrorMessage = "Niepoprawny adres e-mail";
            return;
        }

        if (!_validator.ValidatePassword(Password))
        {
            AppState.ErrorMessage = "Hasło musi mieć min. 10 znaków oraz zawierać małą literę, dużą literę, cyfrę, znak specjalny";
            return;
        }

        if (Password != RepeatedPassword)
        {
            AppState.ErrorMessage = "Hasła różnią się od siebie";
            return;
        }

        var registerBody = new RegisterRequestBodyDto
        {
            Username = Username,
            Email = Email,
            Password = Password
        };

        var registerStatus = await _userSevice.RegisterUserAsync(registerBody);

        if (registerStatus == RegisterStatus.Success)
        {
            await Shell.Current.GoToAsync(nameof(AccountPage));
        }
    }

    private async Task GoToLoginPage()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }

    void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

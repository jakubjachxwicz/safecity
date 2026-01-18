using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Account.ForgotPassword;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Account;

public class ForgotPasswordViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public IAsyncRelayCommand SendCodeCommand { get; }
    public IAsyncRelayCommand VerifyCodeCommand { get; }
    public IAsyncRelayCommand ReturnCommand { get; }

    private string _email = string.Empty;
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

    private string _verificationCode = string.Empty;
    public string VerificationCode
    {
        get => _verificationCode;
        set
        {
            if (_verificationCode != value)
            {
                _verificationCode = value;
                OnPropertyChanged(nameof(VerificationCode));
            }
        }
    }

    public ForgotPasswordViewModel()
    {
        SendCodeCommand = new AsyncRelayCommand(SendCodeHandler);
        VerifyCodeCommand = new AsyncRelayCommand(VerifyCodeHandler);
        ReturnCommand = new AsyncRelayCommand(ReturnHandler);
    }

    private async Task SendCodeHandler()
    {
        await Shell.Current.GoToAsync(nameof(VerifyCodePage));
    }

    private async Task VerifyCodeHandler()
    {
        await Shell.Current.GoToAsync(nameof(NewPasswordPage));
    }

    private async Task ReturnHandler()
    {
        await Shell.Current.GoToAsync(nameof(LoginPage));
    }


    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

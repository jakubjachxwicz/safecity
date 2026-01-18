using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Views.Account;
using System.ComponentModel;
using System.Diagnostics;

namespace SafeCityMobile.ViewModels.Account;


public class ChangePasswordViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // true when user is setting new password from "Forgot password" workflow
    // otherwise password changing occurs with providing an old one
    public bool UserVerified { get; set; } = false;

    public IAsyncRelayCommand ResetPasswordCommand { get; }
    public IAsyncRelayCommand ReturnCommand { get; }

    public ChangePasswordViewModel()
    {
        ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordHandler);
        ReturnCommand = new AsyncRelayCommand(ReturnHandler);
    }

    private string _oldPassword = string.Empty;
    public string OldPassword
    {
        get => _oldPassword;
        set
        {
            if (_oldPassword != value)
            {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }
    }

    private string _newPassword = string.Empty;
    public string NewPassword
    {
        get => _newPassword;
        set
        {
            if (_newPassword != value)
            {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }
    }

    private string _confirmPassword = string.Empty;
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set
        {
            if (_confirmPassword != value)
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
    }


    private async Task ResetPasswordHandler()
    {
        Trace.WriteLine("ResetPasswordHandler");
    }

    private async Task ReturnHandler()
    {
        await Shell.Current.GoToAsync(nameof(AccountPage));
    }


    void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
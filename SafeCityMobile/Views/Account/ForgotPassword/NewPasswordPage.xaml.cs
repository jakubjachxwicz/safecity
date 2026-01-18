using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account.ForgotPassword;

public partial class NewPasswordPage : ContentPage
{
    public NewPasswordPage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();

        var vm = new ChangePasswordViewModel();
        BindingContext = vm;

        vm.UserVerified = true;

        var newPasswordEntry = new CustomEntry() { Placeholder = "Nowe has³o", IsPassword = true, BindingContext = vm };
        newPasswordEntry.SetBinding(CustomEntry.TextProperty, "NewPassword");

        var confirmPasswordEntry = new CustomEntry() { Placeholder = "PotwierdŸ has³o", IsPassword = true, BindingContext = vm };
        confirmPasswordEntry.SetBinding(CustomEntry.TextProperty, "ConfirmPassword");

        var resetPasswordButton = new Button()
        {
            Text = "Zresetuj has³o",
            Style = (Style)Application.Current.Resources["LightButtonStyle"],
            BindingContext = vm
        };
        resetPasswordButton.SetBinding(Button.CommandProperty, "ResetPasswordCommand");

        var returnButton = new Button()
        {
            Text = "Powrót",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = vm
        };
        returnButton.SetBinding(Button.CommandProperty, "ReturnCommand");

        SharedControl.InnerContent = new StackLayout
        {
            Children = { newPasswordEntry, confirmPasswordEntry, resetPasswordButton, returnButton }
        };
    }
}
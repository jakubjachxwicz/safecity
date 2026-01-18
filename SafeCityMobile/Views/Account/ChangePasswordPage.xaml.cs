using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage()
    {
        InitializeComponent();

        var vm = new ChangePasswordViewModel();
        BindingContext = vm;

        var oldPasswordEntry = new CustomEntry() { Placeholder = "Stare has³o", IsPassword = true, BindingContext = vm };
        oldPasswordEntry.SetBinding(CustomEntry.TextProperty, "OldPassword");

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
            Children = { oldPasswordEntry, newPasswordEntry, confirmPasswordEntry, resetPasswordButton, returnButton }
        };
    }
}
using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

        var usernameEntry = new CustomEntry() { Placeholder = "Nazwa u¿ytkownika", BindingContext = viewModel };
        usernameEntry.SetBinding(CustomEntry.TextProperty, "Username");

        var emailEntry = new CustomEntry() { Placeholder = "E-mail", BindingContext = viewModel };
        emailEntry.SetBinding(CustomEntry.TextProperty, "Email");

        var passwordEntry = new CustomEntry() { Placeholder = "Has³o", IsPassword = true, BindingContext = viewModel };
        passwordEntry.SetBinding(CustomEntry.TextProperty, "Password");

        var repeatedPasswordEntry = new CustomEntry() { Placeholder = "Powtórz has³o", IsPassword = true, BindingContext = viewModel };
        repeatedPasswordEntry.SetBinding(CustomEntry.TextProperty, "RepeatedPassword");

        var splitter = new BoxView() { HeightRequest = 48, Background = Brush.Transparent };

        var registerButton = new Button()
        {
            Text = "Zarejestruj siê",
            Style = (Style)Application.Current.Resources["LightButtonStyle"],
            BindingContext = viewModel
        };
        registerButton.SetBinding(Button.CommandProperty, "RegisterCommand");

        var goToLoginButton = new Button()
        {
            Text = "Wróæ do logowania",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = viewModel
        };
        goToLoginButton.SetBinding(Button.CommandProperty, "GoToLoginPageCommand");

        var infoLabel = new InfoLabel() { BindingContext = viewModel };
        infoLabel.SetBinding(InfoLabel.TextProperty, "AppState.ErrorMessage");

        SharedControl.InnerContent = new VerticalStackLayout()
        {
            Children = { usernameEntry, emailEntry, passwordEntry, repeatedPasswordEntry, splitter, registerButton, goToLoginButton, infoLabel }
        };
    }
}
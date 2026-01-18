using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
        InitializeComponent();

        BindingContext = viewModel;

        var usernameEntry = new CustomEntry() { Placeholder = "Nazwa użytkownika", BindingContext = viewModel };
        usernameEntry.SetBinding(CustomEntry.TextProperty, "Username");

        var passwordEntry = new CustomEntry() { Placeholder = "Hasło", IsPassword = true, BindingContext = viewModel };
        passwordEntry.SetBinding(CustomEntry.TextProperty, "Password");

        var loginButton = new Button()
        {
            Text = "Zaloguj się",
            Style = (Style)Application.Current.Resources["LightButtonStyle"],
            BindingContext = viewModel
        };
        loginButton.SetBinding(Button.CommandProperty, "LoginCommand");

        var registerButton = new Button()
        {
            Text = "Zarejestruj się",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = viewModel
        };
        registerButton.SetBinding(Button.CommandProperty, "RegisterCommand");

        var splitter = new BoxView() { HeightRequest = 48, Background = Brush.Transparent };

        var forgotPasswordButton = new Button()
        {
            Text = "Nie pamiętam hasła",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = viewModel
        };
        forgotPasswordButton.SetBinding(Button.CommandProperty, "ForgotPasswordCommand");

        var infoLabel = new InfoLabel() { BindingContext = viewModel };
        infoLabel.SetBinding(InfoLabel.TextProperty, "AppState.ErrorMessage");

        SharedControl.InnerContent = new StackLayout
        {
            Children = { usernameEntry, passwordEntry, loginButton, registerButton, splitter, forgotPasswordButton, infoLabel }
        };
    }
}
using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account.ForgotPassword;

public partial class SendCodePage : ContentPage
{
    private readonly ForgotPasswordViewModel _viewModel;

    public SendCodePage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        var emailEntry = new CustomEntry() { Placeholder = "Email", BindingContext = _viewModel };
        emailEntry.SetBinding(CustomEntry.TextProperty, "Email");

        var sendCodeButton = new Button()
        {
            Text = "Wyślij kod",
            Style = (Style)Application.Current.Resources["LightButtonStyle"],
            BindingContext = _viewModel
        };
        sendCodeButton.SetBinding(Button.CommandProperty, "SendCodeCommand");

        var returnButton = new Button()
        {
            Text = "Powrót",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = _viewModel
        };
        returnButton.SetBinding(Button.CommandProperty, "ReturnCommand");

        SharedControl.InnerContent = new StackLayout
        {
            Children = { emailEntry, sendCodeButton, returnButton }
        };
    }
}
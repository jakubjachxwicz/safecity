using SafeCityMobile.ViewModels.Account;
using SafeCityMobile.Views.Controls;

namespace SafeCityMobile.Views.Account.ForgotPassword;

public partial class VerifyCodePage : ContentPage
{
    private readonly ForgotPasswordViewModel _viewModel;

    public VerifyCodePage(ForgotPasswordViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;

        var emailEntry = new CustomEntry() { Placeholder = "Kod z email", BindingContext = _viewModel };
        emailEntry.SetBinding(CustomEntry.TextProperty, "VerificationCode");

        var verifyCodeButton = new Button()
        {
            Text = "Zweryfikuj kod",
            Style = (Style)Application.Current.Resources["LightButtonStyle"],
            BindingContext = _viewModel
        };
        verifyCodeButton.SetBinding(Button.CommandProperty, "VerifyCodeCommand");

        var returnButton = new Button()
        {
            Text = "Powrót",
            Style = (Style)Application.Current.Resources["DarkButtonStyle"],
            BindingContext = _viewModel
        };
        returnButton.SetBinding(Button.CommandProperty, "ReturnCommand");

        SharedControl.InnerContent = new StackLayout
        {
            Children = { emailEntry, verifyCodeButton, returnButton }
        };
    }
}
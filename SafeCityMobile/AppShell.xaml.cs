using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Account.ForgotPassword;
using SafeCityMobile.Views.Map;

namespace SafeCityMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
            Routing.RegisterRoute(nameof(AccountPage), typeof(AccountPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(NewPasswordPage), typeof(NewPasswordPage));
            Routing.RegisterRoute(nameof(VerifyCodePage), typeof(VerifyCodePage));
            Routing.RegisterRoute(nameof(SendCodePage), typeof(SendCodePage));
        }
    }
}

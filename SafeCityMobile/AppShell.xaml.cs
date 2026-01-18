using SafeCityMobile.Views.Account;
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
        }
    }
}

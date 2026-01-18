using SafeCityMobile.User;
using SafeCityMobile.ViewModels.Account;

namespace SafeCityMobile.Views.Account;

public partial class AccountPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly AccountPageViewModel _viewModel;

    public AccountPage(AuthService authService, AccountPageViewModel viewModel)
    {
        InitializeComponent();

        _authService = authService;

        if (!_authService.IsLoggedIn())
        {
            //Shell.Current.GoToAsync(nameof(LoginPage));
        }

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitializeDataAsync();
    }
}
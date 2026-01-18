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

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_authService.IsLoggedIn())
        {
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }

        await _viewModel.InitializeDataAsync();
    }
}
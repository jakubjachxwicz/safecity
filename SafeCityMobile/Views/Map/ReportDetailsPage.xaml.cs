using SafeCityMobile.ViewModels.Map;

namespace SafeCityMobile.Views.Map;

public partial class ReportDetailsPage : ContentPage
{
    public ReportDetailsPage(ReportDetailsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ReportDetailsViewModel viewModel)
        {
            await viewModel.InitDataAsync();
        }
    }
}
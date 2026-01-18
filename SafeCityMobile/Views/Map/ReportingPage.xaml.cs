using SafeCityMobile.ViewModels.Map;

namespace SafeCityMobile.Views.Map;

public partial class ReportingPage : ContentPage
{
	public ReportingPage(ReportingViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}
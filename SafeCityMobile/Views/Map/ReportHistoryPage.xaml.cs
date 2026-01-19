using SafeCityMobile.ViewModels.Map;

namespace SafeCityMobile.Views.Map;

public partial class ReportHistoryPage : ContentPage
{
	public ReportHistoryPage(ReportHistoryViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}
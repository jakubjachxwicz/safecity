using SafeCityMobile.ViewModels.Map;

namespace SafeCityMobile.Views.Map;

public partial class EditReportPage : ContentPage
{
	public EditReportPage(EditReportViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}
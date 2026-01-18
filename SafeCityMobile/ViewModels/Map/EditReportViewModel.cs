using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Reporting;
using SafeCityMobile.Views.Map;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Map;

public class EditReportViewModel : IQueryAttributable, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IReportRepository _reportRepository;

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }

    private Guid _reportId;

    private string _comment = string.Empty;
    public string Comment
    {
        get => _comment;
        set
        {
            _comment = value;
            OnPropertyChanged(nameof(Comment));
        }
    }

    public List<MappedReportCategory> ReportCategories { get; init; }
        = MappedReportCategory.GetReportCategories();
    private MappedReportCategory? _selectedCategory;
    public MappedReportCategory? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));
        }
    }

    private string _infoText = string.Empty;
    public string InfoText
    {
        get => _infoText;
        set
        {
            _infoText = value;
            OnPropertyChanged(nameof(InfoText));
        }
    }

    public EditReportViewModel(IReportRepository reportRepository)
    {
        SaveCommand = new AsyncRelayCommand(SaveCommandHandler);
        CancelCommand = new AsyncRelayCommand(CancelCommandHandler);

        _reportRepository = reportRepository;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _reportId = (Guid)query["report_id"];
        Comment = (string)query["comment"];
        SelectedCategory = ReportCategories.Single(rc => rc.Category == (ReportCategory)query["category"]);
    }

    private async Task SaveCommandHandler()
    {
        try
        {
            var dto = new ReportUpdateRequestDto()
            {
                Description = Comment,
                Category = SelectedCategory?.Category
            };
            var result = await _reportRepository.UpdateReportAsync(_reportId, dto);

            if (!result.Success)
                throw new();

            var query = new Dictionary<string, object>() { { "id", _reportId } };

            await Shell.Current.GoToAsync(nameof(ReportDetailsPage), query);
        } catch (Exception ex)
        {
            InfoText = "Nie udało się zapisać zmian";
        }
    }

    private async Task CancelCommandHandler()
    {
        var query = new Dictionary<string, object>() { { "id", _reportId } };

        await Shell.Current.GoToAsync(nameof(ReportDetailsPage), query);
    }


    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

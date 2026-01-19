using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Reporting;
using SafeCityMobile.Views.Account;
using SafeCityMobile.Views.Map;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Map;

public class ReportHistoryViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private event Action? PageLoaded;
    public ObservableCollection<FormattedReport> Reports { get; } = new ObservableCollection<FormattedReport>();

    private readonly IReportRepository _reportRepository;

    public IAsyncRelayCommand ReturnCommand { get; }
    public IAsyncRelayCommand<FormattedReport?> ReportSelectedCommand { get; }

    private FormattedReport? _selectedReport;
    public FormattedReport? SelectedReport
    {
        get => _selectedReport;
        set
        {
            _selectedReport = value;
            OnPropertyChanged(nameof(SelectedReport));
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

    public ReportHistoryViewModel(IReportRepository reportRepository)
    {
        PageLoaded += async () => await InitList();

        _reportRepository = reportRepository;
        
        ReturnCommand = new AsyncRelayCommand(ReturnCommandHandler);
        ReportSelectedCommand = new AsyncRelayCommand<FormattedReport?>(ReportSelected);

        PageLoaded?.Invoke();
    }

    private async Task ReturnCommandHandler()
    {
        await Shell.Current.GoToAsync(nameof(AccountPage));
    }

    private async Task InitList()
    {
        var result = await _reportRepository.GetReportsForUser();
        if (!result.Success || result.Data is null)
        {
            InfoText = "Nie udało się pobrać danych";
            return;
        }

        Reports.Clear();
        foreach (var report in result.Data)
        {
            var formatted = new FormattedReport(report);
            Reports.Add(formatted);
        }
    }

    private async Task ReportSelected(FormattedReport? report)
    {
        if (report is null)
            return;
        
        var query = new Dictionary<string, object>() { { "id", report.Id } };

        await Shell.Current.GoToAsync(nameof(ReportDetailsPage), query);
    }

    
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

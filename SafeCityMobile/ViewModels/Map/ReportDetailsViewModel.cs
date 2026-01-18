using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Reporting;
using SafeCityMobile.Views.Map;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Map;

public class ReportDetailsViewModel : INotifyPropertyChanged, IQueryAttributable
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private event Action? ReportIdRetrieved;

    private readonly IReportRepository _reportRepository;

    private Guid _reportId;
    private bool _idRetrieved = false;

    public IAsyncRelayCommand ReportCommand { get; }
    public IAsyncRelayCommand ReturnCommand { get; }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _reportId = (Guid)query["id"];
        _idRetrieved = true;
        ReportIdRetrieved?.Invoke();
    }

    private double _latitude;
    public double Latitude
    {
        get => _latitude;
        set
        {
            if (_latitude != value)
            {
                _latitude = value;
                OnPropertyChanged(nameof(FormattedCoordinates));
            }
        }
    }
    private double _longitude;
    public double Longitude
    {
        get => _longitude;
        set
        {
            if (_longitude != value)
            {
                _longitude = value;
                OnPropertyChanged(nameof(FormattedCoordinates));
            }
        }
    }

    public string FormattedCoordinates
    {
        get => $"{Latitude:F4}° N, {Longitude:F4}° E";
    }

    private DateTime _timestamp;
    public DateTime Timestamp
    {
        get => _timestamp;
        set
        {
            _timestamp = value;
            OnPropertyChanged(nameof(FormattedTimestamp));
        }
    }
    public string FormattedTimestamp
    {
        get => Timestamp.ToString("yyyy-MM-dd HH:mm");
    }

    private string _reportComment = string.Empty;
    public string ReportComment
    {
        get => _reportComment;
        set
        {
            if (_reportComment != value)
            {
                _reportComment = value;
                OnPropertyChanged(nameof(ReportComment));
            }
        }
    }

    private ReportCategory _category;
    public ReportCategory Category
    {
        get => _category;
        set
        {
            _category = value;
            OnPropertyChanged(nameof(MappedCategory));
        }
    }

    public string MappedCategory
    {
        get => MappedReportCategory.GetReportCategories().Single(rc => rc.Category == Category).Text;
    }


    private string _infoLabel = string.Empty;
    public string InfoLabel
    {
        get => _infoLabel;
        set
        {
            _infoLabel = value;
            OnPropertyChanged(nameof(InfoLabel));
        }
    }


    public ReportDetailsViewModel(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
        
        ReportCommand = new AsyncRelayCommand(ReportHandler);
        ReturnCommand = new AsyncRelayCommand(ReturnHandler);

        ReportIdRetrieved += async () => await InitDataAsync();
    }

    public async Task InitDataAsync()
    {
        if (_idRetrieved)
        {
            var response = await _reportRepository.GetReportByIdAsync(_reportId);
            if (response is null || !response.Success)
            {
                InfoLabel = "Nie udało się pobrać danych o zgłoszeniu";
                return;
            }

            var report = response.Data!;

            Latitude = report.Latitude;
            Longitude = report.Longitude;
            ReportComment = string.IsNullOrEmpty(report.Message) ? "brak" : report.Message;
            Timestamp = report.ReportedAt;
            Category = report.Category;
        }
    }

    private async Task ReportHandler()
    {
        // TODO
    }

    private async Task ReturnHandler()
    {
        await Shell.Current.GoToAsync(nameof(MapPage));
    }


    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}


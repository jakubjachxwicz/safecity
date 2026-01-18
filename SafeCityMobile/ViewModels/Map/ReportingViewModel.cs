using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Reporting;
using SafeCityMobile.Views.Map;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Map;

public class ReportingViewModel : IQueryAttributable, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly IReportRepository _reportRepository;

    private static DateTime? _nextAllowedReportTimestamp = null;

    private double _latitude = 0.0;
    private double _longitude = 0.0;
    private DateTime _timestamp;

    public List<MappedReportCategory> ReportCategories { get; init; }
        = MappedReportCategory.GetReportCategories();
    public MappedReportCategory? SelectedCategory { get; set; }

    public IAsyncRelayCommand ConfirmReportCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }

    public double Latitude
    {
        get => _latitude;
        set
        {
            if (_latitude != value)
            {
                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
                OnPropertyChanged(nameof(FormattedCoordinates));
            }
        }
    }
    public double Longitude
    {
        get => _longitude;
        set
        {
            if (_longitude != value)
            {
                _longitude = value;
                OnPropertyChanged(nameof(Longitude));
                OnPropertyChanged(nameof(FormattedCoordinates));
            }
        }
    }

    public string FormattedCoordinates
    {
        get => $"{Latitude:F4}° N, {Longitude:F4}° E";
    }

    public string FormattedTimestamp
    {
        get => _timestamp.ToString("yyyy-MM-dd HH:mm");
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

    private string _infoText = string.Empty;
    public string InfoText
    {
        get => _infoText;
        set
        {
            if (value != _infoText)
            {
                _infoText = value;
                OnPropertyChanged(nameof(InfoText));
            }
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Latitude = (double)query["latitude"];
        Longitude = (double)query["longitude"];
    }

    public ReportingViewModel(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;

        ConfirmReportCommand = new AsyncRelayCommand(ConfirmReportHandler);
        CancelCommand = new AsyncRelayCommand(CancelHandler);

        if (_nextAllowedReportTimestamp is null)
        {
            _nextAllowedReportTimestamp = DateTime.Now;
        }

        _timestamp = DateTime.Now;
        OnPropertyChanged(nameof(FormattedTimestamp));
    }

    private async Task ConfirmReportHandler()
    {
        var now = DateTime.Now;
        if (now < _nextAllowedReportTimestamp)
        {
            InfoText = "Zbyt częste próby zgłoszeń, spróbuj ponownie później";
            return;
        }

        var report = new ReportRequestDto()
        {
            Latitude = Latitude,
            Longitude = Longitude,
            Description = ReportComment,
            Category = SelectedCategory?.Category
        };

        var response = await _reportRepository.SaveReportAsync(report);

        if (response.Success)
        {
            InfoText = "Zgłoszenie wysłano pomyślnie";
            _nextAllowedReportTimestamp = DateTime.Now.AddSeconds(5);
        }
        else
        {
            InfoText = "Nie udało się wysłać zgłoszenia";
        }
    }

    private async Task CancelHandler()
    {
        await Shell.Current.GoToAsync(nameof(MapPage));
    }


    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

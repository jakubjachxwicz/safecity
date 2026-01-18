using CommunityToolkit.Mvvm.Input;
using SafeCityMobile.Geolocating;
using SafeCityMobile.Reporting;
using SafeCityMobile.Views.Map;
using System.ComponentModel;

namespace SafeCityMobile.ViewModels.Map;

public class MapViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public IAsyncRelayCommand ReportCommand { get; }
    public IAsyncRelayCommand CenterOnLocationCommand { get; }
    public IAsyncRelayCommand RefreshMapCommand { get; }


    private WebView Map;
    private List<Report> _reports = [];
    private readonly MapHelpers _mapHelpers;
    private readonly CurrentGeolocationService _currentGeolocationService;
    private readonly IReportRepository _reportRepository;

    private bool _locationSet = false;

    private double _latitude = 50.07185;
    private double _longitude = 19.942238;

    public double Latitude
    {
        get => _latitude;
        set
        {
            if (_latitude != value)
            {
                _latitude = value;
                OnPropertyChanged(nameof(Latitude));
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

    public MapViewModel(MapHelpers mapHelpers,
        CurrentGeolocationService currentGeolocationService,
        IReportRepository reportRepository)
    {
        ReportCommand = new AsyncRelayCommand(ReportHandler);
        CenterOnLocationCommand = new AsyncRelayCommand(CenterOnLocationHandler);
        RefreshMapCommand = new AsyncRelayCommand(RefreshMapHandler);

        _mapHelpers = mapHelpers;
        _currentGeolocationService = currentGeolocationService;
        _reportRepository = reportRepository;
    }

    public async Task InitAsync(WebView webView)
    {
        Map = webView;
    }

    private async Task ReportHandler()
    {
        if (_locationSet)
        {
            var locationDict = new Dictionary<string, object>
            {
                { "latitude", Latitude },
                { "longitude", Longitude }
            };

            await Shell.Current.GoToAsync(nameof(ReportingPage), locationDict);
        }

        InfoText = "Nie ustawiono lokalizacji użytkownika";
    }

    public async Task SetBaseLocation()
    {
        var location = await _currentGeolocationService.GetCurrentLocationAsync();

        if (location is null)
        {
            InfoText = "Nie można ustalić lokalizacji użytkownika";
            return;
        }

        Latitude = location.Latitude;
        Longitude = location.Longitude;

        await PlaceUserMarker();
        await SetView();

        _locationSet = true;
    }

    public async Task RenderReportMarkers()
    {
        var response = await _reportRepository.GetAllReportsAsync();
        if (response is null || !response.Success)
        {
            _reports = [];
            return;
        }

        _reports = response.Data!;

        await Map.EvaluateJavaScriptAsync("clearAllMarkers()");

        foreach (var report in _reports)
        {
            var latStr = _mapHelpers.StringifyCoordinate(report.Latitude);
            var lonStr = _mapHelpers.StringifyCoordinate(report.Longitude);
            var id = report.Id.ToString();
            var category = report.Category;

            var jsInvocation = $"addMarker('{id}', {latStr}, {lonStr}, '{category}')";
            await Map.EvaluateJavaScriptAsync($"addMarker('{id}', {latStr}, {lonStr}, '{category}')");
        }
    }

    private async Task SetView()
    {
        var latStr = _mapHelpers.StringifyCoordinate(Latitude);
        var lonStr = _mapHelpers.StringifyCoordinate(Longitude);
        var zoom = 15;

        await Map.EvaluateJavaScriptAsync($"setViewWrapper({latStr}, {lonStr}, {zoom})");
    }

    private async Task PlaceUserMarker()
    {
        var latStr = _mapHelpers.StringifyCoordinate(Latitude);
        var lonStr = _mapHelpers.StringifyCoordinate(Longitude);

        await Map.EvaluateJavaScriptAsync($"setUserLocationMarker({latStr}, {lonStr})");
    }

    private async Task CenterOnLocationHandler()
    {
        await SetView();
    }

    private async Task RefreshMapHandler()
    {
        await SetBaseLocation();
        await RenderReportMarkers();
    }

    public async void MapNavigatingHandler(object? sender, WebNavigatingEventArgs e)
    {
        if (e.Url == "maui://mapReady")
        {
            e.Cancel = true;
            await RefreshMapHandler();

            return;
        }

        if (e.Url.StartsWith("maui://report/", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;

            var idStr = e.Url.Replace("maui://report/", "");
            if (Guid.TryParse(idStr, out var reportId))
            {
                var query = new Dictionary<string, object>() { { "id", reportId } };

                await Shell.Current.GoToAsync(nameof(ReportDetailsPage), query);
            }
        }
    }


    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
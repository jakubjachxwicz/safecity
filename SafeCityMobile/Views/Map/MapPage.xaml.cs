using SafeCityMobile.ViewModels.Map;

namespace SafeCityMobile.Views.Map;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel mapViewModel)
    {
        InitializeComponent();

        _viewModel = mapViewModel;
        BindingContext = _viewModel;

        // should be refactored to be more elegant
        MapView.Navigated += async (s, e) =>
        {
            await MapView.EvaluateJavaScriptAsync(@"
                window.mauiInvokeMapReady = function() {
                    window.location.href = 'maui://mapReady';
                };
            ");

            await MapView.EvaluateJavaScriptAsync("window.mauiInvokeMapReady();");
        };

        MapView.Navigating += _viewModel.MapNavigatingHandler;

        var htmlSource = Path.Combine(FileSystem.AppDataDirectory, "map.html");

        if (!File.Exists(htmlSource))
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("map.html").Result;
            using var reader = new StreamReader(stream);
            File.WriteAllText(htmlSource, reader.ReadToEnd());
        }

        MapView.Source = new UrlWebViewSource
        {
            //Url = htmlSource

            // remove after all fixes
            Url = $"map.html?ts={DateTime.Now.Ticks}"
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.InitAsync(MapView);
    }

    private async Task OnMapReady()
    {
        await _viewModel.SetBaseLocation();
        await _viewModel.RenderReportMarkers();
    }
}
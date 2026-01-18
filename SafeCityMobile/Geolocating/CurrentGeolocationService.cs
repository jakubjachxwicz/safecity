namespace SafeCityMobile.Geolocating;

public class CurrentGeolocationService
{
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isCheckingLocation;

    public async Task<Location?> GetCurrentLocationAsync()
    {
        _isCheckingLocation = true;

        var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(15));

        _cancellationTokenSource = new CancellationTokenSource();

        return await Geolocation.Default.GetLocationAsync(geolocationRequest, _cancellationTokenSource.Token);
    }
}

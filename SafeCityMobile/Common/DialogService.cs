namespace SafeCityMobile.Common;

public class DialogService
{
    public async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        return await Application.Current!.MainPage!.DisplayAlert(title, message, accept, cancel);
    }
}
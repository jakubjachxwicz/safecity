using System.ComponentModel;

namespace SafeCityMobile.Common;


public class AppState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
    }

    public void ClearMessages()
    {
        ErrorMessage = string.Empty;
    }


    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

namespace SafeCityMobile.Views.Controls;

public partial class InfoLabel : ContentView
{
    public InfoLabel()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(CustomEntry),
            default(string),
            BindingMode.TwoWay);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
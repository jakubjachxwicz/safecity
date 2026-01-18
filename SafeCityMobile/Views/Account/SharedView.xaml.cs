namespace SafeCityMobile.Views.Account;


public partial class SharedView : ContentView
{
    public SharedView()
    {
        InitializeComponent();

        BindingContext = this;
    }

    public string PageTitle
    {
        get => (string)GetValue(PageTitleProperty);
        set => SetValue(PageTitleProperty, value);
    }

    public static readonly BindableProperty PageTitleProperty =
        BindableProperty.Create(nameof(PageTitle), typeof(string), typeof(SharedView), default(string));

    public View InnerContent
    {
        get => DynamicContent.Content;
        set => DynamicContent.Content = value;
    }
}
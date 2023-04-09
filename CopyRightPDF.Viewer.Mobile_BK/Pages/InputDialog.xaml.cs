using System.Windows.Input;

namespace CopyRightPDF.Viewer.Mobile.Pages;

public partial class InputDialog : ContentPage
{
    public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(InputDialog), string.Empty);

    public static readonly BindableProperty InputTextProperty =
        BindableProperty.Create(nameof(InputText), typeof(string), typeof(InputDialog), string.Empty);

    public static readonly BindableProperty OkCommandProperty =
        BindableProperty.Create(nameof(OkCommand), typeof(ICommand), typeof(InputDialog));

    public static readonly BindableProperty CancelCommandProperty =
        BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(InputDialog));

    public InputDialog()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }

    public string InputText
    {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }

    public ICommand OkCommand
    {
        get { return (ICommand)GetValue(OkCommandProperty); }
        set { SetValue(OkCommandProperty, value); }
    }

    public ICommand CancelCommand
    {
        get { return (ICommand)GetValue(CancelCommandProperty); }
        set { SetValue(CancelCommandProperty, value); }
    }

    private TaskCompletionSource<string> _completionSource;

    public async Task<string> ShowAsync()
    {
        _completionSource = new TaskCompletionSource<string>();
        await App.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(this));
        return await _completionSource.Task;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        _completionSource.SetResult(InputText);
        App.Current.MainPage.Navigation.PopModalAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        _completionSource.SetResult(null);
        App.Current.MainPage.Navigation.PopModalAsync();
    }
}
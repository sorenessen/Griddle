using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Griddle.App.Views;

public partial class RecordingErrorDialog : Window
{
    public string TitleText { get; }

    public string MessageText { get; }

    public RecordingErrorDialog(
        string title,
        string message)
    {
        TitleText = title;
        MessageText = message;

        InitializeComponent();

        DataContext = this;
    }

    private void OkButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
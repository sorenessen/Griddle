using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Griddle.App.Views;

public partial class RecordingActiveDialog : Window
{
    public RecordingActiveDialog()
    {
        InitializeComponent();
    }

    private void OkButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close();
    }
}
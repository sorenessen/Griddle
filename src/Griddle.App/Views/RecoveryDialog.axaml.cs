using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Griddle.App.Views;

public partial class RecoveryDialog : Window
{
    public RecoveryDialog()
    {
        InitializeComponent();
    }

    private void DiscardButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            RecoveryChoice.Discard);
    }

    private void RestoreButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            RecoveryChoice.Restore);
    }
}
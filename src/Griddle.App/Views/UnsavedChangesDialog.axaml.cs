using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Griddle.App.Views;

public partial class UnsavedChangesDialog : Window
{
    public UnsavedChangesDialog()
    {
        InitializeComponent();
    }

    private void CancelButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            UnsavedChangesChoice.Cancel);
    }

    private void DontSaveButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            UnsavedChangesChoice.DontSave);
    }

    private void SaveButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            UnsavedChangesChoice.Save);
    }
}
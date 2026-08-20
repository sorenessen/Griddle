using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Griddle.App.Views;

public partial class MicrophoneDisconnectedDialog
    : Window
{
    public MicrophoneDisconnectedDialog()
    {
        InitializeComponent();
    }

    private void ContinueButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            MicrophoneDisconnectedChoice
                .ContinueWithoutMicrophone);
    }

    private void StopButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        Close(
            MicrophoneDisconnectedChoice
                .StopRecording);
    }
}
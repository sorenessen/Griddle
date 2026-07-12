using System;
using Avalonia.Controls;
using Griddle.Platform.MacOS;

namespace Griddle.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        MacOSWindowInterop.SetIgnoresMouseEvents(
            this,
            ignoresMouseEvents: true);
    }
}
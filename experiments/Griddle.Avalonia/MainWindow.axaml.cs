using System;
using Avalonia.Controls;
using Griddle.Avalonia.Platform;

namespace Griddle.Avalonia;

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
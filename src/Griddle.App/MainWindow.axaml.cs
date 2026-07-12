using System;
using Avalonia.Controls;
using Avalonia.Input;
using Griddle.Platform.MacOS;

namespace Griddle.App;

public partial class MainWindow : Window
{
    private bool _isDrawing;

    public MainWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        MacOSWindowInterop.SetIgnoresMouseEvents(
            this,
            ignoresMouseEvents: false);
    }

    private void Overlay_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        var currentPoint = e.GetCurrentPoint(DrawingSurface);

        if (!currentPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isDrawing = true;
        DrawingSurface.BeginStroke(currentPoint.Position);

        e.Pointer.Capture(this);
        e.Handled = true;
    }

    private void Overlay_PointerMoved(
        object? sender,
        PointerEventArgs e)
    {
        if (!_isDrawing)
        {
            return;
        }

        DrawingSurface.ContinueStroke(
            e.GetPosition(DrawingSurface));

        e.Handled = true;
    }

    private void Overlay_PointerReleased(
        object? sender,
        PointerReleasedEventArgs e)
    {
        if (!_isDrawing)
        {
            return;
        }

        DrawingSurface.EndStroke(
            e.GetPosition(DrawingSurface));

        _isDrawing = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }
}
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Griddle.Platform.MacOS;
using Griddle.Core.Models;

namespace Griddle.App;

public partial class MainWindow : Window
{
    private bool _isDrawing;

    public MainWindow()
    {
        InitializeComponent();

        Opened += OnOpened;
        KeyDown += OnKeyDown;
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

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.C:
                DrawingSurface.Clear();
                e.Handled = true;
                break;

            case Key.U:
                DrawingSurface.Undo();
                e.Handled = true;
                break;

            case Key.R:
                DrawingSurface.Redo();
                e.Handled = true;
                break;

            case Key.D1:
            case Key.NumPad1:
                DrawingSurface.SetColor(StrokeColor.Red);
                e.Handled = true;
                break;

            case Key.D2:
            case Key.NumPad2:
                DrawingSurface.SetColor(StrokeColor.Blue);
                e.Handled = true;
                break;

            case Key.D3:
            case Key.NumPad3:
                DrawingSurface.SetThickness(2);
                e.Handled = true;
                break;

            case Key.D4:
            case Key.NumPad4:
                DrawingSurface.SetThickness(6);
                e.Handled = true;
                break;
        }
    }

    private void ClearCanvas()
    {
        DrawingSurface.Clear();
    }

    private void Undo()
    {
        DrawingSurface.Undo();
    }
}
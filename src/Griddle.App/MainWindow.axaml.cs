using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Griddle.Platform.MacOS;
using Griddle.Core.Models;
using Griddle.Core.Tools;
using Griddle.App.Views;
using Griddle.App.ViewModels;

namespace Griddle.App;

public partial class MainWindow : Window
{

    private ToolbarWindow? _toolbar;
    private ToolbarViewModel? _toolbarViewModel;

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

// TODO:
// Introduce a ToolRegistry/ToolFactory so toolbar and drawing
// surface share the same tool instances.
        _toolbarViewModel = new ToolbarViewModel(
            DrawingSurface.Pen,
            new ArrowTool(),
            new RectangleTool(),
            new SelectionTool(DrawingSurface.Selection),
            DrawingSurface.ActiveTool);
        _toolbar = new ToolbarWindow(_toolbarViewModel);

        var overlayScreen = Screens.ScreenFromTopLevel(this);

        if (overlayScreen is not null)
        {
            var workingArea = overlayScreen.WorkingArea;

            _toolbar.Position = new PixelPoint(
                workingArea.X + 40,
                workingArea.Y + 40);
        }

        _toolbar.Show(this);
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
        DrawingSurface.BeginInteraction(currentPoint.Position);

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

        DrawingSurface.ContinueInteraction(
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

        DrawingSurface.EndInteraction(
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

            case Key.Delete:
            case Key.Back:
                DrawingSurface.DeleteSelection();
                e.Handled = true;
                break;

            case Key.Z:
            {
                var commandPressed =
                    e.KeyModifiers.HasFlag(KeyModifiers.Meta) ||
                    e.KeyModifiers.HasFlag(KeyModifiers.Control);

                if (!commandPressed)
                {
                    break;
                }

                if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
                {
                    DrawingSurface.Redo();
                }
                else
                {
                    DrawingSurface.Undo();
                }

                e.Handled = true;
                break;
            }

            case Key.Y:
            {
                var commandPressed =
                    e.KeyModifiers.HasFlag(KeyModifiers.Meta) ||
                    e.KeyModifiers.HasFlag(KeyModifiers.Control);

                if (!commandPressed)
                {
                    break;
                }

                DrawingSurface.Redo();
                e.Handled = true;
                break;
            }

            case Key.Left:
            {
                var distance =
                    e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                        ? 10
                        : 1;

                DrawingSurface.NudgeSelection(
                    -distance,
                    0);

                e.Handled = true;
                break;
            }

            case Key.Right:
            {
                var distance =
                    e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                        ? 10
                        : 1;

                DrawingSurface.NudgeSelection(
                    distance,
                    0);

                e.Handled = true;
                break;
            }

            case Key.Up:
            {
                var distance =
                    e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                        ? 10
                        : 1;

                DrawingSurface.NudgeSelection(
                    0,
                    -distance);

                e.Handled = true;
                break;
            }

            case Key.Down:
            {
                var distance =
                    e.KeyModifiers.HasFlag(KeyModifiers.Shift)
                        ? 10
                        : 1;

                DrawingSurface.NudgeSelection(
                    0,
                    distance);

                e.Handled = true;
                break;
            }

            case Key.P:
                _toolbarViewModel?.SelectPen(StrokeColor.Red);
                e.Handled = true;
                break;

            case Key.H:
                _toolbarViewModel?.SelectHighlighter();
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
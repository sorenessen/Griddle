using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
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
    private bool _isClickThrough;
    private bool _isTintEnabled = false;

    public MainWindow()
    {
        InitializeComponent();

        Opened += OnOpened;
        KeyDown += OnKeyDown;
        TextInput += OnTextInput;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        SetClickThrough(false);

// TODO:
// Introduce a ToolRegistry/ToolFactory so toolbar and drawing
// surface share the same tool instances.
        _toolbarViewModel = new ToolbarViewModel(
            DrawingSurface.Pen,
            new ArrowTool(),
            new RectangleTool(),
            new TextTool(),
            new CalloutTool(),
            new SelectionTool(DrawingSurface.Selection),
            DrawingSurface.ActiveTool);

        _toolbarViewModel.NewCalloutGroupRequested +=
            DrawingSurface.StartNewCalloutGroup;

        _toolbarViewModel.ContinueCalloutGroupRequested +=
            DrawingSurface.ContinueSelectedCalloutGroup;

        _toolbarViewModel.RenumberCalloutGroupRequested +=
            DrawingSurface.RenumberSelectedCalloutGroup;

        _toolbarViewModel.SelectCalloutGroupRequested += () =>
        {
            DrawingSurface.SelectSelectedCalloutGroup();

            Dispatcher.UIThread.Post(() =>
            {
                Activate();
                Focus();
            });
        };

        _toolbarViewModel.HideCalloutGroupRequested += () =>
        {
            DrawingSurface.HideSelectedCalloutGroup();

            Dispatcher.UIThread.Post(() =>
            {
                Activate();
                Focus();
            });
        };

        _toolbarViewModel.ShowLastHiddenCalloutGroupRequested += () =>
        {
            DrawingSurface.ShowLastHiddenCalloutGroup();

            Dispatcher.UIThread.Post(() =>
            {
                Activate();
                Focus();
            });
        };

        _toolbarViewModel.StartCalloutPresentationRequested += () =>
        {
            DrawingSurface.StartSelectedCalloutPresentation();

            _toolbarViewModel.SetCalloutPresentationActive(
                DrawingSurface.IsPresentingCalloutSequence);

            Dispatcher.UIThread.Post(() =>
            {
                Activate();
                Focus();
            });
        };

        _toolbarViewModel.SetPresentationProgress(
            DrawingSurface.PresentationRevealCount,
            DrawingSurface.PresentationTotalCount);

        _toolbarViewModel.ToggleOverlayInteractionRequested += () =>
        {
            SetClickThrough(
                !_isClickThrough);

            _toolbarViewModel.SetOverlayEngaged(
                !_isClickThrough);
        };

        _toolbarViewModel.ToggleTintRequested += () =>
        {
            SetTintEnabled(
                !_isTintEnabled);

            _toolbarViewModel.SetTintEnabled(
                _isTintEnabled);
        };

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

        Focus();

        _isDrawing = true;
        DrawingSurface.BeginInteraction(
            currentPoint.Position,
            e.ClickCount);

        e.Pointer.Capture(DrawingSurface);
        e.Handled = true;
    }

    private void SetTintEnabled(
        bool isEnabled)
    {
        _isTintEnabled = isEnabled;

        Background = isEnabled
            ? new SolidColorBrush(
                Color.Parse("#14FF8C00"))
            : Brushes.Transparent;
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

    private void SetClickThrough(
        bool isClickThrough)
    {
        _isClickThrough = isClickThrough;

        MacOSWindowInterop.SetIgnoresMouseEvents(
            this,
            ignoresMouseEvents: isClickThrough);
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DrawingSurface.IsEditingText)
        {
            switch (e.Key)
            {
                case Key.Back:
                    DrawingSurface.BackspaceText();
                    e.Handled = true;
                    return;

                case Key.Enter:
                    DrawingSurface.CommitText();
                    e.Handled = true;
                    return;

                case Key.Escape:
                    DrawingSurface.CancelText();
                    e.Handled = true;
                    return;
            }

            return;
        }
        
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

            case Key.Escape:
            {
                if (DrawingSurface.IsPresentingCalloutSequence)
                {
                    DrawingSurface.EndSelectedCalloutPresentation();

                    _toolbarViewModel?.SetCalloutPresentationActive(
                        false);

                    _toolbarViewModel?.SetPresentationProgress(
                        0,
                        0);

                    e.Handled = true;
                }

                break;
            }

            case Key.G:
            {
                var commandPressed =
                    e.KeyModifiers.HasFlag(KeyModifiers.Meta) ||
                    e.KeyModifiers.HasFlag(KeyModifiers.Control);

                var shiftPressed =
                    e.KeyModifiers.HasFlag(KeyModifiers.Shift);

                if (!commandPressed || !shiftPressed)
                {
                    break;
                }

                SetClickThrough(
                    !_isClickThrough);

                _toolbarViewModel?.SetOverlayEngaged(
                    !_isClickThrough);

                e.Handled = true;
                break;
            }

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
                if (DrawingSurface.IsPresentingCalloutSequence)
                {
                    DrawingSurface.RevealPreviousCallout();

                    _toolbarViewModel?.SetPresentationProgress(
                        DrawingSurface.PresentationRevealCount,
                        DrawingSurface.PresentationTotalCount);

                    e.Handled = true;
                    break;
                }

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
                if (DrawingSurface.IsPresentingCalloutSequence)
                {
                    DrawingSurface.RevealNextCallout();

                    _toolbarViewModel?.SetPresentationProgress(
                        DrawingSurface.PresentationRevealCount,
                        DrawingSurface.PresentationTotalCount);

                    e.Handled = true;
                    break;
                }

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

    private void OnTextInput(
        object? sender,
        TextInputEventArgs e)
    {
        if (!DrawingSurface.IsEditingText ||
            string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        DrawingSurface.AppendText(e.Text);
        e.Handled = true;
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
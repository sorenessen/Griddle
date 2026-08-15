using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Griddle.Platform.MacOS;
using Griddle.Platform.Capture;
using Griddle.Core.Models;
using Griddle.Core.Tools;
using Griddle.Core.Documents;
using Griddle.Core.Sessions;
using Griddle.Core.Captures;
using Griddle.App.Views;
using Griddle.App.ViewModels;
using Griddle.App.Services;

namespace Griddle.App;

public partial class MainWindow : Window
{

    private ToolbarWindow? _toolbar;
    private ToolbarViewModel? _toolbarViewModel;

    private Screen? _overlayScreen;

    private GriddleSession _currentSession =
        new();

    private string? _savedSessionSnapshot;

    private string? _currentSessionFilePath;

    private bool _isDrawing;
    private bool _isClickThrough;
    private bool _isTintEnabled = false;
    private bool _allowClose;
    private bool _isClosePromptOpen;

    public MainWindow()
    {
        InitializeComponent();

        NativeMenu.SetMenu(
            this,
            CreateSessionNativeMenu());

        Opened += OnOpened;
        KeyDown += OnKeyDown;
        TextInput += OnTextInput;


    }

    private void OnOpened(object? sender, EventArgs e)
    {
        var preferredDisplayName =
            DisplayPreferenceStore.Load();

        _overlayScreen = null;

        if (!string.IsNullOrWhiteSpace(
                preferredDisplayName))
        {
            foreach (var screen in Screens.All)
            {
                if (string.Equals(
                        screen.DisplayName,
                        preferredDisplayName,
                        StringComparison.Ordinal))
                {
                    _overlayScreen = screen;
                    break;
                }
            }
        }

        _overlayScreen ??=
            Screens.ScreenFromTopLevel(this)
            ?? Screens.Primary;

        if (_overlayScreen is not null)
        {
            MoveOverlayToScreen(
                _overlayScreen);
        }

        SetClickThrough(false);

// TODO:
// Introduce a ToolRegistry/ToolFactory so toolbar and drawing
// surface share the same tool instances.
        var annotationStyle =
            DrawingSurface.Pen.Settings;

        _toolbarViewModel = new ToolbarViewModel(
            DrawingSurface.Pen,
            new ArrowTool(annotationStyle),
            new RectangleTool(annotationStyle),
            new TextTool(annotationStyle),
            new CalloutTool(annotationStyle),
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

        _toolbarViewModel.FlipCalloutLabelRequested +=
            DrawingSurface.FlipSelectedCalloutLabel;

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

        NativeMenu.SetMenu(
            _toolbar,
            CreateSessionNativeMenu());

        if (_overlayScreen is not null)
        {
            var workingArea = _overlayScreen.WorkingArea;

            _toolbar.Position = new PixelPoint(
                workingArea.X + 40,
                workingArea.Y + 40);
        }

        _toolbarViewModel.SetDisplays(
            Screens.All.Count);

        _toolbarViewModel.SetActiveDisplay(
            GetOverlayScreenIndex());

        _toolbarViewModel.DisplayRequested +=
            MoveOverlayToScreenIndex;

        Screens.Changed += Screens_Changed;

        _toolbar.Show(this);
    }

    // private void SaveTestSession()
    // {
    //     var session = new GriddleSession
    //     {
    //         Name = "Griddle Test Session",
    //         Strokes = DrawingSurface
    //             .GetStrokesSnapshot()
    //             .ToList()
    //     };

    //     var filePath =
    //         Path.Combine(
    //             Environment.GetFolderPath(
    //                 Environment.SpecialFolder.Desktop),
    //             "griddle-test.griddle");

    //     GriddleSessionFileService.Save(
    //         session,
    //         filePath);

    //     Console.WriteLine(
    //         $"Saved session to: {filePath}");
    // }

    // private void LoadTestSession()
    // {
    //     var filePath =
    //         Path.Combine(
    //             Environment.GetFolderPath(
    //                 Environment.SpecialFolder.Desktop),
    //             "griddle-test.griddle");

    //     var session =
    //         GriddleSessionFileService.Load(
    //             filePath);

    //     DrawingSurface.LoadStrokes(
    //         session.Strokes);

    //     Console.WriteLine(
    //         $"Loaded session: {session.Name}");

    //     Console.WriteLine(
    //         $"Loaded strokes: {session.Strokes.Count}");
    // }

    public async void NewSession()
    {
        if (HasUnsavedChanges())
        {
            var dialog =
                new UnsavedChangesDialog();

            var choice =
                await dialog.ShowDialog<UnsavedChangesChoice>(
                    this);

            if (choice ==
                UnsavedChangesChoice.Cancel)
            {
                return;
            }

            if (choice ==
                UnsavedChangesChoice.Save)
            {
                var saved =
                    await SaveSessionAsync();

                if (!saved)
                {
                    return;
                }
            }
        }

        _currentSession =
            new GriddleSession();

        _currentSessionFilePath =
            null;

        _savedSessionSnapshot =
            null;

        DrawingSurface.Clear();
    }

    public async void OpenSession()
    {
        if (HasUnsavedChanges())
        {
            var dialog =
                new UnsavedChangesDialog();

            var choice =
                await dialog.ShowDialog<UnsavedChangesChoice>(
                    this);

            if (choice ==
                UnsavedChangesChoice.Cancel)
            {
                return;
            }

            if (choice ==
                UnsavedChangesChoice.Save)
            {
                var saved =
                    await SaveSessionAsync();

                if (!saved)
                {
                    return;
                }
            }
        }

        var files =
            await StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    Title = "Open Griddle Session",
                    AllowMultiple = false,

                    FileTypeFilter =
                    [
                        new FilePickerFileType(
                            "Griddle Session")
                        {
                            Patterns =
                            [
                                "*.griddle"
                            ]
                        }
                    ]
                });

        if (files.Count == 0)
        {
            return;
        }

        var file =
            files[0];

        var session =
            GriddleSessionFileService.Load(
                file.Path.LocalPath);

        _currentSession =
            session;

        _currentSessionFilePath =
            file.Path.LocalPath;

        DrawingSurface.LoadStrokes(
            session.Strokes);

        _savedSessionSnapshot =
            GetCurrentSessionSnapshot();
    }

    public async Task<bool> SaveSessionAsync()
    {
        if (string.IsNullOrWhiteSpace(
                _currentSessionFilePath))
        {
            return await SaveSessionAsAsync();
        }

        _currentSession.Strokes =
            DrawingSurface
                .GetStrokesSnapshot()
                .ToList();

        GriddleSessionFileService.Save(
            _currentSession,
            _currentSessionFilePath);

        _savedSessionSnapshot =
            GetCurrentSessionSnapshot();

        return true;
    }

    public async Task<bool> SaveSessionAsAsync()
    {
        var file =
            await StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    Title = "Save Griddle Session",
                    SuggestedFileName =
                        _currentSession.Name,

                    FileTypeChoices =
                    [
                        new FilePickerFileType(
                            "Griddle Session")
                        {
                            Patterns =
                            [
                                "*.griddle"
                            ]
                        }
                    ],

                    DefaultExtension =
                        "griddle"
                });

        if (file is null)
        {
            return false;
        }
        _currentSession.Strokes =
            DrawingSurface
                .GetStrokesSnapshot()
                .ToList();

        _currentSessionFilePath =
            file.Path.LocalPath;

        GriddleSessionFileService.Save(
            _currentSession,
            _currentSessionFilePath);

        _savedSessionSnapshot =
            GetCurrentSessionSnapshot();

        return true;
    }

    private NativeMenu CreateSessionNativeMenu()
    {
        var fileMenu = new NativeMenu();

        var newSession =
            new NativeMenuItem("New Session")
            {
                Gesture = new KeyGesture(
                    Key.N,
                    KeyModifiers.Meta)
            };

        newSession.Click +=
            (_, _) => NewSession();

        var openSession =
            new NativeMenuItem("Open Session...")
            {
                Gesture = new KeyGesture(
                    Key.O,
                    KeyModifiers.Meta)
            };

        openSession.Click +=
            (_, _) => OpenSession();

        var saveSession =
            new NativeMenuItem("Save")
            {
                Gesture = new KeyGesture(
                    Key.S,
                    KeyModifiers.Meta)
            };

        saveSession.Click +=
            async (_, _) =>
                await SaveSessionAsync();

        var saveSessionAs =
            new NativeMenuItem("Save As...")
            {
                Gesture = new KeyGesture(
                    Key.S,
                    KeyModifiers.Meta |
                    KeyModifiers.Shift)
            };

        saveSessionAs.Click +=
            async (_, _) =>
                await SaveSessionAsAsync();

        fileMenu.Add(newSession);
        fileMenu.Add(openSession);
        fileMenu.Add(
            new NativeMenuItemSeparator());
        fileMenu.Add(saveSession);
        fileMenu.Add(saveSessionAs);

        var rootMenu = new NativeMenu();

        rootMenu.Add(
            new NativeMenuItem("File")
            {
                Menu = fileMenu
            });

        return rootMenu;
    }

    // private void TestCaptureRoundTrip()
    // {
    //     var session = new GriddleSession
    //     {
    //         Name = "Capture Round Trip Test",
    //         Strokes = DrawingSurface
    //             .GetStrokesSnapshot()
    //             .ToList(),

    //         Captures =
    //         [
    //             new GriddleCapture
    //             {
    //                 Kind = CaptureKind.Screenshot,
    //                 FileName = "capture-001.png",
    //                 Width = 1920,
    //                 Height = 1080,
    //                 DisplayName = "Test Display",
    //                 IncludesAnnotations = true
    //             }
    //         ]
    //     };

    //     var document =
    //         GriddleDocumentMapper.ToDocument(
    //             session);

    //     var json =
    //         GriddleDocumentSerializer.Serialize(
    //             document);

    //     Console.WriteLine(json);

    //     var restoredDocument =
    //         GriddleDocumentSerializer.Deserialize(
    //             json);

    //     var restoredSession =
    //         GriddleDocumentMapper.ToSession(
    //             restoredDocument);

    //     var restoredCapture =
    //         restoredSession.Captures.Single();

    //     Console.WriteLine(
    //         $"Restored captures: " +
    //         $"{restoredSession.Captures.Count}");

    //     Console.WriteLine(
    //         $"Kind: {restoredCapture.Kind}");

    //     Console.WriteLine(
    //         $"File: {restoredCapture.FileName}");

    //     Console.WriteLine(
    //         $"Size: " +
    //         $"{restoredCapture.Width}x" +
    //         $"{restoredCapture.Height}");

    //     Console.WriteLine(
    //         $"Display: " +
    //         $"{restoredCapture.DisplayName}");

    //     Console.WriteLine(
    //         $"Annotations: " +
    //         $"{restoredCapture.IncludesAnnotations}");
    // }

    private async Task CaptureActiveDisplayAsync()
    {
        if (_overlayScreen is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(
                _currentSessionFilePath))
        {
            var saved =
                await SaveSessionAsAsync();

            if (!saved ||
                string.IsNullOrWhiteSpace(
                    _currentSessionFilePath))
            {
                return;
            }
        }

        var captureService =
            new MacOSScreenCaptureService();

        var bounds =
            _overlayScreen.Bounds;

        var region =
            new CaptureRegion(
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height);

        var result =
            await captureService.CaptureAsync(
                region);

        var sessionDirectory =
            Path.GetDirectoryName(
                _currentSessionFilePath)!;

        var sessionName =
            Path.GetFileNameWithoutExtension(
                _currentSessionFilePath);

        var mediaFolderName =
            $"{sessionName}.media";

        var mediaDirectory =
            Path.Combine(
                sessionDirectory,
                mediaFolderName);

        Directory.CreateDirectory(
            mediaDirectory);

        var captureId =
            Guid.NewGuid();

        var fileName =
            $"capture-{captureId:N}.png";

        var filePath =
            Path.Combine(
                mediaDirectory,
                fileName);

        await File.WriteAllBytesAsync(
            filePath,
            result.ImageData);

        var capture =
            new GriddleCapture
            {
                Id = captureId,
                Kind = CaptureKind.Screenshot,
                CreatedAt = DateTime.UtcNow,
                FileName = fileName,
                Width = result.Width,
                Height = result.Height,
                DisplayName =
                    _overlayScreen.DisplayName,
                IncludesAnnotations = true
            };

        _currentSession.Captures.Add(
            capture);

        Console.WriteLine(
            $"Captured {_overlayScreen.DisplayName}");

        Console.WriteLine(
            $"Saved capture: {filePath}");
    }

    protected override void OnClosing(
        WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        if (_allowClose ||
            !HasUnsavedChanges())
        {
            return;
        }

        e.Cancel = true;

        if (_isClosePromptOpen)
        {
            return;
        }

        _isClosePromptOpen = true;

        _ = ConfirmCloseAsync();
    }

    private async Task ConfirmCloseAsync()
    {
        try
        {
            var dialog =
                new UnsavedChangesDialog();

            var choice =
                await dialog.ShowDialog<UnsavedChangesChoice>(
                    this);

            if (choice ==
                UnsavedChangesChoice.Cancel)
            {
                return;
            }

            if (choice ==
                UnsavedChangesChoice.Save)
            {
                var saved =
                    await SaveSessionAsync();

                if (!saved)
                {
                    return;
                }
            }

            _allowClose = true;

            Close();
        }
        finally
        {
            _isClosePromptOpen = false;
        }
    }

    private string GetCurrentSessionSnapshot()
    {
        _currentSession.Strokes =
            DrawingSurface
                .GetStrokesSnapshot()
                .ToList();

        var document =
            GriddleDocumentMapper.ToDocument(
                _currentSession);

        return GriddleDocumentSerializer.Serialize(
            document);
    }

    private bool HasUnsavedChanges()
    {
        var currentSnapshot =
            GetCurrentSessionSnapshot();

        if (_savedSessionSnapshot is null)
        {
            return _currentSession.Strokes.Count > 0;
        }

        return !string.Equals(
            currentSnapshot,
            _savedSessionSnapshot,
            StringComparison.Ordinal);
    }

    private void MoveOverlayToScreen(
        Screen screen)
    {
        var bounds = screen.Bounds;

        WindowState = WindowState.Normal;

        Position = new PixelPoint(
            bounds.X,
            bounds.Y);

        Width =
            bounds.Width / screen.Scaling;

        Height =
            bounds.Height / screen.Scaling;
    }

    private void MoveOverlayToNextScreen()
    {
        var screens = Screens.All;

        if (screens.Count < 2)
        {
            return;
        }

        var currentIndex = -1;

        if (_overlayScreen is not null)
        {
            for (var index = 0; index < screens.Count; index++)
            {
                if (ReferenceEquals(
                    screens[index],
                    _overlayScreen))
                {
                    currentIndex = index;
                    break;
                }
            }
        }

        var nextIndex =
            (currentIndex + 1) % screens.Count;

        _overlayScreen =
            screens[nextIndex];

        DisplayPreferenceStore.Save(
            _overlayScreen.DisplayName);

        MoveOverlayToScreen(
            _overlayScreen);

        _toolbarViewModel?.SetActiveDisplay(
            nextIndex);
    }

    private void MoveOverlayToScreenIndex(
        int index)
    {
        var screens = Screens.All;

        if (index < 0 ||
            index >= screens.Count)
        {
            return;
        }

        _overlayScreen =
            screens[index];

        DisplayPreferenceStore.Save(
            _overlayScreen.DisplayName);

        MoveOverlayToScreen(
            _overlayScreen);

        _toolbarViewModel?.SetActiveDisplay(
            index);
    }

    private int GetOverlayScreenIndex()
    {
        var screens = Screens.All;

        if (_overlayScreen is null)
        {
            return -1;
        }

        for (var index = 0; index < screens.Count; index++)
        {
            if (ReferenceEquals(
                screens[index],
                _overlayScreen))
            {
                return index;
            }
        }

        return -1;
    }

    private void Overlay_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        var currentPoint =
            e.GetCurrentPoint(DrawingSurface);

        if (!currentPoint.Properties.IsLeftButtonPressed)
        {
            return;
        }

        DrawingSurface.Focus();

        var additiveSelection =
            e.KeyModifiers.HasFlag(
                KeyModifiers.Shift);

        _isDrawing = true;

        DrawingSurface.BeginInteraction(
            currentPoint.Position,
            e.ClickCount,
            additiveSelection);

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

        if (DrawingSurface.IsEditingText)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    DrawingSurface.Focus();
                });
        }
    }

    private void Screens_Changed(
        object? sender,
        EventArgs e)
    {
        _toolbarViewModel?.SetDisplays(
            Screens.All.Count);

        var currentScreen =
            Screens.ScreenFromWindow(this);

        if (currentScreen is null)
        {
            return;
        }

        MoveOverlayToScreen(
            currentScreen);

        var currentIndex = -1;

        for (var index = 0;
             index < Screens.All.Count;
             index++)
        {
            if (ReferenceEquals(
                Screens.All[index],
                currentScreen))
            {
                currentIndex = index;
                break;
            }
        }

        if (currentIndex >= 0)
        {
            _toolbarViewModel?.SetActiveDisplay(
                currentIndex);
        }
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
        if (e.Key == Key.M &&
            e.KeyModifiers.HasFlag(KeyModifiers.Meta) &&
            e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            MoveOverlayToNextScreen();

            e.Handled = true;
            return;
        }

        if (e.Key == Key.C &&
            e.KeyModifiers.HasFlag(
                KeyModifiers.Meta) &&
            e.KeyModifiers.HasFlag(
                KeyModifiers.Shift))
        {
            _ = CaptureActiveDisplayAsync();

            e.Handled = true;
            return;
        }

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

            case Key.F:
                DrawingSurface.FlipSelectedCalloutLabel();
                e.Handled = true;
                break;

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

            // case Key.R:
            //     _ = TestScreenCaptureAsync();
            //     e.Handled = true;
            //     break;

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
                _toolbarViewModel?.SelectPen();
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
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Griddle.Core.Models;
using Griddle.Core.Services;
using Griddle.Core.Tools;

namespace Griddle.App.ViewModels;

public sealed class ToolbarViewModel : INotifyPropertyChanged
{
    public ToolbarViewModel(PenTool pen)
        : this(
            pen,
            new ArrowTool(pen.Settings),
            new RectangleTool(pen.Settings),
            new TextTool(pen.Settings),
            new CalloutTool(pen.Settings),
            new SelectionTool(new SelectionService()),
            new ActiveToolService(pen))
    {
    }

    public ToolbarViewModel(
        PenTool pen,
        ArrowTool arrow,
        RectangleTool rectangle,
        TextTool text,
        CalloutTool callout,
        SelectionTool selection,
        ActiveToolService activeTool)
    {
        Pen = pen;
        Arrow = arrow;
        Rectangle = rectangle;
        Text = text;
        Callout = callout;
        Selection = selection;
        ActiveTool = activeTool;

        ActiveTool.CurrentToolChanged +=
            OnCurrentToolChanged;
    }


    public PenTool Pen { get; }

    public ArrowTool Arrow { get; }

    public RectangleTool Rectangle { get; }

    public TextTool Text { get; }

    public CalloutTool Callout { get; }

    public bool IsCalloutPresentationActive { get; private set; }

    public bool IsTintEnabled { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? NewCalloutGroupRequested;

    public event Action? ContinueCalloutGroupRequested;

    public event Action? RenumberCalloutGroupRequested;

    public event Action? SelectCalloutGroupRequested;

    public event Action? HideCalloutGroupRequested;

    public event Action? ShowLastHiddenCalloutGroupRequested;

    public event Action? StartCalloutPresentationRequested;

    public event Action? ToggleOverlayInteractionRequested;

    public event Action? ToggleTintRequested;

    public event Action<int>? DisplayRequested;

    public SelectionTool Selection { get; }

    public ActiveToolService ActiveTool { get; }

    public int PresentationRevealCount { get; private set; }

    public int PresentationTotalCount { get; private set; }

    public int ActiveDisplayIndex { get; private set; } = -1;

    public string PresentationProgressText =>
        $"{PresentationRevealCount} / {PresentationTotalCount}";

    public ObservableCollection<DisplayOption> Displays { get; } = [];

    public ObservableCollection<ColorPreset> Colors { get; } =
    [
        new("Red", StrokeColor.Red),
        new("Orange", StrokeColor.Orange),
        new("Yellow", StrokeColor.Yellow),
        new("Green", StrokeColor.Green),
        new("Blue", StrokeColor.Blue),
        new("Purple", StrokeColor.Purple),
        new("White", StrokeColor.White),
        new("Black", StrokeColor.Black)
    ];

    public string SelectedColorBrush =>
        SelectedColor switch
        {
            StrokeColor.Red => "Red",
            StrokeColor.Orange => "Orange",
            StrokeColor.Yellow => "Yellow",
            StrokeColor.Green => "LimeGreen",
            StrokeColor.Blue => "DodgerBlue",
            StrokeColor.Purple => "MediumPurple",
            StrokeColor.White => "White",
            StrokeColor.Black => "Black",
            _ => "Red"
        };

    public StrokeColor SelectedColor =>
        Pen.Settings.Color;

    public double SelectedThickness =>
        Pen.Settings.Thickness;

    public bool IsOverlayEngaged { get; private set; } = true;

    public bool IsOverlayDisengaged =>
        !IsOverlayEngaged;

    public void SetTintEnabled(
        bool isEnabled)
    {
        if (IsTintEnabled == isEnabled)
        {
            return;
        }

        IsTintEnabled = isEnabled;

        OnPropertyChanged(
            nameof(IsTintEnabled));
    }

    public void SetOverlayEngaged(
        bool isEngaged)
    {
        if (IsOverlayEngaged == isEngaged)
        {
            return;
        }

        IsOverlayEngaged = isEngaged;

        OnPropertyChanged(
            nameof(IsOverlayEngaged));

        OnPropertyChanged(
            nameof(IsOverlayDisengaged));
    }

    public void SelectThickness(
        double thickness)
    {
        Pen.Settings.Thickness = thickness;

        OnPropertyChanged(
            nameof(SelectedThickness));
    }

    public void ToggleTint()
    {
        ToggleTintRequested?.Invoke();
    }

    public bool IsPenSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Preset == PenPreset.Pen;

    public bool IsRedSelected =>
        Pen.Settings.Color == StrokeColor.Red;

    public bool IsOrangeSelected =>
        Pen.Settings.Color == StrokeColor.Orange;

    public bool IsYellowSelected =>
        Pen.Settings.Color == StrokeColor.Yellow;

    public bool IsGreenSelected =>
        Pen.Settings.Color == StrokeColor.Green;

    public bool IsBlueSelected =>
        Pen.Settings.Color == StrokeColor.Blue;

    public bool IsPurpleSelected =>
        Pen.Settings.Color == StrokeColor.Purple;

    public bool IsWhiteSelected =>
        Pen.Settings.Color == StrokeColor.White;

    public bool IsBlackSelected =>
        Pen.Settings.Color == StrokeColor.Black;

    public bool IsHighlighterSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Preset == PenPreset.Highlighter;

    public bool IsArrowSelected =>
        ReferenceEquals(ActiveTool.Current, Arrow);

    public bool IsRectangleSelected =>
        ReferenceEquals(ActiveTool.Current, Rectangle);

    public bool IsTextSelected =>
        ReferenceEquals(ActiveTool.Current, Text);

    public bool IsCalloutSelected =>
        ReferenceEquals(ActiveTool.Current, Callout);

    public bool IsSelectionSelected =>
        ReferenceEquals(ActiveTool.Current, Selection);

    public void SelectPen()
    {
        Pen.Preset = PenPreset.Pen;

        ActiveTool.Current = Pen;
        NotifySelectionChanged();
    }

    public void SelectColor(
        StrokeColor color)
    {
        Pen.Settings.Color = color;

        OnPropertyChanged(
            nameof(SelectedColor));

        OnPropertyChanged(
            nameof(SelectedColorBrush));

        NotifySelectionChanged();
    }

    public void SelectHighlighter()
    {
        Pen.Preset = PenPreset.Highlighter;

        ActiveTool.Current = Pen;
        NotifySelectionChanged();
    }

    public void SelectArrow()
    {
        ActiveTool.Current = Arrow;
        NotifySelectionChanged();
    }

    public void SelectRectangle()
    {
        ActiveTool.Current = Rectangle;
        NotifySelectionChanged();
    }

    public void SelectText()
    {
        ActiveTool.Current = Text;
        NotifySelectionChanged();
    }

    public void SelectCallout()
    {
        ActiveTool.Current = Callout;
        NotifySelectionChanged();
    }

    public void StartNewCalloutGroup()
    {
        NewCalloutGroupRequested?.Invoke();
    }

    public void ContinueCalloutGroup()
    {
        ContinueCalloutGroupRequested?.Invoke();

        ActiveTool.Current = Callout;
        NotifySelectionChanged();
    }

    public void RenumberCalloutGroup()
    {
        RenumberCalloutGroupRequested?.Invoke();
    }

    public void SelectCalloutGroup()
    {
        SelectCalloutGroupRequested?.Invoke();

        ActiveTool.Current = Selection;
        NotifySelectionChanged();
    }

    public void HideCalloutGroup()
    {
        HideCalloutGroupRequested?.Invoke();
    }

    public void ShowLastHiddenCalloutGroup()
    {
        ShowLastHiddenCalloutGroupRequested?.Invoke();
    }

    public void StartCalloutPresentation()
    {
        StartCalloutPresentationRequested?.Invoke();
    }

    public void SetCalloutPresentationActive(
        bool isActive)
    {
        if (IsCalloutPresentationActive == isActive)
        {
            return;
        }

        IsCalloutPresentationActive = isActive;

        OnPropertyChanged(
            nameof(IsCalloutPresentationActive));
    }

    public void SetPresentationProgress(
        int revealed,
        int total)
    {
        PresentationRevealCount = revealed;
        PresentationTotalCount = total;

        OnPropertyChanged(
            nameof(PresentationRevealCount));

        OnPropertyChanged(
            nameof(PresentationTotalCount));

        OnPropertyChanged(
            nameof(PresentationProgressText));
    }

    public void ToggleOverlayInteraction()
    {
        ToggleOverlayInteractionRequested?.Invoke();
    }

    public void SetDisplays(
        int count)
    {
        Displays.Clear();

        for (var index = 0; index < count; index++)
        {
            Displays.Add(
                new DisplayOption(
                    index,
                    $"Display {index + 1}"));
        }
    }

    public void RequestDisplay(
            int index)
        {
            DisplayRequested?.Invoke(index);
        }

    public void SetActiveDisplay(
        int index)
    {
        ActiveDisplayIndex = index;

        foreach (var display in Displays)
        {
            display.IsActive =
                display.Index == index;
        }

        OnPropertyChanged(
            nameof(ActiveDisplayIndex));
    }

    public void SelectSelection()
    {
        ActiveTool.Current = Selection;
        NotifySelectionChanged();
    }

    private void OnCurrentToolChanged(
        object? sender,
        EventArgs e)
    {
        NotifySelectionChanged();
    }

    private void NotifySelectionChanged()
    {
        OnPropertyChanged(nameof(IsPenSelected));
        OnPropertyChanged(nameof(IsHighlighterSelected));

        OnPropertyChanged(nameof(IsRedSelected));
        OnPropertyChanged(nameof(IsOrangeSelected));
        OnPropertyChanged(nameof(IsYellowSelected));
        OnPropertyChanged(nameof(IsGreenSelected));
        OnPropertyChanged(nameof(IsBlueSelected));
        OnPropertyChanged(nameof(IsPurpleSelected));
        OnPropertyChanged(nameof(IsWhiteSelected));
        OnPropertyChanged(nameof(IsBlackSelected));
        
        OnPropertyChanged(nameof(IsArrowSelected));
        OnPropertyChanged(nameof(IsRectangleSelected));
        OnPropertyChanged(nameof(IsTextSelected));
        OnPropertyChanged(nameof(IsCalloutSelected));
        OnPropertyChanged(nameof(IsSelectionSelected));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
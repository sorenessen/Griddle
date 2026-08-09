using System;
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
            new ArrowTool(),
            new RectangleTool(),
            new TextTool(),
            new CalloutTool(),
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

    public event PropertyChangedEventHandler? PropertyChanged;

    public event Action? NewCalloutGroupRequested;

    public event Action? ContinueCalloutGroupRequested;

    public event Action? RenumberCalloutGroupRequested;

    public event Action? SelectCalloutGroupRequested;

    public event Action? HideCalloutGroupRequested;

    public event Action? ShowLastHiddenCalloutGroupRequested;

    public event Action? StartCalloutPresentationRequested;

    public event Action? ToggleOverlayInteractionRequested;

    public SelectionTool Selection { get; }

    public ActiveToolService ActiveTool { get; }

    public int PresentationRevealCount { get; private set; }

    public int PresentationTotalCount { get; private set; }

    public string PresentationProgressText =>
        $"{PresentationRevealCount} / {PresentationTotalCount}";

    // TODO: Bring back when switched to an ItemsControl-driven palette.
    // public ObservableCollection<ColorPreset> Colors { get; } =
    // [
    //     new("Red", StrokeColor.Red),
    //     new("Blue", StrokeColor.Blue),
    //     new("Black", StrokeColor.Black)
    // ];

    public bool IsOverlayEngaged { get; private set; } = true;

    public bool IsOverlayDisengaged =>
        !IsOverlayEngaged;

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

    public bool IsPenSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Settings.Preset == PenPreset.Pen;

    public bool IsBlueSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Settings.Preset == PenPreset.Pen &&
        Pen.Settings.Color == StrokeColor.Blue;

    public bool IsBlackSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Settings.Preset == PenPreset.Pen &&
        Pen.Settings.Color == StrokeColor.Black;

    public bool IsHighlighterSelected =>
        ReferenceEquals(ActiveTool.Current, Pen) &&
        Pen.Settings.Preset == PenPreset.Highlighter;

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

    public void SelectPen(StrokeColor color)
    {
        Pen.Settings.Preset = PenPreset.Pen;
        Pen.Settings.Color = color;
        Pen.Settings.Thickness = 4;
        Pen.Settings.Opacity = 1.0;

        ActiveTool.Current = Pen;
        NotifySelectionChanged();
    }

    public void SelectHighlighter()
    {
        Pen.Settings.Preset = PenPreset.Highlighter;
        Pen.Settings.Color = StrokeColor.Yellow;
        Pen.Settings.Thickness = 16;
        Pen.Settings.Opacity = 0.18;

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
        OnPropertyChanged(nameof(IsBlueSelected));
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
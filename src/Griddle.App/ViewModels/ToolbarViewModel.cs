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
            new ActiveToolService(pen))
    {
    }

    public ToolbarViewModel(
        PenTool pen,
        ArrowTool arrow,
        ActiveToolService activeTool)
    {
        Pen = pen;
        Arrow = arrow;
        ActiveTool = activeTool;

        ActiveTool.CurrentToolChanged +=
            OnCurrentToolChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PenTool Pen { get; }

    public ArrowTool Arrow { get; }

    public ActiveToolService ActiveTool { get; }

    // TODO: Bring back when switched to an ItemsControl-driven palette.
    // public ObservableCollection<ColorPreset> Colors { get; } =
    // [
    //     new("Red", StrokeColor.Red),
    //     new("Blue", StrokeColor.Blue),
    //     new("Black", StrokeColor.Black)
    // ];

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
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
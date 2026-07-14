using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Griddle.Core.Models;
using Griddle.Core.Tools;

namespace Griddle.App.ViewModels;

public sealed class ToolbarViewModel : INotifyPropertyChanged
{
    public ToolbarViewModel(PenTool pen)
    {
        Pen = pen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PenTool Pen { get; }

// *** TODO: Bring back when switched to ItemsControl-driven palette ***
    // public ObservableCollection<ColorPreset> Colors { get; } =
    // [
    //     new("Red", StrokeColor.Red),
    //     new("Blue", StrokeColor.Blue),
    //     new("Black", StrokeColor.Black)
    // ];

    public bool IsPenSelected =>
        Pen.Settings.Preset == PenPreset.Pen;

    public bool IsBlueSelected =>
        Pen.Settings.Preset == PenPreset.Pen &&
        Pen.Settings.Color == StrokeColor.Blue;

    public bool IsBlackSelected =>
        Pen.Settings.Preset == PenPreset.Pen &&
        Pen.Settings.Color == StrokeColor.Black;

    public bool IsHighlighterSelected =>
        Pen.Settings.Preset == PenPreset.Highlighter;

    public void SelectPen(StrokeColor color)
    {
        Pen.Settings.Preset = PenPreset.Pen;
        Pen.Settings.Color = color;
        Pen.Settings.Thickness = 4;
        Pen.Settings.Opacity = 1.0;

        NotifySelectionChanged();
    }

    public void SelectHighlighter()
    {
        Pen.Settings.Preset = PenPreset.Highlighter;
        Pen.Settings.Color = StrokeColor.Yellow;
        Pen.Settings.Thickness = 16;
        Pen.Settings.Opacity = 0.18;

        NotifySelectionChanged();
    }

    private void NotifySelectionChanged()
    {
        OnPropertyChanged(nameof(IsPenSelected));
        OnPropertyChanged(nameof(IsHighlighterSelected));
        OnPropertyChanged(nameof(IsBlueSelected));
        OnPropertyChanged(nameof(IsBlackSelected));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
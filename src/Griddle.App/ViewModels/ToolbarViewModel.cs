using System.ComponentModel;
using System.Runtime.CompilerServices;
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

    public bool IsPenSelected =>
        Pen.Settings.Preset == PenPreset.Pen;

    public bool IsHighlighterSelected =>
        Pen.Settings.Preset == PenPreset.Highlighter;

    public void SelectPen()
    {
        Pen.Settings.Preset = PenPreset.Pen;
        Pen.Settings.Color = StrokeColor.Red;
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
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
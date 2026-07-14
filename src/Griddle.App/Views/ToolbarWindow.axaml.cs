using Avalonia.Controls;
using Avalonia.Interactivity;
using Griddle.App.ViewModels;
using Griddle.Core.Models;
using Griddle.Core.Tools;
using Avalonia;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Griddle.App.Views;

public partial class ToolbarWindow : Window
{
    private readonly ToolbarViewModel _viewModel;

    public ToolbarWindow()
        : this(
            new ToolbarViewModel(
                new PenTool(
                    new PenSettings())))
    {
    }

    public ToolbarWindow(ToolbarViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = viewModel;
    }

    private void RedButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectPen();
    }

    private void HighlighterButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectHighlighter();
    }

    private void ToolbarBackground_PointerPressed(
        object? sender,
        PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);

        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        if (e.Source is Visual source &&
            source.FindAncestorOfType<Button>(includeSelf: true) is not null)
        {
            return;
        }

        BeginMoveDrag(e);
    }
}
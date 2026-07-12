using Avalonia.Controls;
using Avalonia.Interactivity;
using Griddle.App.ViewModels;
using Griddle.Core.Models;

namespace Griddle.App.Views;

public partial class ToolbarWindow : Window
{
    private readonly ToolbarViewModel _viewModel;

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
        _viewModel.Pen.Settings.Color = StrokeColor.Red;
        _viewModel.Pen.Settings.Thickness = 4;
        _viewModel.Pen.Settings.Opacity = 1.0;
    }

    private void HighlighterButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.Pen.Settings.Color = StrokeColor.Yellow;
        _viewModel.Pen.Settings.Thickness = 16;
        _viewModel.Pen.Settings.Opacity = 0.18;
    }
}
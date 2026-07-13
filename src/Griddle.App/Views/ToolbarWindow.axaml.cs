using Avalonia.Controls;
using Avalonia.Interactivity;
using Griddle.App.ViewModels;
using Griddle.Core.Models;
using Griddle.Core.Tools;

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
}
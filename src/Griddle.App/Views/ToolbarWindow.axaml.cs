using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
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

    private void SelectionButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectSelection();
    }

    private void RedButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectPen(StrokeColor.Red);
    }

    private void BlueButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectPen(StrokeColor.Blue);
    }

    private void BlackButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectPen(StrokeColor.Black);
    }

    private void ArrowButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectArrow();
    }

    private void RectangleButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectRectangle();
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
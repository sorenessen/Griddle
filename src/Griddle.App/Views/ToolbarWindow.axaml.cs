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

    private void TextButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectText();
    }

    private void CalloutButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectCallout();
    }

    private void NewCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.StartNewCalloutGroup();
    }

    private void SelectCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectCalloutGroup();
    }

    private void HideCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.HideCalloutGroup();
    }

    private void ShowLastHiddenCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.ShowLastHiddenCalloutGroup();
    }

    private void StartCalloutPresentationButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.StartCalloutPresentation();
    }

    private void ContinueCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.ContinueCalloutGroup();
    }

    private void RenumberCalloutGroupButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.RenumberCalloutGroup();
    }

    private void HighlighterButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectHighlighter();
    }

    private void GriddleKnob_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.ToggleOverlayInteraction();
    }

    private void TintButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.ToggleTint();
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
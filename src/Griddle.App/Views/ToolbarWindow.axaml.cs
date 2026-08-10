using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Griddle.App.Services;
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
                    new AnnotationStyle())))
    {
    }

    public ToolbarWindow(ToolbarViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = viewModel;

        Opened += ToolbarWindow_Opened;
        PositionChanged += ToolbarWindow_PositionChanged;
    }

    private void SelectionButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectSelection();
    }

    private void PenButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectPen();
    }

    private void RedColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Red);
    }

    private void OrangeColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Orange);
    }

    private void YellowColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Yellow);
    }

    private void GreenColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Green);
    }

    private void BlueColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Blue);
    }

    private void PurpleColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Purple);
    }

    private void WhiteColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.White);
    }

    private void BlackColorButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectColor(StrokeColor.Black);
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

    private void Thickness2Button_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectThickness(2);
    }

    private void Thickness4Button_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectThickness(4);
    }

    private void Thickness6Button_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectThickness(6);
    }

    private void Thickness8Button_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.SelectThickness(8);
    }

    private void TintButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _viewModel.ToggleTint();
    }

    private void DisplayOptionButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (sender is not Button button ||
            button.DataContext is not DisplayOption display)
        {
            return;
        }

        _viewModel.RequestDisplay(
            display.Index);
    }

    private void ToolbarWindow_Opened(
        object? sender,
        EventArgs e)
    {
        var savedPosition =
            ToolbarPositionStore.Load();

        if (savedPosition is not null)
        {
            Position = savedPosition.Value;
        }
    }

    private void ToolbarWindow_PositionChanged(
        object? sender,
        PixelPointEventArgs e)
    {
        ToolbarPositionStore.Save(
            Position);
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
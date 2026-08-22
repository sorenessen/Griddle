using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace Griddle.App.Views;

public partial class StartupSplashWindow : Window
{
    public StartupSplashWindow()
    {
        InitializeComponent();

        Opened += OnOpened;
        PointerPressed += SkipAnimation;
        KeyDown += SkipAnimation;
    }

    private async void OnOpened(
        object? sender,
        EventArgs e)
    {
        CenterOnPrimaryScreen();

        await RunAnimationAsync();

        Close();
    }

    private void CenterOnPrimaryScreen()
    {
        var screen = Screens.Primary;

        if (screen is null)
        {
            return;
        }

        var area = screen.WorkingArea;

        Position = new PixelPoint(
            area.X + ((area.Width - (int)Width) / 2),
            area.Y + ((area.Height - (int)Height) / 2));
    }

    private async Task RunAnimationAsync()
    {
        SplashCard.Opacity = 0;

        // Slower fade in
        for (var i = 0; i <= 20; i++)
        {
            SplashCard.Opacity = i / 20.0;
            await Task.Delay(30);
        }

        // Give people time to actually see it
        await Task.Delay(2000);

        // Slower fade out
        for (var i = 20; i >= 0; i--)
        {
            SplashCard.Opacity = i / 20.0;
            await Task.Delay(25);
        }
    }

    private void SkipAnimation(
        object? sender,
        EventArgs e)
    {
        Close();
    }
}
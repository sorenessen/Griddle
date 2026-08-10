using Avalonia.Media;
using Griddle.Core.Models;

namespace Griddle.App.Rendering;

public static class StrokeColorMapper
{
    public static Color ToAvaloniaColor(
        StrokeColor color)
    {
        return color switch
        {
            StrokeColor.Red => Colors.Red,
            StrokeColor.Orange => Colors.Orange,
            StrokeColor.Yellow => Colors.Yellow,
            StrokeColor.Green => Colors.LimeGreen,
            StrokeColor.Blue => Colors.DodgerBlue,
            StrokeColor.Purple => Colors.MediumPurple,
            StrokeColor.White => Colors.White,
            StrokeColor.Black => Colors.Black,
            _ => Colors.Red
        };
    }

    public static IBrush ToBrush(
        StrokeColor color)
    {
        return new SolidColorBrush(
            ToAvaloniaColor(color));
    }
}